using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using bisSport.Domain.Core.Base;
using bisSport.Domain.Core.Interfaces;
using bisSport.Domain.Enums;
using bisSport.Domain.Helpers;

namespace bisSport.Domain.Core
{
  public class Structure : Entity
  {
    [ValidateCollection(1, ErrorMessage = "Необходимо добавить минимум один раунд")]
    public virtual IList<Round> Rounds { get; set; }

    [Required(ErrorMessage = "Не заполнено обязательное поле")]
    public virtual SportType SportType { get; set; }

    public Structure()
    {
      Rounds = new List<Round>();
    }
  }
}