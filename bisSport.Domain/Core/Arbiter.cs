using bisSport.Domain.Core.Base;
using bisSport.Domain.Enums;
using bisSport.Domain.Helpers;

namespace bisSport.Domain.Core
{
  public class Arbiter : ParticipantBase
  {
    public virtual bool IsHead { get; set; }

    public Arbiter()
    {
      Type = ParticipantType.Arbiter;
    }
  }
}