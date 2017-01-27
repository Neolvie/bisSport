using System.Linq;
using bisSport.Domain.Core;
using bisSport.Domain.Enums;
using bisSport.Server.Helpers;
using bisSport.Server.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace bisSport.Tests.CRUD
{
  [TestClass]
  public class AreaTests : TestBase
  {
    [TestMethod]
    public void AreaCreate()
    {
      var area = CreateArea(Repository);
      var areaId = area.Id;

      var areaFromDb = Repository.Areas.GetAll().FirstOrDefault(x => x.Id == areaId);
      
      Assert.AreEqual(areaFromDb, area);
    }

    public static Area CreateArea(UnitOfWork repository)
    {
      var area = repository.Areas.Create();

      area.AreaType = AreaType.Club;
      area.Audience = 20;
      area.LocationType = LocationType.Indoor;
      area.Address = AddressTests.CreateAddress(repository);

      area.Save();

      return area;
    }
  }
}