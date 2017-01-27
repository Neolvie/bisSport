using System;
using bisSport.Domain.Core.Exceptions;
using bisSport.Server.Helpers;
using bisSport.Tests.CRUD;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace bisSport.Tests.EntityTests
{
  [TestClass]
  public class EntityEventTests : TestBase
  {
    [TestMethod]
    public void ValidationTest()
    {
      var address = AddressTests.CreateAddress(Repository);
      address.PostAddress = null;

      try
      {
        address.Save();
      }
      catch (ValidationException ex)
      {
        Assert.IsTrue(!string.IsNullOrEmpty(ex.Message));
      }
      catch (Exception ex)
      {
        Assert.Fail($"No validation error! Error: {ex}");
      }  
    }
  }
}