using bisSport.Domain.Core;
using FluentNHibernate.Mapping;

namespace bisSport.Server.Mappings
{
  public class SportTypeMap : ClassMap<SportType>
  {
    public SportTypeMap()
    {
      Table("SportTypes");
      Id(x => x.Id);
      Map(x => x.Name);
      Map(x => x.TypeGuid).Column("TypeGuid");
      Map(x => x.Status);

      Map(x => x.CleanTime);
      Map(x => x.CleanGameDuration);
      Map(x => x.GameDuration);
      Map(x => x.SportKind);
    }
  }
}