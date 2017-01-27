using bisSport.Domain.Core;
using FluentNHibernate.Mapping;
using NHibernate.Mapping;

namespace bisSport.Server.Mappings
{
  public class MultiEventMap : SubclassMap<MultiEvent>
  {
    public MultiEventMap()
    {
      DiscriminatorValue("2829A68D-7D4A-458F-873A-09BDAB566434");
    }
  }
}