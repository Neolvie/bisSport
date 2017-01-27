using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using bisSport.Domain.Core.Base;
using bisSport.Domain.Core.Interfaces;

namespace bisSport.Domain.Core
{
  public class SingleEvent : EventBase
  {
    [Required(ErrorMessage = "Не заполнено обязательное поле")]
    public virtual Structure Structure { get; set; }

    public virtual IList<Match> Matches { get; set; }

    public SingleEvent()
    {
      Matches = new List<Match>();
    } 
  }
}