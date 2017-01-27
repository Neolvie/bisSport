using System.Collections.Generic;
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
  public class ResultTests : TestBase
  {
    [TestMethod]
    public void ResultCreate()
    {
      var result = CreateResult(Repository, new List<IParticipant>());
      var resultId = result.Id;

      var resultFromDb = Repository.Results.GetAll().FirstOrDefault(x => x.Id == resultId);

      Assert.IsNotNull(resultFromDb);
      Assert.IsTrue(resultFromDb.Scores.Any());
      Assert.IsTrue(resultFromDb.Scores.Any(x => x.Point.GameResult == GameResult.Win));
    }

    public static Result CreateResult(UnitOfWork repository, List<IParticipant> participants)
    {
      var result = repository.Results.Create();

      var winner = ScoreTests.CreateScore(repository, participants.FirstOrDefault() ?? PlayerTests.CreatePlayer(repository), GameResult.Win, 2);
      var loser = ScoreTests.CreateScore(repository, participants.LastOrDefault() ?? PlayerTests.CreatePlayer(repository), GameResult.Lose, 0);

      result.Scores.Add(winner);
      result.Scores.Add(loser);

      result.Save();

      return result;
    }
  }
}