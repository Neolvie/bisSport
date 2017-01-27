using System;
using System.Web.Mvc;
using bisSport.Domain.Helpers;

namespace bisSport.WebClient.Helpers
{
  public class EntityModelBinder : DefaultModelBinder
  {
    protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext,
      Type modelType)
    {
      if (modelType.IsAbstract || modelType.IsInterface)
      {
        // Получаем guid типа сущности из формы. Первый раз для элемента коллекции BeginCollectionItem или
        // обычного свойства, второй раз для текущей сущности.
        var typeGuidValue = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + ".typeguid") 
          ?? bindingContext.ValueProvider.GetValue("typeguid");

        if (typeGuidValue != null)
        {
          var entity = CoreTypes.CreateEntityInstance(typeGuidValue.AttemptedValue);

          // Проверяем, правильно ли мы определили тип (т.к. в эту проверку могут войти и коллекции, которые содержаться в сущности
          // но guid будет основного типа сущности - поэтому коллекцию возращаем базовым CreateModel.
          if (modelType.IsInstanceOfType(entity))
          {
            return entity;
          }
        }
      }

      var model = base.CreateModel(controllerContext, bindingContext, modelType);
      return model;
    }
  }
}