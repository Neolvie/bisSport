using bisSport.Domain.Core;
using FluentNHibernate.Mapping;

namespace bisSport.Server.Mappings
{
  public class SportClubMap : SubclassMap<SportClub>
  {
    public SportClubMap()
    {
      DiscriminatorValue("FD594871-02D9-4027-8701-3BEFC8E0AA06");

      References(x => x.Address).Cascade.All();
      HasMany(x => x.SportKinds).Cascade.All().Table("SportClubKinds").Element("SportKind");
    }
  }
}