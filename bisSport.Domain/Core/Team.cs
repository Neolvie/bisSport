using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using bisSport.Domain.Core.Base;
using bisSport.Domain.Core.Interfaces;
using bisSport.Domain.Enums;

namespace bisSport.Domain.Core
{
  public class Team : ParticipantBase
  {
    public Team()
    {
      Type = ParticipantType.Team;
    }

    [Display(Name = "Игроки")]
    public virtual IList<Player> Players { get; set; }
  }
}