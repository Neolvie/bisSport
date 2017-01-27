using System;
using System.Collections.Generic;
using System.Linq;
using bisSport.Domain.Core;
using bisSport.Server.Helpers;
using bisSport.Server.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate.Util;
using Remotion.Linq.Clauses.ResultOperators;

namespace bisSport.Tests.CRUD
{
  [TestClass]
  public class TeamTests : TestBase
  {
    [TestMethod]
    public void TeamCreate()
    {
      var team = CreateTeam(Repository);
      var savedTeam = Repository.Teams.GetAll().FirstOrDefault(x => x.Id == team.Id);
      Assert.AreEqual(team, savedTeam);
    }

    [TestMethod]
    public void TeamReSave()
    {
      var team = CreateTeam(Repository);

      var players = new List<Player>()
      {
        PlayerTests.CreatePlayer(Repository),
        PlayerTests.CreatePlayer(Repository),
        PlayerTests.CreatePlayer(Repository)
      };

      foreach (var player in players)
      {
        team.Players.Add(player);
      }

      team.Save();

      var savedTeam = Repository.Teams.GetAll().FirstOrDefault(x => x.Id == team.Id);
      savedTeam.Save();

      Assert.AreEqual(team, savedTeam);
    }

    [TestMethod]
    public void TeamAddParticipants()
    {
      var players = new List<Player>()
      {
        PlayerTests.CreatePlayer(Repository),
        PlayerTests.CreatePlayer(Repository),
        PlayerTests.CreatePlayer(Repository)
      };
      var team = CreateTeam(Repository);

      var playerId = players[0].Id;  

      foreach (var player in players)
      {
        team.Players.Add(player);
      }

      team.Save();

      Repository.Session.Clear();

      var savedTeam = Repository.Teams.GetAll().SingleOrDefault(x => x.Id == team.Id);
      var savedPlayer = Repository.Players.GetAll().SingleOrDefault(x => x.Id == playerId);

      Assert.IsNotNull(savedTeam);
      Assert.IsNotNull(savedPlayer);
      Assert.IsNotNull(savedPlayer.Team);
      Assert.IsNotNull(savedTeam.Players);

      savedPlayer.Team = null;
      savedPlayer.Save();
      
      Repository.Session.Clear();

      savedTeam = Repository.Teams.GetAll().Single(x => x.Id == team.Id);

      Assert.IsTrue(!savedTeam.Players.Contains(savedPlayer));

      var deletedPlayer = savedTeam.Players.First();
      var deletedPlayerId = deletedPlayer.Id;

      savedTeam.Players.Remove(deletedPlayer);
      savedTeam.Save();

      Repository.Session.Clear();

      var deletedPlayerFromDb = Repository.Players.GetAll().Single(x => x.Id == deletedPlayerId);

      Assert.IsNull(deletedPlayerFromDb.Team);

      deletedPlayerFromDb.Team = savedTeam;

      deletedPlayerFromDb.Save();

      savedTeam = Repository.Teams.GetAll().Single(x => x.Id == team.Id);

      Assert.IsTrue(savedTeam.Players.Contains(deletedPlayerFromDb));
    }

    [TestMethod]
    public void TeamDeleteParticipants()
    {
      var players = new List<Player>()
      {
        PlayerTests.CreatePlayer(Repository),
        PlayerTests.CreatePlayer(Repository),
        PlayerTests.CreatePlayer(Repository)
      };
      var team = CreateTeam(Repository);
      team.Save();
      foreach (var player in players)
      {
        player.Team = team;
        player.Save();
      }
      
      Repository.Session.Clear();

      // Delete participant.
      var savedTeam = Repository.Teams.GetAll().FirstOrDefault(x => x.Id == team.Id);
      var deletedPlayer = players.First();
      deletedPlayer.Team = null;
      deletedPlayer.Save();

      Repository.Session.Clear();

      savedTeam = Repository.Teams.GetAll().FirstOrDefault(x => x.Id == team.Id);
      var participants = savedTeam.Players;
      Assert.AreEqual(participants.Count, players.Count - 1);
      Assert.IsNull(savedTeam.Players.FirstOrDefault(p => p.Equals(deletedPlayer)));
    }

    public static Team CreateTeam(UnitOfWork repository)
    {
      var team = repository.Teams.Create();
      var random = new Random();
      team.Name = $"Команда {random.Next(1, 99)}";
      team.Save();
      return team;
    }
  }
}
