using bisSport.Domain.Core;
using FluentNHibernate.Mapping;

namespace bisSport.Server.Mappings
{
  public class AreaMap : ClassMap<Area>
  {
    public AreaMap()
    {
      Table("Areas");
      Id(x => x.Id);
      Map(x => x.Name);
      Map(x => x.TypeGuid).Column("TypeGuid");
      Map(x => x.Status);

      Map(x => x.AreaType);
      Map(x => x.Audience);
      Map(x => x.LocationType);
      References(x => x.Address, "AddressId").Cascade.All();
    }
  }
}