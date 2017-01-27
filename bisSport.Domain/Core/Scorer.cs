using System;
using System.ComponentModel.DataAnnotations;
using bisSport.Domain.Core.Base;
using bisSport.Domain.Core.Interfaces;

namespace bisSport.Domain.Core
{
  public class Scorer : Entity
  {
    [Required(ErrorMessage = "Не заполнено обязательное поле")]
    public virtual IParticipant Participant { get; set; }

    public virtual TimeSpan? Time { get; set; }
  }
}