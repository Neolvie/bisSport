using System;
using System.Collections.Generic;
using System.Linq;
using bisSport.Domain.Core;
using bisSport.Domain.Core.Interfaces;
using bisSport.Domain.Enums;
using bisSport.Server.Helpers;
using bisSport.Server.Repository;
using bisSport.Tests.CRUD;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace bisSport.Tests.SportEvents
{
  [TestClass]
  public class Tennis : TestBase
  {
    [TestMethod]
    public void TennisEvent()
    {
      var tennisTournament = CreateTennisEvent(Repository);

      var savedTournament = Repository.SingleEvents.GetAll().FirstOrDefault(x => x.Id == tennisTournament.Id);

      Assert.IsNotNull(savedTournament);
      Assert.AreEqual(savedTournament.Participants.Count, 8);
      Assert.AreEqual(4, Repository.Matches.GetAll().Count(x => x.Event.Id == savedTournament.Id && x.Round.RoundNumber == 1));
    }

    public static SingleEvent CreateTennisEvent(UnitOfWork repository)
    {
      // Create the event.
      var random = new Random();
      var @event = repository.SingleEvents.Create();
      @event.Name = "DefaultTennisTournament";
      @event.BeginDate = DateTime.Now;
      @event.EndDate = DateTime.Now.AddDays(30);
      @event.Organizer = OrganizerTests.CreateSportClub(repository);
      @event.Structure = CreateStructure(repository);
      @event.Address = AddressTests.CreateAddress(repository);
      @event.Save();

      // Create 8 tournament players.
      for (var i = 0; i < 8; i++)
        @event.Participants.Add(PlayerTests.CreatePlayer(repository, random));

      @event.Save();

      // Create round 1 matches.
      for (var i = 0; i < 4; i++)
      {
        var firstPlayer = @event.Participants[i*2];
        var secondPlayer = @event.Participants[i*2 + 1];
        var match = repository.Matches.Create();
        match.Participants.Add(firstPlayer);
        match.Participants.Add(secondPlayer);
        match.BeginDate = DateTime.Now.AddDays(i);
        match.Area = AreaTests.CreateArea(repository);
        match.Round = @event.Structure.Rounds.FirstOrDefault(x => x.RoundNumber == 1);
        match.Event = @event;
        match.Save();
        @event.Matches.Add(match);
      }

      // Create round 2 matches.
      for (var i = 0; i < 2; i++)
      {
        var match = repository.Matches.Create();
        match.BeginDate = DateTime.Now.AddDays(i);
        match.Round = @event.Structure.Rounds.FirstOrDefault(x => x.RoundNumber == 2);
        match.Area = AreaTests.CreateArea(repository);
        match.Event = @event;
        match.Save();
        @event.Matches.Add(match);
      }

      // Create round 3 matches.
      for (var i = 0; i < 1; i++)
      {
        var match = repository.Matches.Create();
        match.BeginDate = DateTime.Now.AddDays(4);
        match.Round = @event.Structure.Rounds.FirstOrDefault(x => x.RoundNumber == 3);
        match.Area = AreaTests.CreateArea(repository);
        match.Event = @event;
        match.Save();
        @event.Matches.Add(match);
      }

      @event.Save();

      Simulate(repository, @event);

      @event.Save();

      return @event;
    }

    public static Structure CreateStructure(UnitOfWork repository)
    {
      var structure = repository.Structures.Create();
      structure.Name = "TennisDefault";
      structure.SportType = CreateSportType(repository);
      
      var rounds = new List<Round>();

      var firstRound = repository.Rounds.Create();
      firstRound.Games = 1;
      firstRound.Periods = 3;
      firstRound.MaxPeriodPoints = 7;
      // TODO: что за Points в Round?
      // firstRound.Points = new List<Point>() {new Point() {} };
      firstRound.RoundNumber = 1;
      firstRound.WinnerCount = 4;
      firstRound.Save();

      var semiFinal = repository.Rounds.Create();
      semiFinal.Games = 1;
      semiFinal.Periods = 3;
      semiFinal.MaxPeriodPoints = 7;
      // TODO: что за Points в Round?
      // firstRound.Points = new List<Point>() {new Point() {} };
      semiFinal.RoundNumber = 2;
      semiFinal.WinnerCount = 2;
      semiFinal.Save();

      var final = repository.Rounds.Create();
      final.Games = 1;
      final.Periods = 5;
      final.MaxPeriodPoints = 7;
      // TODO: что за Points в Round?
      // firstRound.Points = new List<Point>() {new Point() {} };
      final.RoundNumber = 3;
      final.WinnerCount = 1;
      final.Save();

      rounds.Add(firstRound);
      rounds.Add(semiFinal);
      rounds.Add(final);

      structure.Rounds = rounds;
      structure.Save();
      return structure;
    }

    public static SportType CreateSportType(UnitOfWork repository)
    {
      var sportType = repository.SportTypes.Create();
      sportType.SportKind = SportKind.Tennis;
      sportType.CleanTime = false;
      sportType.CleanGameDuration = null;
      sportType.GameDuration = 120;
      sportType.Save();
      return sportType;
    }

    private static void Simulate(UnitOfWork repository, SingleEvent @event)
    {
      // Simulate round 1 matches.
      var matches = repository.Matches.GetAll().Where(x => x.Event.Id == @event.Id && 
                                           x.Round.RoundNumber == 1).ToList();
      var random = new Random();
      foreach (var match in matches)
      {
        MatchTests.Simulate(repository, match, random);
      }

      // Form round 1 winners.
      var winners = new List<IParticipant>();
      foreach (var match in matches)
      {
        var winner = match.DefineWinner();
        winners.Add(winner);
      }

      matches = repository.Matches.GetAll().Where(x => x.Event.Id == @event.Id &&
                                           x.Round.RoundNumber == 2).ToList();
      for (var i = 0; i < matches.Count(); i++)
      {
        matches[i].Participants.Add(winners[i * 2]);
        matches[i].Participants.Add(winners[i * 2 + 1]);
      }

      matches.SaveAll();

      // Simulate round 2 matches.
      foreach (var match in matches)
      {
        MatchTests.Simulate(repository, match, random);
      }

      // Form round 3 winners.
      //winners = new List<IParticipant>();
      //foreach (var match in matches)
      //{
      //  var winner = match.DefineWinner();
      //  winners.Add(winner);
      //}
    }
  }
}
