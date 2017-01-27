using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using bisSport.Server.Repository;

namespace bisSport.WebClient.Controllers
{
  public class SingleEventController : BaseController
  {
    // GET: SingleEvent
    public ActionResult Index(int id)
    {
      var singleEvent = Repository.SingleEvents.GetAll().FirstOrDefault(x => x.Id == id);

      if (singleEvent == null)
        return HttpNotFound("Соревнование не найдено");

      return View(singleEvent);
    }

    [Description("Создать простой турнир")]
    public ActionResult Create()
    {
      return RedirectToAction("CreateSingleEvent", "Events");
    }
  }
}