using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using bisSport.Domain.Core.Interfaces;
using bisSport.Server.Repository;

namespace bisSport.Server.Helpers
{
  public static class EntityHelper
  {
    public static void SaveAll(this IEnumerable<IEntity> objects)
    {
      var objectList = objects.ToList();

      foreach (var entity in objectList)
        entity.Save();
    }

    public static void Save(this IEntity entity)
    {
      ValidateBeforeSave(entity);

      var session = UnitOfWork.GetCurrentOrCreateNewSession();

      using (var transact = session.BeginTransaction())
      {
        try
        {
          if (entity.Id == 0)
            session.Save(entity);
          else
            session.Update(entity);

          ValidateBeforeSave(entity);

          transact.Commit();

          session.Refresh(entity);
        }
        catch (Exception)
        {
          transact.Rollback();
          throw;
        }
      }
    }

    public static void DeleteAll(this IEnumerable<IEntity> objects)
    {
      var objectList = objects.ToList();

      foreach (var entity in objectList)
        entity.Delete();
    }

    public static void Delete(this IEntity entity)
    {
      if (entity.Id == 0)
        return;

      var session = UnitOfWork.GetCurrentOrCreateNewSession();

      using (var transact = session.BeginTransaction())
      {
        try
        {
          session.Delete(entity);

          transact.Commit();
        }
        catch (Exception)
        {
          transact.Rollback();
          throw;
        }
      }

    }

    private static void ValidateBeforeSave(IEntity entity)
    {
      if (entity == null)
        throw new ArgumentNullException(nameof(entity), "No entity for save");

      var entityType = entity.GetType();
      var validateMethod = entityType.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).Single(x => x.Name == "Validate");
      try
      {
        validateMethod.Invoke(entity, null);
      }
      catch (TargetInvocationException ex)
      {
        throw ex.InnerException;
      }
    }
  }
}
