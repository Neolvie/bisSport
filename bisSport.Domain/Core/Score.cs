using System.ComponentModel.DataAnnotations;
using bisSport.Domain.Core.Base;
using bisSport.Domain.Core.Interfaces;

namespace bisSport.Domain.Core
{
  public class Score : Entity
  {
    [Required(ErrorMessage = "Не заполнено обязательное поле")]
    public virtual IParticipant Participant { get; set; }

    [Required(ErrorMessage = "Не заполнено обязательное поле")]
    public virtual Point Point { get; set; }

    public virtual int Period { get; set; }
  }
}