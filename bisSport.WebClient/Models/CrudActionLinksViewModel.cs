using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Web.Routing;

namespace bisSport.WebClient.Models
{
  public class CrudActionLinksViewModel
  {
    private const string EditActionName = "Edit";
    private const string DeleteActionName = "Delete";
    private const string CreateActionName = "Create";

    public ActionDescription CreateAction;
    public ActionDescription DeleteAction;
    public ActionDescription EditAction;

    public bool HasActions;

    public CrudActionLinksViewModel(string controllerName, string currentAction, string area, int entityId)
    {
      GetControllerActionInfo(controllerName, currentAction, area, entityId);
    }

    public CrudActionLinksViewModel(RouteData routeData)
    {
      var controllerName = routeData.Values["controller"].ToString();
      var currentAction = routeData.Values["action"].ToString();
      var area = routeData.DataTokens["area"]?.ToString() ?? string.Empty;
      var entityIdValue = routeData.Values["id"];
      int entityId;
      int.TryParse((string)entityIdValue, out entityId);

      GetControllerActionInfo(controllerName, currentAction, area, entityId);
    }

    private void GetControllerActionInfo(string controllerName, string currentAction, string area, int entityId)
    {
      var controllerInfo = Helpers.ControllerList.Controllers.SingleOrDefault(x => x.Name == $"{controllerName}Controller" && x.Area == area);

      if (controllerInfo == null)
        return;

      if (controllerInfo.Actions.Any(x => x.Key.StartsWith(CreateActionName)) && !currentAction.StartsWith(CreateActionName))
      {
        var actions = controllerInfo.Actions.Where(x => x.Key.StartsWith(CreateActionName)).ToDictionary(x => x.Key, x => x.Value);
        if (CreateAction == null)
          CreateAction = new ActionDescription(controllerName, actions, area, 0);
        HasActions = true;
      }

      if (controllerInfo.Actions.Any(x => x.Key.StartsWith(DeleteActionName)) && entityId != 0 && !currentAction.StartsWith(DeleteActionName))
      {
        var actions = controllerInfo.Actions.Where(x => x.Key.StartsWith(DeleteActionName)).ToDictionary(x => x.Key, x => x.Value); ;
        DeleteAction = new ActionDescription(controllerName, actions, area, entityId);
        HasActions = true;
      }

      if (controllerInfo.Actions.Any(x => x.Key.StartsWith(EditActionName)) && entityId != 0 && !currentAction.StartsWith(EditActionName))
      {
        var actions = controllerInfo.Actions.Where(x => x.Key.StartsWith(EditActionName)).ToDictionary(x => x.Key, x => x.Value);
        EditAction = new ActionDescription(controllerName, actions, area, entityId);
        HasActions = true;
      }
    }

    public class ActionDescription
    {
      public Dictionary<string, string> ActionNames { get; set; }
      public string ControllerName { get; set; }
      public string Area { get; set; }
      public int EntityId { get; set; }

      public ActionDescription(string controllerName, Dictionary<string, string> actionName, string area, int entityId)
      {
        ControllerName = controllerName;
        ActionNames = actionName;
        Area = area;
        EntityId = entityId;
      }
    }
  }
}