using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using bisSport.Domain.Core;

namespace bisSport.WebClient.Models
{
  public class PlayerInfoViewModel
  {
    public Player Player { get; private set; }

    public int TotalGoals { get; private set; }

    public List<Match> ScoredMatches { get; set; }

    public PlayerInfoViewModel(Player player, List<Match> matches)
    {
      ScoredMatches = matches;
      Player = player;

      TotalGoals = matches
        .Where(x => x.Result != null).Count(x => x.Result.Scores.Any(s => s.Point.Scorers.Any(g => g.Participant.Id == Player.Id)));
    }

    public PlayerInfoViewModel(Player player)
    {
      TotalGoals = 0;
      ScoredMatches = new List<Match>();
      Player = player;
    }
  }
}