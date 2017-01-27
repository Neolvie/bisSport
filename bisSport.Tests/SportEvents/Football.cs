using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using bisSport.Domain.Core;
using bisSport.Domain.Core.Interfaces;
using bisSport.Domain.Enums;
using bisSport.Server.Helpers;
using bisSport.Server.Mappings;
using bisSport.Server.Repository;
using bisSport.Tests.CRUD;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace bisSport.Tests.SportEvents
{
  [TestClass]
  public class Football : TestBase
  {
    [TestMethod]
    public void FootballEvent()
    {
      var stopwatch = Stopwatch.StartNew();

      var footballEvent = CreateFootbalEvent(Repository);
      var random = new Random(Environment.TickCount * DateTime.Now.Millisecond);

      Repository.Session.Clear();

      var footballEventFromDb = Repository.SingleEvents.GetAll().FirstOrDefault(x => x.Id == footballEvent.Id);

      Assert.AreNotSame(footballEventFromDb, footballEvent);
      Assert.IsNotNull(footballEventFromDb);

      var rounds = footballEventFromDb.Structure.Rounds;
      var teams = new List<IParticipant>();

      foreach (var round in rounds)
      {
        ProcessRound(Repository, footballEventFromDb, round, random);

        teams = DefineWinners(footballEventFromDb, round, random);

        Assert.IsTrue(footballEventFromDb.Matches.Any());
        Assert.AreEqual(teams.Count, footballEventFromDb.Participants.Count / Math.Pow(2, round.RoundNumber));
      }

      Assert.AreEqual(teams.Count, 1);

      footballEventFromDb.Save();

      //footballEventFromDb.Delete();

      //footballEventFromDb = SingleEvents.GetAll().FirstOrDefault(x => x.Id == footballEventFromDb.Id);

      //Assert.IsNull(footballEventFromDb);

      stopwatch.Stop();
    }

    public static void ProcessRound(UnitOfWork repository, SingleEvent @event, Round round, Random random)
    {
      switch (round.Type)
      {
        case RoundType.Playoff:
          ProcessPlayOffRound(repository, @event, round, random);
          break;
        case RoundType.AllPlayAll:
          ProcessAllPlayAllRound(repository, @event, round, random);
          break;
      }
    }

    public static void ProcessPlayOffRound(UnitOfWork repository, SingleEvent @event, Round round, Random random)
    {
      var teams = DefineWinners(@event, @event.Structure.Rounds.SingleOrDefault(x => x.RoundNumber == round.RoundNumber - 1), random);

      var matchCount = teams.Count / 2;
      var countInGroup = 2;

      for (var i = 0; i < matchCount; i++)
      {
        var participants = Tossing(teams.ToList(), countInGroup, random);

        foreach (var participant in participants)
          teams.Remove(participant);

        Match prevMatch = null;

        for (var j = 1; j <= round.Games; j++)
        {
          var match = repository.Matches.Create();
          match.Participants = participants;

          if (j % 2 == 0)
            match.Participants = match.Participants.Reverse().ToList();

          match.Round = round;
          match.Game = j;
          match.Area = AreaTests.CreateArea(repository);
          match.Area.Name = $"Стадион команды {match.Participants[0].Name}";
          match.Name = $"{match.Participants[0].Name} - {match.Participants[1].Name}";
          match.BeginDate = DateTime.Now;
          match.Event = @event;
          match.Result = GenerateMatchResult(repository, @event, match, random);
          match.PrevMatch = prevMatch;
          prevMatch = match;
          @event.Matches.Add(match);
        }
      }
    }

    public static void ProcessAllPlayAllRound(UnitOfWork repository, SingleEvent @event, Round round, Random random)
    {
      var groupNames = new[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P" };
      var teams = DefineWinners(@event, @event.Structure.Rounds.SingleOrDefault(x => x.RoundNumber == round.RoundNumber - 1), random);

      var groups = teams.Count / round.ParticipantsPerGroup;

      for (var i = 0; i < groups; i++)
      {
        var group = repository.Groups.Create();
        group.Name = $"Группа {groupNames[i]}";
        round.Groups.Add(group);

        group.Participants = Tossing(teams.ToList(), round.ParticipantsPerGroup.Value, random);

        foreach (var participant in group.Participants)
          teams.Remove(participant);

        for (var team1Num = 0; team1Num < group.Participants.Count - 1; ++team1Num)
        {
          for (int team2Num = team1Num + 1; team2Num < group.Participants.Count; team2Num++)
          {
            Match prevMatch = null;

            for (var j = 1; j <= round.Games; j++)
            {
              var match = repository.Matches.Create();

              match.Participants.Add(group.Participants[team1Num]);
              match.Participants.Add(group.Participants[team2Num]);

              if (j % 2 == 0)
                match.Participants = match.Participants.Reverse().ToList();

              match.Round = round;
              match.Game = j;
              match.Area = AreaTests.CreateArea(repository);
              match.Area.Name = $"Стадион команды {match.Participants[0].Name}";
              match.Name = $"{match.Participants[0].Name} - {match.Participants[1].Name}";
              match.BeginDate = DateTime.Now;
              match.Event = @event;
              match.Result = GenerateMatchResult(repository, @event, match, random);
              match.PrevMatch = prevMatch;
              prevMatch = match;
              @event.Matches.Add(match);
            }
          }
        }

      }
    }

    public static List<IParticipant> DefineWinners(SingleEvent @event, Round round, Random random)
    {
      if (round == null)
        return @event.Participants.ToList();

      var winners = new List<IParticipant>();

      switch (round.Type)
      {
        case RoundType.AllPlayAll:
          winners = DefineAllPlayAllWinners(@event, round);
          break;
        case RoundType.Playoff:
          winners = DefinePlayOffWinners(@event, round, random);
          break;
      }

      return winners;
    }

    private static List<IParticipant> DefineAllPlayAllWinners(SingleEvent @event, Round round)
    {
      var winners = new List<IParticipant>();

      foreach (var @group in round.Groups)
      {
        var results = new Dictionary<IParticipant, double>();

        foreach (var team in @group.Participants)
        {
          var matches = @event.Matches
            .Where(x => x.Round.Id == round.Id)
            .Where(x => x.Participants.Any(s => s.Id == team.Id)).ToList();

          var points = matches
            .Select(match => match.Result.Scores.Single(x => x.Participant.Id == team.Id))
            .Select(result => round.Points.Single(x => x.GameResult == result.Point.GameResult).Count).Sum();

          results.Add(team, points);
        }

        winners.AddRange(results.OrderByDescending(x => x.Value).Select(x => x.Key).Take(2));
      }

      return winners;
    }

    private static List<IParticipant> DefinePlayOffWinners(SingleEvent @event, Round round, Random random)
    {
      var winners = new List<IParticipant>();

      var matches = @event.Matches.Where(x => x.Round.Id == round.Id && x.Game == round.Games).ToList();

      foreach (var match in matches)
      {
        var team1 = match.Participants[0];
        var team2 = match.Participants[1];
        var team1Count = 0.0;
        var team2Count = 0.0;
        var curMatch = match;

        while (curMatch != null)
        {
          if (curMatch.Result == null)
            break;

          team1Count += curMatch.Result.Scores.Single(x => x.Participant == team1).Point.Count;
          team2Count += curMatch.Result.Scores.Single(x => x.Participant == team2).Point.Count;
          curMatch = curMatch.PrevMatch;
        }

        if (team1Count > team2Count)
          winners.Add(team1);
        else if (team1Count < team2Count)
          winners.Add(team2);
        else
          winners.Add(match.Participants[random.Next(0, 2)]);
      }

      return winners;
    }

    public static Result GenerateMatchResult(UnitOfWork repository, SingleEvent @event, Match match, Random random)
    {
      Result result = null;

      var gameResult = (GameResult)random.Next(0, 3);

      switch (gameResult)
      {
        case GameResult.Draw:
          result = GenerateDrawResult(repository, @event, match, random);
          break;
        case GameResult.Win:
          result = GenerateWinResult(repository, @event, match, random);
          break;
        case GameResult.Lose:
          result = GenerateLoseResult(repository, @event, match, random);
          break;
      }

      result.Name = $"Результат матча {match.Name}";

      return result;
    }

    private static Result GenerateLoseResult(UnitOfWork repository, SingleEvent @event, Match match, Random random)
    {
      var result = repository.Results.Create();

      var countWin = random.Next(1, 4);
      var countLose = random.Next(0, countWin);

      var firstTeamScore = repository.Scores.Create();
      firstTeamScore.Name = $"Поражение {countLose} - {countWin}";
      firstTeamScore.Participant = match.Participants[0];
      firstTeamScore.Point = repository.Points.Create();
      firstTeamScore.Point.GameResult = GameResult.Lose;
      firstTeamScore.Point.Count = countLose;

      GenerateScorers(repository, @event, random, countLose, firstTeamScore);

      var secondTeamScore = repository.Scores.Create();
      secondTeamScore.Name = $"Победа {countLose} - {countWin}";
      secondTeamScore.Participant = match.Participants[1];
      secondTeamScore.Point = repository.Points.Create();
      secondTeamScore.Point.GameResult = GameResult.Win;
      secondTeamScore.Point.Count = countWin;

      GenerateScorers(repository, @event, random, countWin, secondTeamScore);

      result.Scores.Add(firstTeamScore);
      result.Scores.Add(secondTeamScore);

      return result;
    }

    private static void GenerateScorers(UnitOfWork repository, SingleEvent @event, Random random, int count, Score teamScore)
    {
      for (var i = 1; i <= count; ++i)
      {
        var scorer = repository.Scorers.Create();
        var team = (teamScore.Participant as Team);
        scorer.Participant = team.Players[random.Next(0, team.Players.Count)];
        scorer.Time = new TimeSpan(0,0, random.Next(1, @event.Structure.SportType.GameDuration.Value), random.Next(60));
        scorer.Name = $"{scorer.Participant}, {scorer.Time} минута";
        teamScore.Point.Scorers.Add(scorer);
      }
    }

    private static Result GenerateWinResult(UnitOfWork repository, SingleEvent @event, Match match, Random random)
    {
      var result = repository.Results.Create();

      var countWin = random.Next(1, 4);
      var countLose = random.Next(0, countWin);

      var firstTeamScore = repository.Scores.Create();
      firstTeamScore.Name = $"Победа {countWin} - {countLose}";
      firstTeamScore.Participant = match.Participants[0];
      firstTeamScore.Point = repository.Points.Create();
      firstTeamScore.Point.GameResult = GameResult.Win;
      firstTeamScore.Point.Count = countWin;

      GenerateScorers(repository, @event, random, countWin, firstTeamScore);

      var secondTeamScore = repository.Scores.Create();
      secondTeamScore.Name = $"Поражение {countWin} - {countLose}";
      secondTeamScore.Participant = match.Participants[1];
      secondTeamScore.Point = repository.Points.Create();
      secondTeamScore.Point.GameResult = GameResult.Lose;
      secondTeamScore.Point.Count = countLose;

      GenerateScorers(repository, @event, random, countLose, secondTeamScore);

      result.Scores.Add(firstTeamScore);
      result.Scores.Add(secondTeamScore);

      return result;
    }

    private static Result GenerateDrawResult(UnitOfWork repository, SingleEvent @event, Match match, Random random)
    {
      var result = repository.Results.Create();

      var count = random.Next(0, 4);

      var firstTeamScore = repository.Scores.Create();
      firstTeamScore.Name = $"Ничья {count} - {count}";
      firstTeamScore.Participant = match.Participants[0];
      firstTeamScore.Point = repository.Points.Create();
      firstTeamScore.Point.GameResult = GameResult.Draw;
      firstTeamScore.Point.Count = count;

      GenerateScorers(repository, @event, random, count, firstTeamScore);

      var secondTeamScore = repository.Scores.Create();
      secondTeamScore.Name = $"Ничья {count} - {count}";
      secondTeamScore.Participant = match.Participants[1];
      secondTeamScore.Point = repository.Points.Create();
      secondTeamScore.Point.GameResult = GameResult.Draw;
      secondTeamScore.Point.Count = count;

      GenerateScorers(repository, @event, random, count, secondTeamScore);

      result.Scores.Add(firstTeamScore);
      result.Scores.Add(secondTeamScore);

      return result;
    }

    public static List<IParticipant> Tossing(List<IParticipant> participants, int countInGroup, Random random)
    {
      var tossParticipants = new List<IParticipant>();

      if (!participants.Any())
        return tossParticipants;

      for (var i = 0; i < countInGroup; ++i)
      {
        var team = participants[random.Next(0, participants.Count)];
        tossParticipants.Add(team);
        participants.Remove(team);
      }

      return tossParticipants;
    }

    public static SingleEvent CreateFootbalEvent(UnitOfWork repository)
    {
      var footballEvent = repository.SingleEvents.Create();

      var random = new Random(Environment.TickCount * DateTime.Now.Millisecond);

      footballEvent.Address = AddressTests.CreateAddress(repository);
      footballEvent.Organizer = OrganizerTests.CreateSportClub(repository);
      footballEvent.BeginDate = DateTime.Now;
      footballEvent.EndDate = DateTime.Now.AddMonths(9);
      footballEvent.Name = $"Лига Чемпионов UEFA {DateTime.Now.Year}";
      footballEvent.Structure = CreateUefaClStructure(repository);
      footballEvent.Participants =
        CreateTeams(repository, footballEvent.Structure.Rounds.FirstOrDefault(x => x.RoundNumber == 1).MaxParticipants.Value, random);

      footballEvent.Save();

      return footballEvent;
    }

    public static Structure CreateUefaClStructure(UnitOfWork repository)
    {
      var structure = repository.Structures.Create();

      structure.Name = "Лига Чемпионов UEFA";
      structure.SportType = CreateFootballSportType(repository);

      structure.Rounds = CreateUefaClRounds(repository);

      structure.Save();

      return structure;
    }

    public static List<IParticipant> CreateTeams(UnitOfWork repository, int count, Random random)
    {
      var teams = new List<IParticipant>();

      for (var i = 0; i < count; i++)
      {
        var name = TeamNames[random.Next(0, TeamNames.Count)];
        var team = repository.Teams.Create();
        team.Name = name;
        team.Players = CreatePlayers(repository, team, 11, random);

        teams.Add(team);
      }

      return teams;
    }

    public static List<Player> CreatePlayers(UnitOfWork repository, Team team, int count, Random random)
    {
      var players = new List<Player>();

      for (var i = 0; i < count; i++)
      {
        var name = PlayerTests.PlayerNames[random.Next(0, PlayerTests.PlayerNames.Count)];
        var player = repository.Players.Create();
        player.Team = team;
        player.Name = name;
        players.Add(player);
      }

      return players;
    }

    public static List<Round> CreateUefaClRounds(UnitOfWork repository)
    {
      var rounds = new List<Round>();

      // Квалификации
      var firstQualificationRound = repository.Rounds.Create();
      firstQualificationRound.Name = "Квалификационный раунд 1";
      firstQualificationRound.MinParticipants = 130;
      firstQualificationRound.MaxParticipants = 256;
      firstQualificationRound.Type = RoundType.Playoff;
      firstQualificationRound.Games = 2;
      firstQualificationRound.RoundNumber = 1;
      firstQualificationRound.WinnerCount = 1;
      firstQualificationRound.Periods = 2;
      rounds.Add(firstQualificationRound);

      var secondQualificationRound = repository.Rounds.Create();
      secondQualificationRound.Name = "Квалификационный раунд 2";
      secondQualificationRound.MinParticipants = 66;
      secondQualificationRound.MaxParticipants = 128;
      secondQualificationRound.Type = RoundType.Playoff;
      secondQualificationRound.Games = 2;
      secondQualificationRound.RoundNumber = 2;
      secondQualificationRound.WinnerCount = 1;
      secondQualificationRound.Periods = 2;
      rounds.Add(secondQualificationRound);

      var thirdQualificationRound = repository.Rounds.Create();
      thirdQualificationRound.Name = "Квалификационный раунд 3";
      thirdQualificationRound.MinParticipants = 34;
      thirdQualificationRound.MaxParticipants = 64;
      thirdQualificationRound.Type = RoundType.Playoff;
      thirdQualificationRound.Games = 2;
      thirdQualificationRound.RoundNumber = 3;
      thirdQualificationRound.WinnerCount = 1;
      thirdQualificationRound.Periods = 2;
      rounds.Add(thirdQualificationRound);

      // Групповой этап
      var groupRound = repository.Rounds.Create();
      groupRound.Name = "Групповой этап";
      groupRound.MinParticipants = 32;
      groupRound.MaxParticipants = 32;
      groupRound.Type = RoundType.AllPlayAll;
      groupRound.Games = 2;
      groupRound.RoundNumber = 4;
      groupRound.WinnerCount = 2;
      groupRound.Periods = 2;
      groupRound.ParticipantsPerGroup = 4;
      groupRound.Points = CreateDefaultFootballPoints(repository);
      rounds.Add(groupRound);

      // 1/8
      var playOffRound18 = repository.Rounds.Create();
      playOffRound18.Name = "1/8 финала";
      playOffRound18.MinParticipants = 16;
      playOffRound18.MaxParticipants = 16;
      playOffRound18.Type = RoundType.Playoff;
      playOffRound18.Games = 2;
      playOffRound18.RoundNumber = 5;
      playOffRound18.WinnerCount = 1;
      playOffRound18.Periods = 2;
      rounds.Add(playOffRound18);

      // 1/4
      var playOffRound14 = repository.Rounds.Create();
      playOffRound14.Name = "1/4 финала";
      playOffRound14.MinParticipants = 8;
      playOffRound14.MaxParticipants = 8;
      playOffRound14.Type = RoundType.Playoff;
      playOffRound14.Games = 2;
      playOffRound14.RoundNumber = 6;
      playOffRound14.WinnerCount = 1;
      playOffRound14.Periods = 2;
      rounds.Add(playOffRound14);

      // 1/2
      var playOffRound12 = repository.Rounds.Create();
      playOffRound12.Name = "Полуфинал";
      playOffRound12.MinParticipants = 4;
      playOffRound12.MaxParticipants = 4;
      playOffRound12.Type = RoundType.Playoff;
      playOffRound12.Games = 2;
      playOffRound12.RoundNumber = 7;
      playOffRound12.WinnerCount = 1;
      playOffRound12.Periods = 2;
      rounds.Add(playOffRound12);

      // Финал
      var final = repository.Rounds.Create();
      final.Name = "Финал";
      final.MinParticipants = 2;
      final.MaxParticipants = 2;
      final.Type = RoundType.Playoff;
      final.Games = 1;
      final.RoundNumber = 8;
      final.WinnerCount = 1;
      final.Periods = 2;
      rounds.Add(final);

      return rounds;
    }

    public static List<Point> CreateDefaultFootballPoints(UnitOfWork repository)
    {
      var points = new List<Point>();

      var winPoint = repository.Points.Create();
      winPoint.GameResult = GameResult.Win; ;
      winPoint.Count = 3;
      winPoint.Name = "Победа";

      var drawPoint = repository.Points.Create();
      drawPoint.GameResult = GameResult.Draw;
      drawPoint.Count = 1;
      drawPoint.Name = "Ничья";

      var losePoint = repository.Points.Create();
      losePoint.GameResult = GameResult.Lose;
      losePoint.Count = 0;
      losePoint.Name = "Поражение";

      points.Add(winPoint);
      points.Add(drawPoint);
      points.Add(losePoint);

      return points;
    }

    public static SportType CreateFootballSportType(UnitOfWork repository)
    {
      var sportType = repository.SportTypes.Create();

      sportType.SportKind = SportKind.Football;
      sportType.CleanGameDuration = null;
      sportType.CleanTime = false;
      sportType.GameDuration = 90;
      sportType.Name = "Футбол";

      return sportType;
    }

    #region Названия клубов

    public static List<string> TeamNames = new List<string>()
    {
      "АНДЕРЛЕХТ",
      "АПОЭЛ",
      "АРСЕНАЛ",
      "АТЛЕТИКО",
      "АУСТРИЯ В",
      "АЯКС",
      "БАВАРИЯ",
      "БАЗЕЛЬ",
      "БАЙЕР",
      "БАРСЕЛОНА",
      "БАТЭ",
      "БЕНФИКА",
      "БИРКИРКАРА",
      "БОРУССИЯ Д",
      "ВАРДАР",
      "ВИКТОРИЯ ПЛ",
      "ГАЛАТАСАРАЙ",
      "ГРАССХОППЕР",
      "ДАУГАВА Д",
      "ДИНАМО З",
      "ДИНАМО ТБ",
      "ДЬЕР",
      "ЖЕЛЕЗНИЧАР",
      "ЗАЛЬЦБУРГ",
      "ЗЕНИТ",
      "ЗЮЛТЕ-ВАРЕГЕМ",
      "КЛИФТОНВИЛЛ",
      "КОПЕНГАГЕН",
      "ЛЕГИЯ",
      "ЛИОН",
      "ЛУДОГОРЕЦ",
      "ЛУСИТАНС",
      "МАККАБИ ТЕЛЬ-АВИВ",
      "МАНЧЕСТЕР СИТИ",
      "МАНЧЕСТЕР ЮНАЙТЕД",
      "МАРИБОР",
      "МАРСЕЛЬ",
      "МЕТАЛЛИСТ",
      "МИЛАН",
      "МОЛЬДЕ",
      "НАПОЛИ",
      "НЕФТЧИ",
      "НЫММЕ КАЛЬЮ",
      "ОЛИМПИАКОС П",
      "ПАОК",
      "ПАРТИЗАН",
      "ПАСУШ ДЕ ФЕРРЕЙРА",
      "ПОРТУ",
      "ПСВ",
      "ПСЖ",
      "РЕАЛ",
      "РЕАЛ СОСЬЕДАД",
      "СЕЛТИК",
      "СКЕНДЕРБЕУ",
      "СЛАЙГО РОВЕРС",
      "СЛОВАН БР",
      "СТРЕЙМЮР",
      "СТЯУА",
      "СУТЬЕСКА",
      "ТНС",
      "ТРЕ ПЕННЕ",
      "ФЕНЕРБАХЧЕ",
      "ФОЛА",
      "ХАБНАРФЬЕРДЮР",
      "ХИК",
      "ЦСКА",
      "ЧЕЛСИ",
      "ШАЛЬКЕ-04",
      "ШАХТЕР Д",
      "ШАХТЕР КГ",
      "ШЕРИФ",
      "ШИРАК",
      "ЭКРАНАС",
      "ЭЛЬФСБОРГ",
      "ЮВЕНТУС",
      "АНДЕРЛЕХТ (U21)",
      "АПОЭЛ (U21)",
      "АРСЕНАЛ (U21)",
      "АТЛЕТИКО (U21)",
      "АУСТРИЯ В (U21)",
      "АЯКС (U21)",
      "БАВАРИЯ (U21)",
      "БАЗЕЛЬ (U21)",
      "БАЙЕР (U21)",
      "БАРСЕЛОНА (U21)",
      "БАТЭ (U21)",
      "БЕНФИКА (U21)",
      "БИРКИРКАРА (U21)",
      "БОРУССИЯ Д (U21)",
      "ВАРДАР (U21)",
      "ВИКТОРИЯ ПЛ (U21)",
      "ГАЛАТАСАРАЙ (U21)",
      "ГРАССХОППЕР (U21)",
      "ДАУГАВА Д (U21)",
      "ДИНАМО З (U21)",
      "ДИНАМО ТБ (U21)",
      "ДЬЕР (U21)",
      "ЖЕЛЕЗНИЧАР (U21)",
      "ЗАЛЬЦБУРГ (U21)",
      "ЗЕНИТ (U21)",
      "ЗЮЛТЕ-ВАРЕГЕМ (U21)",
      "КЛИФТОНВИЛЛ (U21)",
      "КОПЕНГАГЕН (U21)",
      "ЛЕГИЯ (U21)",
      "ЛИОН (U21)",
      "ЛУДОГОРЕЦ (U21)",
      "ЛУСИТАНС (U21)",
      "МАККАБИ ТЕЛЬ-АВИВ (U21)",
      "МАНЧЕСТЕР СИТИ (U21)",
      "МАНЧЕСТЕР ЮНАЙТЕД (U21)",
      "МАРИБОР (U21)",
      "МАРСЕЛЬ (U21)",
      "МЕТАЛЛИСТ (U21)",
      "МИЛАН (U21)",
      "МОЛЬДЕ (U21)",
      "НАПОЛИ (U21)",
      "НЕФТЧИ (U21)",
      "НЫММЕ КАЛЬЮ (U21)",
      "ОЛИМПИАКОС П (U21)",
      "ПАОК (U21)",
      "ПАРТИЗАН (U21)",
      "ПАСУШ ДЕ ФЕРРЕЙРА (U21)",
      "ПОРТУ (U21)",
      "ПСВ (U21)",
      "ПСЖ (U21)",
      "РЕАЛ (U21)",
      "РЕАЛ СОСЬЕДАД (U21)",
      "СЕЛТИК (U21)",
      "СКЕНДЕРБЕУ (U21)",
      "СЛАЙГО РОВЕРС (U21)",
      "СЛОВАН БР (U21)",
      "СТРЕЙМЮР (U21)",
      "СТЯУА (U21)",
      "СУТЬЕСКА (U21)",
      "ТНС (U21)",
      "ТРЕ ПЕННЕ (U21)",
      "ФЕНЕРБАХЧЕ (U21)",
      "ФОЛА (U21)",
      "ХАБНАРФЬЕРДЮР (U21)",
      "ХИК (U21)",
      "ЦСКА (U21)",
      "ЧЕЛСИ (U21)",
      "ШАЛЬКЕ-04 (U21)",
      "ШАХТЕР Д (U21)",
      "ШАХТЕР КГ (U21)",
      "ШЕРИФ (U21)",
      "ШИРАК (U21)",
      "ЭКРАНАС (U21)",
      "ЭЛЬФСБОРГ (U21)",
      "ЮВЕНТУС (U21)",
      "АНДЕРЛЕХТ (U18)",
      "АПОЭЛ (U18)",
      "АРСЕНАЛ (U18)",
      "АТЛЕТИКО (U18)",
      "АУСТРИЯ В (U18)",
      "АЯКС (U18)",
      "БАВАРИЯ (U18)",
      "БАЗЕЛЬ (U18)",
      "БАЙЕР (U18)",
      "БАРСЕЛОНА (U18)",
      "БАТЭ (U18)",
      "БЕНФИКА (U18)",
      "БИРКИРКАРА (U18)",
      "БОРУССИЯ Д (U18)",
      "ВАРДАР (U18)",
      "ВИКТОРИЯ ПЛ (U18)",
      "ГАЛАТАСАРАЙ (U18)",
      "ГРАССХОППЕР (U18)",
      "ДАУГАВА Д (U18)",
      "ДИНАМО З (U18)",
      "ДИНАМО ТБ (U18)",
      "ДЬЕР (U18)",
      "ЖЕЛЕЗНИЧАР (U18)",
      "ЗАЛЬЦБУРГ (U18)",
      "ЗЕНИТ (U18)",
      "ЗЮЛТЕ-ВАРЕГЕМ (U18)",
      "КЛИФТОНВИЛЛ (U18)",
      "КОПЕНГАГЕН (U18)",
      "ЛЕГИЯ (U18)",
      "ЛИОН (U18)",
      "ЛУДОГОРЕЦ (U18)",
      "ЛУСИТАНС (U18)",
      "МАККАБИ ТЕЛЬ-АВИВ (U18)",
      "МАНЧЕСТЕР СИТИ (U18)",
      "МАНЧЕСТЕР ЮНАЙТЕД (U18)",
      "МАРИБОР (U18)",
      "МАРСЕЛЬ (U18)",
      "МЕТАЛЛИСТ (U18)",
      "МИЛАН (U18)",
      "МОЛЬДЕ (U18)",
      "НАПОЛИ (U18)",
      "НЕФТЧИ (U18)",
      "НЫММЕ КАЛЬЮ (U18)",
      "ОЛИМПИАКОС П (U18)",
      "ПАОК (U18)",
      "ПАРТИЗАН (U18)",
      "ПАСУШ ДЕ ФЕРРЕЙРА (U18)",
      "ПОРТУ (U18)",
      "ПСВ (U18)",
      "ПСЖ (U18)",
      "РЕАЛ (U18)",
      "РЕАЛ СОСЬЕДАД (U18)",
      "СЕЛТИК (U18)",
      "СКЕНДЕРБЕУ (U18)",
      "СЛАЙГО РОВЕРС (U18)",
      "СЛОВАН БР (U18)",
      "СТРЕЙМЮР (U18)",
      "СТЯУА (U18)",
      "СУТЬЕСКА (U18)",
      "ТНС (U18)",
      "ТРЕ ПЕННЕ (U18)",
      "ФЕНЕРБАХЧЕ (U18)",
      "ФОЛА (U18)",
      "ХАБНАРФЬЕРДЮР (U18)",
      "ХИК (U18)",
      "ЦСКА (U18)",
      "ЧЕЛСИ (U18)",
      "ШАЛЬКЕ-04 (U18)",
      "ШАХТЕР Д (U18)",
      "ШАХТЕР КГ (U18)",
      "ШЕРИФ (U18)",
      "ШИРАК (U18)",
      "ЭКРАНАС (U18)",
      "ЭЛЬФСБОРГ (U18)",
      "ЮВЕНТУС (U18)",
      "АНДЕРЛЕХТ (мол)",
      "АПОЭЛ (мол)",
      "АРСЕНАЛ (мол)",
      "АТЛЕТИКО (мол)",
      "АУСТРИЯ В (мол)",
      "АЯКС (мол)",
      "БАВАРИЯ (мол)",
      "БАЗЕЛЬ (мол)",
      "БАЙЕР (мол)",
      "БАРСЕЛОНА (мол)",
      "БАТЭ (мол)",
      "БЕНФИКА (мол)",
      "БИРКИРКАРА (мол)",
      "БОРУССИЯ Д (мол)",
      "ВАРДАР (мол)",
      "ВИКТОРИЯ ПЛ (мол)",
      "ГАЛАТАСАРАЙ (мол)",
      "ГРАССХОППЕР (мол)",
      "ДАУГАВА Д (мол)",
      "ДИНАМО З (мол)",
      "ДИНАМО ТБ (мол)",
      "ДЬЕР (мол)",
      "ЖЕЛЕЗНИЧАР (мол)",
      "ЗАЛЬЦБУРГ (мол)",
      "ЗЕНИТ (мол)",
      "ЗЮЛТЕ-ВАРЕГЕМ (мол)",
      "КЛИФТОНВИЛЛ (мол)",
      "КОПЕНГАГЕН (мол)",
      "ЛЕГИЯ (мол)",
      "ЛИОН (мол)",
      "ЛУДОГОРЕЦ (мол)",
      "ЛУСИТАНС (мол)",
      "МАККАБИ ТЕЛЬ-АВИВ (мол)",
      "МАНЧЕСТЕР СИТИ (мол)",
      "МАНЧЕСТЕР ЮНАЙТЕД (мол)",
      "МАРИБОР (мол)",
      "МАРСЕЛЬ (мол)",
      "МЕТАЛЛИСТ (мол)",
      "МИЛАН (мол)",
      "МОЛЬДЕ (мол)",
      "НАПОЛИ (мол)",
      "НЕФТЧИ (мол)",
      "НЫММЕ КАЛЬЮ (мол)",
      "ОЛИМПИАКОС П (мол)",
      "ПАОК (мол)",
      "ПАРТИЗАН (мол)",
      "ПАСУШ ДЕ ФЕРРЕЙРА (мол)",
      "ПОРТУ (мол)",
      "ПСВ (мол)",
      "ПСЖ (мол)",
      "РЕАЛ (мол)",
      "РЕАЛ СОСЬЕДАД (мол)",
      "СЕЛТИК (мол)",
      "СКЕНДЕРБЕУ (мол)",
      "СЛАЙГО РОВЕРС (мол)",
      "СЛОВАН БР (мол)",
      "СТРЕЙМЮР (мол)",
      "СТЯУА (мол)",
      "СУТЬЕСКА (мол)",
      "ТНС (мол)",
      "ТРЕ ПЕННЕ (мол)",
      "ФЕНЕРБАХЧЕ (мол)",
      "ФОЛА (мол)",
      "ХАБНАРФЬЕРДЮР (мол)",
      "ХИК (мол)",
      "ЦСКА (мол)",
      "ЧЕЛСИ (мол)",
      "ШАЛЬКЕ-04 (мол)",
      "ШАХТЕР Д (мол)",
      "ШАХТЕР КГ (мол)",
      "ШЕРИФ (мол)",
      "ШИРАК (мол)",
      "ЭКРАНАС (мол)",
      "ЭЛЬФСБОРГ (мол)",
      "ЮВЕНТУС (мол)"
    };

    #endregion

    
  }
}