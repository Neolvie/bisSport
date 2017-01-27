using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace bisSport.Domain.Helpers
{
  public class ValidateCollectionAttribute : ValidationAttribute
  {
    private readonly int _minElements;

    public ValidateCollectionAttribute(int minElements)
    {
      _minElements = minElements;
    }

    public override bool IsValid(object value)
    {
      var collection = value as IList;

      return collection?.Count >= _minElements;
    }
  }
}