using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using bisSport.Domain.Core;
using bisSport.Domain.Core.Base;
using FluentNHibernate.Mapping;

namespace bisSport.Server.Mappings
{
  public class EventBaseMap : ClassMap<EventBase>
  {
    public EventBaseMap()
    {
      DiscriminateSubClassesOnColumn("TypeGuid");
      Table("Events");

      Id(x => x.Id);
      Map(x => x.Name);
      Map(x => x.BeginDate);
      Map(x => x.EndDate);
      Map(x => x.Status);

      References<Address>(x => x.Address).Cascade.All();
      References<OrganizerBase>(x => x.Organizer).Cascade.SaveUpdate();
      References<EventBase>(x => x.MainEvent, "MainEventId").Cascade.None();
      HasMany<EventBase>(x => x.Events).KeyColumn("MainEventId").Cascade.AllDeleteOrphan();
      HasManyToMany<ParticipantBase>(x => x.Participants).Cascade.SaveUpdate().Table("EventParticipants");
    }
  }
}
