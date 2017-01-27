using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using bisSport.Server.Repository;
using bisSport.WebClient.Models;

namespace bisSport.WebClient.Controllers
{
  public class EventsController : BaseController
  {
    // GET: Events
    public ActionResult Index()
    {
      var events = Repository.Events.GetAll().ToList();

      var model = new EventListViewModel(events);

      return View(model);
    }

    [Description("Создать простой турнир")]
    public ActionResult CreateSingleEvent()
    {
      return HttpNotFound("Not implemented");
    }

    [Description("Создать соревнование")]
    public ActionResult CreateMultiEvent()
    {
      return HttpNotFound("Not implemented");
    }
  }
}