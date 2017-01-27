using bisSport.Domain.Core;
using bisSport.Domain.Core.Base;
using FluentNHibernate.Mapping;

namespace bisSport.Server.Mappings
{
  public class TeamMap : SubclassMap<Team>
  {
    public TeamMap()
    {
      DiscriminatorValue("72396685-C65C-48ED-81B0-1B87DFF6577B");

      HasMany(x => x.Players).Cascade.All().KeyColumn("TeamId").AsBag();
    }
  }
}