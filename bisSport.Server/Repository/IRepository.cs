using System.Linq;
using bisSport.Domain.Core.Base;
using bisSport.Domain.Core.Interfaces;

namespace bisSport.Server.Repository
{
  public interface IRepository<T> where T: Entity
  {
    IQueryable<T> GetAll();
    T Get(int id);
  }
}