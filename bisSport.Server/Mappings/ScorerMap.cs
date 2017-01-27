using bisSport.Domain.Core;
using bisSport.Domain.Core.Base;
using FluentNHibernate.Mapping;

namespace bisSport.Server.Mappings
{
  public class ScorerMap : ClassMap<Scorer>
  {
    public ScorerMap()
    {
      Table("Scorers");
      Id(x => x.Id);
      Map(x => x.Name);
      Map(x => x.TypeGuid).Column("TypeGuid");
      Map(x => x.Status);

      Map(x => x.Time);
      References<ParticipantBase>(x => x.Participant).Cascade.SaveUpdate();
    }
  }
}