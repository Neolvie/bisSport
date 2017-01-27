using System.Web.Mvc;
using bisSport.Server.Repository;

namespace bisSport.WebClient.Controllers
{
  public class BaseController : Controller
  {
    protected UnitOfWork Repository { get; set; }

    public BaseController()
    {
      Repository = new UnitOfWork();
    }

    protected override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      Repository.BeginTransaction();

      base.OnActionExecuting(filterContext);
    }

    protected override void OnResultExecuted(ResultExecutedContext filterContext)
    {
      Repository.Commit();

      base.OnResultExecuted(filterContext);
    }
  }
}