using System.Linq;
using bisSport.Domain.Core;
using bisSport.Domain.Enums;
using bisSport.Server.Helpers;
using bisSport.Server.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace bisSport.Tests.CRUD
{
  [TestClass]
  public class PointTests : TestBase
  {
    [TestMethod]
    public void PointCreate()
    {
      var point = CreatePoint(Repository, GameResult.Draw, 1);

      var pointFromDb = Repository.Points.GetAll().FirstOrDefault(x => x.Id == point.Id);

      Assert.IsNotNull(pointFromDb);
      Assert.AreEqual(pointFromDb, point);
      Assert.AreEqual(pointFromDb.GameResult, point.GameResult);
    }

    public static Point CreatePoint(UnitOfWork repository, GameResult result, int points)
    {
      var point = repository.Points.Create();
      point.GameResult = result;
      point.Count = points;
      point.Save();

      return point;
    }
  }
}