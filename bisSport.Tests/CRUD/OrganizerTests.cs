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
  public class OrganizerTests : TestBase
  {
    [TestMethod]
    public void SportClubCreate()
    {
      var sportClub = CreateSportClub(Repository);

      var sportClubFromDb = Repository.SportClubs.GetAll().FirstOrDefault(x => x.Id == sportClub.Id);

      Assert.IsNotNull(sportClubFromDb);
    }

    public static SportClub CreateSportClub(UnitOfWork repository)
    {
      var sportClub = repository.SportClubs.Create();
      var random = new Random();

      sportClub.Name = $"Тестовый спортивный клуб {random.Next(100, 400)}";
      sportClub.Address = repository.Addresses.Create();
      sportClub.Address.PostAddress = "г. Ижевск, ул.Кирова, 65";
      sportClub.Address.Name = sportClub.Address.PostAddress;
      sportClub.Address.Latitude = 44.8;
      sportClub.Address.Longitude = 45.5;
      sportClub.Address.Phone = "8(800)555-35-36";
      sportClub.Address.Save();
      sportClub.SportKinds.Add(SportKind.Football);
      sportClub.SportKinds.Add(SportKind.Tennis);

      sportClub.Save();

      return sportClub;
    }
  }
}