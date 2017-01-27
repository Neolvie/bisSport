using System.Collections.Generic;
using System.Linq;
using bisSport.Domain.Core;
using bisSport.Domain.Core.Interfaces;
using bisSport.Domain.Enums;

namespace bisSport.WebClient.Helpers
{
  public static class ScoreHelpers
  {
    public static IParticipant DefinePlayOffWinner(List<Score> scores, Round round)
    {
      var teamCount = new Dictionary<IParticipant, double>();

      foreach (var score in scores)
      {
        if (!teamCount.ContainsKey(score.Participant))
          teamCount.Add(score.Participant, 0);

        teamCount[score.Participant] += score.Point.Count;
      }

      // Если все результаты одинаковые, то в групповом этапе победителей в личных встречах нет.
      if (teamCount.Values.Distinct().Count() == 1 && round.Type == RoundType.AllPlayAll)
        return null;

      return teamCount.OrderByDescending(x => x.Value).First().Key;
    }

    public static Dictionary<IParticipant, double> CalculateAllPlayAllScore(List<Match> matches, Group @group)
    {
      var results = new Dictionary<IParticipant, double>();
      if (!matches.Any() || @group == null)
        return results;

      var round = matches[0].Round;

      foreach (var team in @group.Participants)
      {
        var teamMatches = matches
          .Where(x => x.Participants.Any(s => s.Id == team.Id)).ToList();

        var points = teamMatches
          .Select(match => match.Result.Scores.Single(x => x.Participant.Id == team.Id))
          .Select(result => round.Points.Single(x => x.GameResult == result.Point.GameResult).Count).Sum();

        results.Add(team, points);
      }

      return results.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
    }

    public static double GetParticipantResult(Result result, IParticipant participant)
    {
      var scores = result.Scores.Where(x => Equals(x.Participant, participant));
      if (result.CountPointsByWonPeriods)
        return scores.Count(x => x.Point.GameResult == GameResult.Win);
      else
        return scores.Sum(x => x.Point.Count);
    }
  }
}