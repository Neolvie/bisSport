using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using bisSport.Domain.Core.Interfaces;

namespace bisSport.Domain.Core.Base
{
  public abstract class EventBase : Entity, IEvent
  {
    public virtual DateTime? BeginDate { get; set; }

    public virtual DateTime? EndDate { get; set; }

    public virtual IList<IParticipant> Participants { get; set; }

    public virtual IOrganizer Organizer { get; set; }

    [Required(ErrorMessage = "Не заполнено обязательное поле")]
    public virtual IAddress Address { get; set; }

    public virtual IList<IEvent> Events { get; set; }

    public virtual IEvent MainEvent { get; set; }

    protected EventBase()
    {
      Participants = new List<IParticipant>();
      Events = new List<IEvent>();
    }
  }
}