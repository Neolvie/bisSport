using System.ComponentModel;

namespace bisSport.Domain.Enums
{
  public enum SportKind
  {
    [Description("Теннис")]
    Tennis = 0,
    [Description("Футбол")]
    Football = 1,
    [Description("Биатлон")]
    Biathlon = 2,
    [Description("Настольный теннис")]
    PinPong = 3
  }
}