using bisSport.Domain.Core.Base;
using FluentNHibernate.Mapping;

namespace bisSport.Server.Mappings
{
  public class OrganizerBaseMap : ClassMap<OrganizerBase>
  {
    public OrganizerBaseMap()
    {
      DiscriminateSubClassesOnColumn("TypeGuid");
      Table("Organizers");

      Id(x => x.Id);
      Map(x => x.Name);
      Map(x => x.Status);
    }
  }
}