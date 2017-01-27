using System.Linq;
using bisSport.Domain.Core.Base;
using NHibernate;
using NHibernate.Linq;

namespace bisSport.Server.Repository
{
  public class Repository<T> : BaseRepository<T> where T : Entity, new()
  {
    public Repository(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
      
    }

    public T Create()
    {
      return new T();
    }
  }
}