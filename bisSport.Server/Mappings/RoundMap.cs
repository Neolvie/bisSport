using bisSport.Domain.Core;
using FluentNHibernate.Mapping;

namespace bisSport.Server.Mappings
{
  public class RoundMap : ClassMap<Round>
  {
    public RoundMap()
    {
      Table("Rounds");
      Id(x => x.Id);
      Map(x => x.Name);
      Map(x => x.TypeGuid).Column("TypeGuid");
      Map(x => x.Status);

      Map(x => x.Type);
      Map(x => x.MinParticipants);
      Map(x => x.MaxParticipants);
      Map(x => x.Games);
      Map(x => x.Periods);
      Map(x => x.MaxPeriodPoints);
      Map(x => x.RoundNumber);
      Map(x => x.WinnerCount);
      Map(x => x.ParticipantsPerGroup);
      HasMany(x => x.Points).Cascade.AllDeleteOrphan().KeyColumn("RoundId");
      HasMany(x => x.Groups).Cascade.AllDeleteOrphan().KeyColumn("RoundId");
    }
  }
}