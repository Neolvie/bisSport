using System.Collections.Generic;
using bisSport.Domain.Core;

namespace bisSport.WebClient.Areas.Tennis.Models
{
  public class MatchListViewModel
  {
    public List<Match> Matches { get; set; }

    public Round Round { get; set; }

    public bool AsPartial { get; set; }

    public MatchListViewModel(Round round, List<Match> matches, bool asPartial = false)
    {
      Matches = matches;
      Round = round;
      AsPartial = asPartial;
    }

    public MatchListViewModel()
    {
      Matches = new List<Match>();
    }
  }
}