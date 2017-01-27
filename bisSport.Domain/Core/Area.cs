using System.ComponentModel.DataAnnotations;
using bisSport.Domain.Core.Base;
using bisSport.Domain.Core.Interfaces;
using bisSport.Domain.Enums;

namespace bisSport.Domain.Core
{
  public class Area : Entity
  {
    [Required(ErrorMessage = "Не заполнено обязательное поле")]
    public virtual Address Address { get; set; }

    [Required(ErrorMessage = "Не заполнено обязательное поле")]
    public virtual AreaType AreaType { get; set; }

    public virtual int Audience { get; set; }

    [Required(ErrorMessage = "Не заполнено обязательное поле")]
    public virtual LocationType LocationType { get; set; }
  }
}