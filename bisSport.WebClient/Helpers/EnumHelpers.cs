using System;
using System.Linq;
using System.Web.Mvc;
using bisSport.Domain.Helpers;

namespace bisSport.WebClient.Helpers
{
  public static class EnumHelpers
  {
    public static SelectList ToSelectList<TEnum>(this TEnum enumObj)
      where TEnum : struct, IComparable, IFormattable, IConvertible
    {
      var values = from Enum e in Enum.GetValues(typeof(TEnum))
                   select new { Id = e, Name = e.GetDescription() };
      return new SelectList(values, "Id", "Name", enumObj);
    }

    public static SelectList StatusList()
    {
      return Domain.Enums.Status.Active.ToSelectList();
    }
  }
}
