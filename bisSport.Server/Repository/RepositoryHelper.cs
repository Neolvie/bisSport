using bisSport.Domain.Core.Base;
using bisSport.Domain.Core.Interfaces;
using NHibernate;
using NHibernate.Proxy;

namespace bisSport.Server.Repository
{
  public static class RepositoryHelper
  {
    public static T As<T>(this IEntity entity) where T : Entity
    {
      var session = UnitOfWork.GetCurrentOrCreateNewSession();

      if (!NHibernateUtil.IsInitialized(entity))
        NHibernateUtil.Initialize(entity);

      // ReSharper disable once SuspiciousTypeConversion.Global
      // Проксики наследуются в рантайме.
      if (entity is INHibernateProxy)
        entity = (IEntity)session.GetSessionImplementation().PersistenceContext.Unproxy(entity);

      return entity as T;
    }

    public static bool Is<T>(this IEntity entity) where T : Entity
    {
      var session = UnitOfWork.GetCurrentOrCreateNewSession();

      if (!NHibernateUtil.IsInitialized(entity))
        NHibernateUtil.Initialize(entity);

      // ReSharper disable once SuspiciousTypeConversion.Global
      // Проксики наследуются в рантайме.
      if (entity is INHibernateProxy)
        entity = (IEntity)session.GetSessionImplementation().PersistenceContext.Unproxy(entity);

      return entity is T;
    }
  }
}