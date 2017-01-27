using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using bisSport.Domain.Core;
using bisSport.Domain.Core.Interfaces;
using bisSport.Domain.Events;
using NHibernate;
using NHibernate.Collection.Generic;
using NHibernate.Type;

namespace bisSport.Server.NHibernate
{
  public class AuditInterceptor : EmptyInterceptor
  {
    public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types)
    {
      ProcessBeforeSave(entity, state, propertyNames);

      return base.OnSave(entity, id, state, propertyNames, types);
    }

    public override bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames,
      IType[] types)
    {
      ProcessBeforeSave(entity, currentState, propertyNames);

      return base.OnFlushDirty(entity, id, currentState, previousState, propertyNames, types);
    }

    public override void OnDelete(object entity, object id, object[] state, string[] propertyNames, IType[] types)
    {
      var entityType = entity.GetType();
      var beforeDeleteMethod = entityType.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).FirstOrDefault(x => x.Name == "BeforeDelete");

      beforeDeleteMethod.Invoke(entity, new object[] { new BeforeEntityDeleteEventArgs() });

      base.OnDelete(entity, id, state, propertyNames, types);
    }

    private int? GetPropertyIndex(string[] propertyNames, string property)
    {
      for (var i = 0; i < propertyNames.Length; i++)
        if (propertyNames[i] == property)
          return i;

      return null;
    }

    private void ProcessBeforeSave(object entity, object[] state, string[] propertyNames)
    {
      var entityType = entity.GetType();
      var beforeSaveMethod = entityType.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).Single(x => x.Name == "BeforeSave");

      beforeSaveMethod.Invoke(entity, new object[] { new BeforeEntitySaveEventArgs() });

      foreach (var property in entityType.GetProperties())
      {
        var index = GetPropertyIndex(propertyNames, property.Name);
        if (index == null)
          continue;

        // TODO: не заменяются коллекции в BeforeSave. Пока миримся.
        if (typeof (IEnumerable).IsAssignableFrom(property.PropertyType))
          continue;

        var value = property.GetValue(entity);
        state[index.Value] = value;
      }
    }
  }
}