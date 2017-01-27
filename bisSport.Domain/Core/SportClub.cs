using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using bisSport.Domain.Core.Base;
using bisSport.Domain.Enums;
using bisSport.Domain.Helpers;

namespace bisSport.Domain.Core
{
  public class SportClub : OrganizerBase
  {
    [Required(ErrorMessage = "Не заполнено обязательное поле")]
    public virtual Address Address { get; set; }

    [ValidateCollection(1, ErrorMessage = "Необходимо указать минимум один вид спорта")]
    public virtual IList<SportKind> SportKinds { get; set; }

    public virtual IList<Area> Areas { get; set; } 

    public SportClub()
    {
      SportKinds = new List<SportKind>();
      Areas = new List<Area>();
    } 
  }
}