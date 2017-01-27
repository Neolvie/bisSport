using bisSport.Domain.Core;
using FluentNHibernate.Mapping;

namespace bisSport.Server.Mappings
{
  public class PlayerMap : SubclassMap<Player>
  {
    public PlayerMap()
    {
      DiscriminatorValue("EC96138A-1591-4207-86B7-8335B20019DC");
    }
  }
}