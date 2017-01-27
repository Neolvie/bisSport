using bisSport.Domain.Core;
using bisSport.Domain.Core.Base;
using FluentNHibernate.Mapping;

namespace bisSport.Server.Mappings
{
  public class MatchMap : ClassMap<Match>
  {
    public MatchMap()
    {
      Table("Matches");

      Id(x => x.Id);
      Map(x => x.Name);
      Map(x => x.BeginDate);
      Map(x => x.Game);
      Map(x => x.TypeGuid).Column("TypeGuid");
      Map(x => x.Status);

      HasManyToMany<ParticipantBase>(x => x.Participants).Cascade.None().Table("MatchParticipants");
      References(x => x.Event, "EventId").Cascade.None();
      References(x => x.Round, "RoundId").Cascade.SaveUpdate();
      References(x => x.Area, "AreaId").Cascade.SaveUpdate();
      References(x => x.Result, "ResultId").Cascade.All();
      References(x => x.PrevMatch, "PrevMatchId").Cascade.SaveUpdate();
    }
  }
}