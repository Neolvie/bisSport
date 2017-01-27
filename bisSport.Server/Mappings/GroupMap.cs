using bisSport.Domain.Core;
using bisSport.Domain.Core.Base;
using FluentNHibernate.Mapping;

namespace bisSport.Server.Mappings
{
  public class GroupMap : ClassMap<Group>
  {
    public GroupMap()
    {
      Table("Groups");
      Id(x => x.Id);
      Map(x => x.Name);
      Map(x => x.TypeGuid).Column("TypeGuid");
      Map(x => x.Status);

      HasManyToMany<ParticipantBase>(x => x.Participants).Cascade.SaveUpdate().Table("RoundGroups");
    }
  }
}