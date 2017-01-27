using System;
using System.Data.SqlClient;
using bisSport.Domain.Core;
using bisSport.Domain.Core.Base;
using bisSport.Server.Mappings;
using bisSport.Server.NHibernate;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Helpers;
using NHibernate;
using NHibernate.Context;
using NHibernate.Tool.hbm2ddl;

namespace bisSport.Server.Repository
{
  public class UnitOfWork : IUnitOfWork
  {
    #region Репозитории сущностей

    private Repository<Address> _addressRepository;
    private Repository<Area> _areaRepository;
    private BaseRepository<EventBase> _eventBaseRepository;
    private Repository<Group> _groupRepository;
    private Repository<Match> _matchRepository;
    private Repository<MultiEvent> _multiEventRepository;
    private BaseRepository<OrganizerBase> _organizerBaseRepository;
    private BaseRepository<ParticipantBase> _participanBaseRepository;
    private Repository<Player> _playerRepository;
    private Repository<Point> _pointRepository;
    private Repository<Result> _resultRepository;
    private Repository<Round> _roundRepository;
    private Repository<Scorer> _scorerRepository;
    private Repository<Score> _scoreRepository;
    private Repository<SingleEvent> _singleEventRepository;
    private Repository<SportClub> _sportClubRepository;
    private Repository<SportType> _sportTypeRepository;
    private Repository<Structure> _structureRepository;
    private Repository<Team> _teamRepository;
    private Repository<Arbiter> _arbiterRepository;

    public Repository<Address> Addresses => _addressRepository ?? (_addressRepository = new Repository<Address>(this));
    public Repository<Area> Areas => _areaRepository ?? (_areaRepository = new Repository<Area>(this));
    public BaseRepository<EventBase> Events => _eventBaseRepository ?? (_eventBaseRepository = new BaseRepository<EventBase>(this));
    public Repository<Group> Groups => _groupRepository ?? (_groupRepository = new Repository<Group>(this));
    public Repository<Match> Matches => _matchRepository ?? (_matchRepository = new Repository<Match>(this));
    public Repository<MultiEvent> MultiEvents => _multiEventRepository ?? (_multiEventRepository = new Repository<MultiEvent>(this));
    public BaseRepository<OrganizerBase> Organizers => _organizerBaseRepository ?? (_organizerBaseRepository = new BaseRepository<OrganizerBase>(this));
    public BaseRepository<ParticipantBase> Participants => _participanBaseRepository ?? (_participanBaseRepository = new BaseRepository<ParticipantBase>(this));
    public Repository<Player> Players => _playerRepository ?? (_playerRepository = new Repository<Player>(this));
    public Repository<Point> Points => _pointRepository ?? (_pointRepository = new Repository<Point>(this));
    public Repository<Result> Results => _resultRepository ?? (_resultRepository = new Repository<Result>(this));
    public Repository<Round> Rounds => _roundRepository ?? (_roundRepository = new Repository<Round>(this));
    public Repository<Scorer> Scorers => _scorerRepository ?? (_scorerRepository = new Repository<Scorer>(this));
    public Repository<Score> Scores => _scoreRepository ?? (_scoreRepository = new Repository<Score>(this));
    public Repository<SingleEvent> SingleEvents => _singleEventRepository ?? (_singleEventRepository = new Repository<SingleEvent>(this));
    public Repository<SportClub> SportClubs => _sportClubRepository ?? (_sportClubRepository = new Repository<SportClub>(this));
    public Repository<SportType> SportTypes => _sportTypeRepository ?? (_sportTypeRepository = new Repository<SportType>(this));
    public Repository<Structure> Structures => _structureRepository ?? (_structureRepository = new Repository<Structure>(this));
    public Repository<Team> Teams => _teamRepository ?? (_teamRepository = new Repository<Team>(this));
    public Repository<Arbiter> Arbiters => _arbiterRepository ?? (_arbiterRepository = new Repository<Arbiter>(this));

    #endregion

    #region Статические свойства и методы

    private static ISessionFactory _sessionFactory;

    private static ISessionFactory SessionFactory
    {
      get
      {
        if (!Initialized)
          Initialize();

        return _sessionFactory;
      }
      set { _sessionFactory = value; }
    }

    private static string DefaultConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=BisSport;User ID=admin;Password=11111";
    public static bool Initialized;

    public static void Initialize(string connectionString = null)
    {
      Initialized = false;

      connectionString = string.IsNullOrEmpty(connectionString) ? DefaultConnectionString : connectionString;

      try
      {
        CreateDatabaseIfNotExist(connectionString);
      }
      catch (Exception)
      {
        // Ну не смогла...
      }

      SessionFactory = CreateSessionFactory(connectionString);

      Initialized = true;
    }

    private static ISessionFactory CreateSessionFactory(string connectionString)
    {
      return Fluently
        .Configure()
        .Database(MsSqlConfiguration.MsSql2012.ConnectionString(connectionString))
        .CurrentSessionContext("thread_static")
        .Mappings(m => m.FluentMappings.AddFromAssemblyOf<AddressMap>()
                  .Conventions.Add(ForeignKey.EndsWith("Id")))
        .ExposeConfiguration(cfg => new SchemaUpdate(cfg).Execute(false, true))
        .ExposeConfiguration(x => x.SetInterceptor(new AuditInterceptor()))
        .BuildSessionFactory();
    }

    private static void CreateDatabaseIfNotExist(string connectionString)
    {
      var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
      var databaseName = connectionStringBuilder.InitialCatalog;

      connectionStringBuilder.InitialCatalog = "master";

      using (var connection = new SqlConnection(connectionStringBuilder.ToString()))
      {
        connection.Open();

        using (var command = connection.CreateCommand())
        {
          command.CommandText = string.Format("select * from master.dbo.sysdatabases where name = '{0}'", databaseName);
          using (var reader = command.ExecuteReader())
          {
            if (reader.HasRows)
              return;
          }

          command.CommandText = string.Format("CREATE DATABASE {0}", databaseName);
          command.ExecuteNonQuery();
        }
      }
    }

    public static ISession GetCurrentOrCreateNewSession()
    {
      if (!CurrentSessionContext.HasBind(SessionFactory))
      {
        CurrentSessionContext.Bind(SessionFactory.OpenSession());
      }

      return SessionFactory.GetCurrentSession();
    }

    #endregion

    private ITransaction _transaction;
    private ISession _session;
    public ISession Session => _session ?? (_session = GetCurrentOrCreateNewSession());

    public void BeginTransaction()
    {
      _transaction = Session.BeginTransaction();
    }

    public void Commit()
    {
      try
      {
        if (_transaction != null && _transaction.IsActive)
          _transaction.Commit();
      }
      catch
      {
        if (_transaction != null && _transaction.IsActive)
          _transaction.Rollback();
        throw;
      }
      finally
      {
        Session.Clear();
      }
    }

    public void Rollback()
    {
      try
      {
        if (_transaction != null && _transaction.IsActive)
          _transaction.Rollback();
      }
      finally
      {
        Session.Clear();
      }
    }

    public void Merge(Entity entity)
    {
      Session.Merge(entity);
    }

    //public void Dispose()
    //{
    //  Dispose(true);
    //  GC.SuppressFinalize(this);
    //}

    //protected virtual void Dispose(bool disposing)
    //{
    //  //if (disposing)
    //  // Session?.Clear();
    //}

    //~UnitOfWork()
    //{
    //  Dispose(false);
    //}
  }
}