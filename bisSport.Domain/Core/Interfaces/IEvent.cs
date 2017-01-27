using System;
using System.Collections.Generic;
using bisSport.Domain.Enums;

namespace bisSport.Domain.Core.Interfaces
{
  public interface IEvent : IEntity
  {
    DateTime? BeginDate { get; set; }

    DateTime? EndDate { get; set; }

    IList<IParticipant> Participants { get; set; }
    
    IOrganizer Organizer { get; set; }
    
    IAddress Address { get; set; }

    IList<IEvent> Events { get; set; } 
  }
}