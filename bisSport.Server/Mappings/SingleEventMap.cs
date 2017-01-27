using bisSport.Domain.Core;
using FluentNHibernate.Mapping;

namespace bisSport.Server.Mappings
{
  public class SingleEventMap : SubclassMap<SingleEvent>
  {
    public SingleEventMap()
    {
      DiscriminatorValue("3288FC3E-0490-4DF9-83B6-0E5F8573A93A");

      References(x => x.Structure).Cascade.SaveUpdate();
      HasMany(x => x.Matches).KeyColumn("EventId").Cascade.AllDeleteOrphan();
    }
  }
}