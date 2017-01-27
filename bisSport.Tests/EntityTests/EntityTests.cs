using System.Linq;
using bisSport.Domain.Core;
using bisSport.Server.Helpers;
using bisSport.Server.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace bisSport.Tests.EntityTests
{
  [TestClass]
  public class EntityTests : TestBase
  {
    [TestMethod]
    public void EntityInstanceCreate()
    {
      var entityGuid = Domain.Helpers.CoreTypes.CoreTypeGuids[typeof(Team)];

      var entity = Domain.Helpers.CoreTypes.CreateEntityInstance(entityGuid);

      Assert.IsNotNull(entity);

      entity.Name = entity.GetType().Name;

      entity.Save();
      var entityId = entity.Id;

      Repository.Session.Clear();

      var entityFromDb = Repository.Teams.GetAll().FirstOrDefault(x => x.Id == entityId);

      Assert.IsNotNull(entityFromDb);
      Assert.AreNotSame(entityFromDb, entity);
      Assert.IsTrue(entityFromDb.Name == entity.Name);

      entityFromDb.Delete();

      entityFromDb = Repository.Teams.GetAll().FirstOrDefault(x => x.Id == entityId);

      Assert.IsNull(entityFromDb);
    }
  }
}