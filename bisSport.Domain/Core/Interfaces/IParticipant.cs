using System.Collections.Generic;
using bisSport.Domain.Enums;

namespace bisSport.Domain.Core.Interfaces
{
  public interface IParticipant : IEntity
  {   
    ParticipantType Type { get; } 
  }
}