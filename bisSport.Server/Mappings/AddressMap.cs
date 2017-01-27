using System.Security.Cryptography.X509Certificates;
using bisSport.Domain.Core;
using FluentNHibernate.Mapping;

namespace bisSport.Server.Mappings
{
  public class AddressMap : ClassMap<Address>
  {
    public AddressMap()
    {
      Table("Addresses");
      Id(x => x.Id);
      Map(x => x.Name);
      Map(x => x.TypeGuid).Column("TypeGuid");
      Map(x => x.Status);

      Map(x => x.PostAddress);
      Map(x => x.Longitude);
      Map(x => x.Latitude);
      Map(x => x.Phone);
    }
  }
}