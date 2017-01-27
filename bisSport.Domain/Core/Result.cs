using System.Collections.Generic;
using System.Linq;
using bisSport.Domain.Core.Base;
using bisSport.Domain.Core.Interfaces;
using bisSport.Domain.Enums;
using bisSport.Domain.Helpers;

namespace bisSport.Domain.Core
{
  public class Result : Entity
  {
    [ValidateCollection(1, ErrorMessage = "Необходимо указать минимум один счет")]
    public virtual IList<Score> Scores { get; set; }

    public virtual bool CountPointsByWonPeriods { get; set; }

    public Result()
    {
      Scores = new List<Score>();
      CountPointsByWonPeriods = false;
    }
  }
}