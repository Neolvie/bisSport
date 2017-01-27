using System.Linq;
using bisSport.Domain.Core;
using bisSport.Domain.Core.Interfaces;
using bisSport.Domain.Enums;
using bisSport.Server.Helpers;
using bisSport.Server.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace bisSport.Tests.CRUD
{
  [TestClass]
  public class ScoreTests : TestBase
  {
    [TestMethod]
    public void ScoreCreate()
    {
      var score = CreateScore(Repository, null, GameResult.Win, 3);

      var scoreFromDb = Repository.Scores.GetAll().FirstOrDefault(x => x.Id == score.Id);

      Assert.IsNotNull(scoreFromDb);
      Assert.AreEqual(score, scoreFromDb);
      Assert.IsNotNull(scoreFromDb.Point);
      Assert.IsNotNull(scoreFromDb.Participant);
    }

    public static Score CreateScore(UnitOfWork repository, IParticipant participant, GameResult result, int count)
    {
      var score = repository.Scores.Create();
      score.Participant = participant ?? PlayerTests.CreatePlayer(repository);
      score.Period = 1;
      score.Point = PointTests.CreatePoint(repository, result, count);

      score.Save();

      return score;
    }
  }
}