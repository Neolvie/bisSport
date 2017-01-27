using System.Collections.Generic;
using NHibernate;
using NHibernate.Collection.Generic;

namespace bisSport.Server.Helpers
{
  public static class CollectionsHelper
  {
    public static IList<T> ConvertListToBag<T>(IList<T> list, ISession session = null)
    {
      if (session == null)
        session = Repository.UnitOfWork.GetCurrentOrCreateNewSession();

      var sessionImplementor = session.GetSessionImplementation();

      return new PersistentGenericBag<T>(sessionImplementor, list);
    } 
  }
}