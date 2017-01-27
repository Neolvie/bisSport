using bisSport.Domain.Core;
using FluentNHibernate.Mapping;

namespace bisSport.Server.Mappings
{
  public class PointMap : ClassMap<Point>
  {
    public PointMap()
    {
      Table("Points");
      Id(x => x.Id);
      Map(x => x.Name);
      Map(x => x.Status);

      Map(x => x.Count);
      Map(x => x.GameResult);
      Map(x => x.TypeGuid).Column("TypeGuid");
      HasMany(x => x.Scorers).Cascade.AllDeleteOrphan().KeyColumn("PointId");
    }
  }
}