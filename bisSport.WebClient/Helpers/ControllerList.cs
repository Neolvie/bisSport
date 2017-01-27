using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace bisSport.WebClient.Helpers
{
  public class ControllerList
  {
    private static readonly List<string> CrudActionName = new List<string> { "Create", "Delete", "Edit" }; 

    private static List<SimpleControllerInfo> _controllers;

    public static List<SimpleControllerInfo> Controllers => _controllers ?? (_controllers = CollectControllers());

    private static List<SimpleControllerInfo> CollectControllers()
    {
      var controllers = new List<SimpleControllerInfo>();
      var types = System.Reflection.Assembly.GetExecutingAssembly().GetTypes().Where(x => x.IsSubclassOf(typeof (Controller))).ToList();

      foreach (var type in types)
      {
        var area = GetControllerArea(type);
        var methods = type.GetMethods()
          .Where(x => CrudActionName.Any(n => x.Name.StartsWith(n, StringComparison.CurrentCultureIgnoreCase)))
          .Where(x => x.GetCustomAttributes(typeof(HttpPostAttribute), false).FirstOrDefault() == null)
          .ToDictionary(x => x.Name, GetMethodDescription);

        controllers.Add(new SimpleControllerInfo(type.Name, methods, area));
      }

      return controllers;
    }

    private static string GetControllerArea(Type type)
    {
      var fullName = type.FullName;

      var pos = fullName.IndexOf(".Areas.", StringComparison.InvariantCultureIgnoreCase);

      if (pos == -1)
        return string.Empty;

      pos = pos + 7;

      var endAreaName = fullName.IndexOf(".", pos, StringComparison.InvariantCultureIgnoreCase);

      var areaName = fullName.Substring(pos, endAreaName - pos);

      return areaName;
    }

    private static string GetMethodDescription(MethodInfo method)
    {
      var descAttribute = method.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault();

      if (descAttribute != null)
        return ((DescriptionAttribute)descAttribute).Description;

      return string.Empty;
    }

    public class SimpleControllerInfo
    {
      public string Name { get; set; }
      public string Area { get; set; }
      public Dictionary<string, string> Actions { get; set; }

      public SimpleControllerInfo(string name, Dictionary<string, string> actions, string area)
      {
        Name = name;
        Actions = actions;
        Area = area;
      }
    }
  }
}