using System.Linq;
using bisSport.Domain.Core;
using bisSport.Server.Helpers;
using bisSport.Server.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace bisSport.Tests.CRUD
{
  [TestClass]
  public class AddressTests : TestBase
  {
    [TestMethod]
    public void AddressCreate()
    {
      var address = CreateAddress(Repository);

      var addressFromDb = Repository.Addresses.GetAll().FirstOrDefault(x => x.Id == address.Id);

      Assert.IsNotNull(addressFromDb);
      Assert.AreEqual(addressFromDb, address);
    }

    public static Address CreateAddress(UnitOfWork repository)
    {
      var address = repository.Addresses.Create();
      address.PostAddress = "г. Ижевск, ул.Ленина, 33";
      address.Name = address.PostAddress;
      address.Latitude = 43.8;
      address.Longitude = 48.5;
      address.Phone = "8(800)555-35-35";

      address.Save();

      return address;
    }
  }
}