using System.Collections.Generic;
using bisSport.Domain.Core.Base;
using bisSport.Domain.Core.Interfaces;

namespace bisSport.Domain.Core
{
  public class Group : Entity
  {
    public virtual IList<IParticipant> Participants { get; set; } 
  }
}