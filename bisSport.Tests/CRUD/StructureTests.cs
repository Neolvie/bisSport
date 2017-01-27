using System.Linq;
using bisSport.Domain.Core;
using bisSport.Domain.Enums;
using bisSport.Server;
using bisSport.Server.Helpers;
using bisSport.Server.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace bisSport.Tests.CRUD
{
  [TestClass]
  public class StructureTests : TestBase
  {
    [TestMethod]
    public void StructureCreate()
    {
      var structure = CreateStructure(Repository);

      Repository.Session.Clear();

      var structureFromDb = Repository.Structures.GetAll().FirstOrDefault(x => x.Id == structure.Id);

      Assert.IsNotNull(structureFromDb);
      Assert.AreNotSame(structure, structureFromDb);
      Assert.IsTrue(structureFromDb.Rounds.Any());
    }

    public static Structure CreateStructure(UnitOfWork repository, SportKind sportKind = SportKind.Football)
    {
      var structure = repository.Structures.Create();

      structure.SportType = SportTypeTests.CreateSportType(repository, sportKind);

      structure.Rounds.Add(RoundTests.CreateRound(repository));
      structure.Rounds.Add(RoundTests.CreateRound(repository, RoundType.Playoff, 2));
      structure.Rounds.Add(RoundTests.CreateRound(repository, number: 3));

      structure.Save();

      return structure;
    }
  }
}