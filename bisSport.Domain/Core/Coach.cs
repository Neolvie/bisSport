using bisSport.Domain.Core.Base;
using bisSport.Domain.Enums;

namespace bisSport.Domain.Core
{
  public class Coach : ParticipantBase
  {
    public Coach()
    {
      Type = ParticipantType.Coach;
    }
  }
}