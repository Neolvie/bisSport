using bisSport.Domain.Core;
using FluentNHibernate.Mapping;

namespace bisSport.Server.Mappings
{
  public class ResultMap : ClassMap<Result>
  {
    public ResultMap()
    {
      Table("Results");
      Id(x => x.Id);
      Map(x => x.Name);
      Map(x => x.TypeGuid).Column("TypeGuid");
      Map(x => x.CountPointsByWonPeriods);
      Map(x => x.Status);

      HasMany(x => x.Scores).Cascade.AllDeleteOrphan().KeyColumn("ResultId");
    }
  }
}