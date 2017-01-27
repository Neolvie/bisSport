using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using bisSport.Domain.Core.Base;
using bisSport.Domain.Core.Interfaces;
using bisSport.Domain.Enums;

namespace bisSport.Domain.Core
{
  public class Point : Entity
  {
    [Required(ErrorMessage = "Не заполнено обязательное поле")]
    public virtual GameResult GameResult { get; set; }

    [Display(Name = "Очки")]
    public virtual double Count { get; set; }

    public virtual IList<Scorer> Scorers { get; set; }

    public Point()
    {
      Scorers = new List<Scorer>();
    }
  }
}