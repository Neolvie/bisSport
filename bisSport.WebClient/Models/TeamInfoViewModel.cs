using System.Collections.Generic;
using bisSport.Domain.Core;
using bisSport.Domain.Core.Base;

namespace bisSport.WebClient.Models
{
  public class TeamInfoViewModel
  {
    public readonly Team Team;

    public List<ParticipantBase> Players; 

    public TeamInfoViewModel(Team team)
    {
      Team = team;
    } 
  }
}