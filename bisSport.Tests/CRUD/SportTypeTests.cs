using System;
using System.Linq;
using bisSport.Domain.Core;
using bisSport.Domain.Enums;
using bisSport.Server.Helpers;
using bisSport.Server.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace bisSport.Tests.CRUD
{
  [TestClass]
  public class SportTypeTests : TestBase
  {
    public void SportTypeCreate()
    {
      var sportType = CreateSportType(Repository, SportKind.PinPong);

      var sportTypeId = sportType.Id;

      var sportTypeFromDb = Repository.SportTypes.GetAll().FirstOrDefault(x => x.Id == sportTypeId);

      Assert.IsNotNull(sportTypeFromDb);
      Assert.AreNotSame(sportTypeFromDb, sportType);
      Assert.IsTrue(sportTypeFromDb.SportKind == SportKind.PinPong);
    }

    public static SportType CreateSportType(UnitOfWork repository, SportKind sportKind = SportKind.Football, string name = "Футбол")
    {
      var sportType = repository.SportTypes.Create();
      sportType.SportKind = sportKind;
      sportType.CleanGameDuration = null;
      sportType.CleanTime = false;
      sportType.GameDuration = 90;
      sportType.Name = name;

      sportType.Save();

      return sportType;
    }
  }
}