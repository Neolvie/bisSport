using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bisSport.Domain.Enums
{
  public enum Status
  {
    [Description("Действующая")]
    Active = 0,
    [Description("Закрытая")]
    Closed = 1,
  }
}
