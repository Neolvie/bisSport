using System;
using System.Collections.Generic;
using System.Linq;
using bisSport.Domain.Core;
using bisSport.Domain.Core.Base;
using bisSport.Domain.Core.Interfaces;
using bisSport.Domain.Enums;
using bisSport.Server.Helpers;
using bisSport.Server.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace bisSport.Tests.CRUD
{
  [TestClass]
  public class MatchTests : TestBase
  {
    [TestMethod]
    public void MatchCreate()
    {
      var match = CreateMatch(Repository, EventsTests.CreateSingleEvent(Repository));

      var matchFromDb = Repository.Matches.GetAll().FirstOrDefault(x => x.Id == match.Id);

      Assert.IsNotNull(matchFromDb);
      Assert.IsNotNull(matchFromDb.Area);
      Assert.IsNotNull(matchFromDb.Round);
      Assert.IsNotNull(matchFromDb.Result);
      Assert.IsTrue(matchFromDb.Participants.Any());
    }

    public static Match CreateMatch(UnitOfWork repository, EventBase @event, List<IParticipant> participants = null)
    {
      var match = repository.Matches.Create();
      var random = new Random();

      match.BeginDate = DateTime.Now.AddHours(1);
      match.Game = 1;

      if (participants != null && participants.Any())
      {
        for (int j = 0; j < 2; j++)
        {
          var player = participants[random.Next(0, participants.Count)];

          match.Participants.Add(player);
        }
      }
      else
      {
        for (int j = 0; j < 2; j++)
        {
          var player = PlayerTests.CreatePlayer(repository);

          match.Participants.Add(player);
        }
      }

      match.Name = $"{match.Participants[0].Name} vs {match.Participants[1].Name}";
      match.Area = AreaTests.CreateArea(repository);
      match.Round = RoundTests.CreateRound(repository);
      match.Result = ResultTests.CreateResult(repository, match.Participants.ToList());
      match.Event = @event.As<SingleEvent>();
      match.Save();

      return match;
    }

    public static void Simulate(UnitOfWork repository, Match match, Random random)
    {
      var @event = match.Event as SingleEvent;
      Assert.IsNotNull(@event, "Match cannot be created directly in MultiEvent.");

      Assert.AreNotEqual(match.Participants.Count, 0, "Match should have 2 participants atleast");
      Assert.IsTrue(match.Participants.Count > 1, "Match should have 2 participants atleast");
      var round = match.Round;
      var hasMaxScore = round.MaxPeriodPoints != null;

      if (hasMaxScore)
      {
        // For 2 participants only.
        match.Result = GenerateTwoParticipantResult(repository, match, random);
      }
      else
      {
        throw new NotImplementedException("Match withoun maximum score simulation is not implemented");
      }

      match.Save();
    }

    private static Result GenerateTwoParticipantResult(UnitOfWork repository, Match match, Random random)
    {
      var result = repository.Results.Create();
      result.CountPointsByWonPeriods = true;
      var round = match.Round;
      for (var i = 0; i < round.Periods; i++)
      {
        if (result.Scores.GroupBy(x => x.Participant).Any(x => x.Count(y => y.Point.GameResult == GameResult.Win) > (round.Periods / 2)))
          break;

        var periodNumber = i + 1;
        var maxScore = round.MaxPeriodPoints.Value;
        var winner = DefineWinner(match, random);
        var loser = match.Participants.FirstOrDefault(x => x.Id != winner.Id);
        var winnerPoints = maxScore - random.Next(2);
        var loserPoints = random.Next(winnerPoints);
        if (winnerPoints == maxScore)
          loserPoints = maxScore - random.Next(2) - 1;

        var winnerScore = repository.Scores.Create();
        winnerScore.Period = periodNumber;
        winnerScore.Name = "Победитель";
        winnerScore.Participant = winner;
        winnerScore.Point = new Point() {Count = winnerPoints, GameResult = GameResult.Win};
        winnerScore.Save();

        var loserScore = repository.Scores.Create();
        loserScore.Period = periodNumber;
        loserScore.Name = "Проигравший";
        loserScore.Participant = loser;
        loserScore.Point = new Point() {Count = loserPoints, GameResult = GameResult.Lose};
        loserScore.Save();

        result.Scores.Add(winnerScore);
        result.Scores.Add(loserScore);
      }
      result.Save();

      return result;
    }

    public static IParticipant DefineWinner(Match match, Random winnersRandom)
    {
      var winnerNumber = winnersRandom.Next(match.Participants.Count);
      return match.Participants[winnerNumber];
    }
  }
}