using System;
using System.ComponentModel.DataAnnotations;
using bisSport.Domain.Core.Base;
using bisSport.Domain.Core.Interfaces;
using bisSport.Domain.Enums;

namespace bisSport.Domain.Core
{
  public class SportType : Entity
  {
    [Display(Name = "Вид спорта")]
    [Required(ErrorMessage = "Не заполнено обязательное поле")]
    public virtual SportKind SportKind { get; set; }

    [Display(Name = "Чистое время")]
    public virtual bool CleanTime { get; set; }

    [Display(Name = "Продолжительность матча")]
    public virtual int? GameDuration { get; set; }

    [Display(Name = "Чистое время матча")]
    public virtual int? CleanGameDuration { get; set; }
  }
}