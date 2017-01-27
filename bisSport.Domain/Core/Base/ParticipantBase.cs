using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using bisSport.Domain.Core.Interfaces;
using bisSport.Domain.Enums;

namespace bisSport.Domain.Core.Base
{
  public abstract class ParticipantBase : Entity, IParticipant
  {
    public virtual ParticipantType Type { get; protected set; }

    [Display(Name = "Команда")]
    public virtual Team Team { get; set; }
  }
}