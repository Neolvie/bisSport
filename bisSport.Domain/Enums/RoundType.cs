using System.ComponentModel;

namespace bisSport.Domain.Enums
{
  public enum RoundType
  {
    [Description("На вылет")]
    Playoff = 0,
    [Description("Групповой этап")]
    AllPlayAll = 1
  }
}