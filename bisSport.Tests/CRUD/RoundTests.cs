using System.Linq;
using bisSport.Domain.Core;
using bisSport.Domain.Enums;
using bisSport.Server.Helpers;
using bisSport.Server.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace bisSport.Tests.CRUD
{
  [TestClass]
  public class RoundTests : TestBase
  {
    [TestMethod]
    public void RoundCreate()
    {
      var round = CreateRound(Repository);

      var roundFromDb = Repository.Rounds.GetAll().FirstOrDefault(x => x.Id == round.Id);

      Assert.AreEqual(round, roundFromDb);
    }

    public static Round CreateRound(UnitOfWork repository, RoundType roundType = RoundType.Playoff, int number = 1)
    {
      var round = repository.Rounds.Create();

      round.Type = roundType;
      round.RoundNumber = 1;
      round.Games = 2;
      round.Periods = 2;
      round.RoundNumber = 1;
      round.WinnerCount = 1;

      round.Points.Add(PointTests.CreatePoint(repository, GameResult.Win, 3));
      round.Points.Add(PointTests.CreatePoint(repository, GameResult.Draw, 1));
      round.Points.Add(PointTests.CreatePoint(repository, GameResult.Lose, 0));

      round.Save();

      return round;
    }
  }
}