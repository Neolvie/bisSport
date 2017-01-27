using System.Linq;
using bisSport.Domain.Core.Base;
using bisSport.Domain.Core.Interfaces;
using NHibernate;
using NHibernate.Linq;
using NHibernate.Proxy;

namespace bisSport.Server.Repository
{
  public class BaseRepository<T> : IRepository<T> where T : Entity
  {
    private readonly UnitOfWork _unitOfWork;
    protected ISession Session => _unitOfWork.Session;

    public BaseRepository(IUnitOfWork unitOfWork)
    {
      _unitOfWork = (UnitOfWork)unitOfWork;
    }

    public IQueryable<T> GetAll()
    {
      return Session.Query<T>();
    }

    public T Get(int id)
    {
      return Session.Get<T>(id);
    }

    public IEntity Unproxy(IEntity entity)
    {
      return (IEntity)Session.GetSessionImplementation().PersistenceContext.Unproxy(entity);
    }
  }
}