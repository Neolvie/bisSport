using System;
using System.Linq;
using bisSport.Domain.Core;
using bisSport.Server.Helpers;
using bisSport.Server.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace bisSport.Tests.CRUD
{
  [TestClass]
  public class EventsTests : TestBase
  {
    [TestMethod]
    public void SingleEventCreate()
    {
      var @event = CreateSingleEvent(Repository);
      var eventId = @event.Id;

      Repository.Session.Clear();

      @event = Repository.SingleEvents.GetAll().FirstOrDefault(x => x.Id == eventId);

      Assert.IsNotNull(@event);
      Assert.AreEqual(@event, @event);
      Assert.IsTrue(@event.Matches.Any());
      Assert.IsTrue(@event.Participants.Any());
      Assert.IsTrue(@event.Matches[0].Participants.Any());
      Assert.IsNotNull(@event.Matches[0].Event);
      Assert.IsNotNull(@event.Address);
      Assert.IsNotNull(@event.Organizer);

      var sportClub = @event.Organizer.As<SportClub>();
      Assert.IsNotNull(sportClub);
      Assert.IsTrue(sportClub.SportKinds.Any());
    }

    [TestMethod]
    public void EventDelete()
    {
      var @event = CreateSingleEvent(Repository);

      var eventId = @event.Id;
      var playerId = @event.Participants[0].Id;
      var addressId = @event.Address.Id;
      var sportClubId = @event.Organizer.Id;

      @event.Delete();

      var eventFromDb = Repository.SingleEvents.GetAll().FirstOrDefault(x => x.Id == eventId);
      var hasMatches = Repository.Matches.GetAll().Any(x => x.Event.Id == eventId);
      var player = Repository.Players.GetAll().FirstOrDefault(x => x.Id == playerId);
      var address = Repository.Addresses.GetAll().FirstOrDefault(x => x.Id == addressId);
      var sportClub = Repository.SportClubs.GetAll().FirstOrDefault(x => x.Id == sportClubId);

      Assert.IsNull(eventFromDb);
      Assert.IsFalse(hasMatches);
      Assert.IsNotNull(player);
      Assert.IsNull(address);

      Assert.IsNotNull(sportClub);
      Assert.IsTrue(sportClub.SportKinds.Any());
    }

    [TestMethod]
    public void MultiEventCreate()
    {
      var multiEvent = CreateMultiEvent(Repository);
      var singleEvent = CreateSingleEvent(Repository);

      singleEvent.MainEvent = multiEvent;

      singleEvent.Save();

      Repository.Session.Clear();

      var multiEventFromDb = Repository.MultiEvents.GetAll().FirstOrDefault(x => x.Id == multiEvent.Id);

      Assert.IsNotNull(multiEventFromDb);
      Assert.AreNotSame(multiEventFromDb, multiEvent);
      Assert.IsTrue(multiEventFromDb.Events.Any());
      Assert.IsNotNull((multiEventFromDb.Events[0] as SingleEvent)?.MainEvent);
    }

    public static SingleEvent CreateSingleEvent(UnitOfWork repository)
    {
      var @event = repository.SingleEvents.Create();
      
      var random = new Random();

      @event.Name = $"Тестовое соревнование {random.Next(20, 90)}";
      @event.BeginDate = DateTime.Now;
      @event.EndDate = DateTime.Now.AddDays(3);

      @event.Address = AddressTests.CreateAddress(repository);
      @event.Organizer = OrganizerTests.CreateSportClub(repository);
      @event.Structure = StructureTests.CreateStructure(repository);

      @event.Save();

      for (int i = 0; i < 5; i++)
        @event.Participants.Add(PlayerTests.CreatePlayer(repository));

      for (int i = 0; i < 3; i++)
      {
        var match = MatchTests.CreateMatch(repository, @event, @event.Participants.ToList());
        @event.Matches.Add(match);
      }      

      @event.Save();

      return @event;
    }

    public static MultiEvent CreateMultiEvent(UnitOfWork repository)
    {
      var multiEvent = repository.MultiEvents.Create();

      for (int i = 0; i < 5; i++)
        multiEvent.Participants.Add(PlayerTests.CreatePlayer(repository));

      multiEvent.Address = AddressTests.CreateAddress(repository);
      multiEvent.BeginDate = DateTime.Now;
      multiEvent.EndDate = DateTime.Today.AddDays(10);
      multiEvent.Name = "Чемпионат";
      multiEvent.Organizer = OrganizerTests.CreateSportClub(repository);

      multiEvent.Save();

      return multiEvent;
    }
  }
}