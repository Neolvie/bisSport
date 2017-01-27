using System.ComponentModel.DataAnnotations;
using bisSport.Domain.Core.Base;
using bisSport.Domain.Core.Interfaces;
using bisSport.Domain.Helpers;

namespace bisSport.Domain.Core
{
  public class Address : Entity, IAddress
  {
    [Display(Name = "Адрес")]
    [Required(ErrorMessage = "Не заполнено обязательное поле"), MinLength(1, ErrorMessage = "Не заполнено обязательное поле")]
    public virtual string PostAddress { get; set; }

    [Display(Name = "Широта")]
    public virtual double? Latitude { get; set; }

    [Display(Name = "Долгота")]
    public virtual double? Longitude { get; set; }

    [Display(Name = "Телефон")]
    public virtual string Phone { get; set; }
  }
}