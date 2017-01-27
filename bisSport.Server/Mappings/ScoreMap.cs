using bisSport.Domain.Core;
using bisSport.Domain.Core.Base;
using FluentNHibernate.Mapping;

namespace bisSport.Server.Mappings
{
  public class ScoreMap : ClassMap<Score>
  {
    public ScoreMap()
    {
      Table("Scores");
      Id(x => x.Id);
      Map(x => x.Name);
      Map(x => x.TypeGuid).Column("TypeGuid");
      Map(x => x.Status);

      Map(x => x.Period);
      References<ParticipantBase>(x => x.Participant, "ParticipantId").Cascade.SaveUpdate();
      References(x => x.Point, "PointId").Cascade.All();
    }
  }
}