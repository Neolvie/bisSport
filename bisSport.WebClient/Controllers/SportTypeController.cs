using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using bisSport.Domain.Core;
using bisSport.Server.Helpers;

namespace bisSport.WebClient.Controllers
{
  public class SportTypeController : BaseController
  {
    // GET: SportType
    public ActionResult Index(int id)
    {
      var sportType = Repository.SportTypes.Get(id);

      if (sportType == null)
        return HttpNotFound();

      return View(sportType);
    }

    [Description("Создать вид спорта")]
    public ActionResult Create()
    {
      var sportType = Repository.SportTypes.Create();

      return View("Edit", sportType);
    }

    [HttpPost]
    public ActionResult Create(SportType sportType)
    {
      if (ModelState.IsValid)
      {
        sportType.Save();
        return RedirectToAction("Index", new { id = sportType.Id });
      }

      return View("Edit", sportType);
    }

    public ActionResult Edit(int id)
    {
      var sportType = Repository.SportTypes.GetAll().SingleOrDefault(x => x.Id == id);

      if (sportType == null)
        return HttpNotFound();

      return View(sportType);
    }

    [HttpPost]
    public ActionResult Edit(SportType sportType)
    {
      if (ModelState.IsValid)
      {
        sportType.Save();
        return RedirectToAction("Index", new { id = sportType.Id });
      }

      return View(sportType);
    }

    public ActionResult Delete(int id)
    {
      var sportType = Repository.SportTypes.GetAll().FirstOrDefault(x => x.Id == id);

      if (sportType == null)
        return HttpNotFound();

      sportType.Delete();

      return RedirectToAction("Index", "Home");
    }
  }
}