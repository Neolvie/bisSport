using bisSport.Domain.Core;
using FluentNHibernate.Mapping;

namespace bisSport.Server.Mappings
{
  public class StructureMap : ClassMap<Structure>
  {
    public StructureMap()
    {
      Table("Structures");
      Id(x => x.Id);
      Map(x => x.Name);
      Map(x => x.TypeGuid).Column("TypeGuid");
      Map(x => x.Status);

      References(x => x.SportType, "SportTypeId").Cascade.SaveUpdate();
      HasMany(x => x.Rounds).KeyColumn("StructureId").Cascade.AllDeleteOrphan();
    }
  }
}