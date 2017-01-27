using bisSport.Domain.Core.Base;
using FluentNHibernate.Mapping;

namespace bisSport.Server.Mappings
{
  public class ParticipantBaseMap : ClassMap<ParticipantBase>
  {
    public ParticipantBaseMap()
    {
      DiscriminateSubClassesOnColumn("TypeGuid");
      Table("Participants");
      Map(x => x.Status);

      Id(x => x.Id);
      Map(x => x.Name);
      Map(x => x.Type);
      References(x => x.Team, "TeamId").Cascade.None();
    }
  }
}