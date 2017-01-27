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
  public class AddressController : BaseController
  {
    // GET: Address
    public ActionResult Index(int id)
    {
      var address = Repository.Addresses.GetAll().SingleOrDefault(x => x.Id == id);

      if (address == null)
        return HttpNotFound();

      return View(address);
    }

    [Description("Создать адрес")]
    public ActionResult Create()
    {
      var address = Repository.Addresses.Create();

      return View("Edit", address);
    }

    [HttpPost]
    public ActionResult Create(Address address)
    {
      if (ModelState.IsValid)
      {
        address.Save();
        return RedirectToAction("Index", new {id = address.Id});
      }

      return View("Edit", address);
    }

    public ActionResult Edit(int id)
    {
      var address = Repository.Addresses.GetAll().SingleOrDefault(x => x.Id == id);

      if (address == null)
        return HttpNotFound();

      return View(address);
    }

    [HttpPost]
    public ActionResult Edit(Address address)
    {
      if (ModelState.IsValid)
      {
        address.Save();
        return RedirectToAction("Index", new { id = address.Id });
      }

      return View(address);
    }

    public ActionResult Delete(int id)
    {
      var address = Repository.Addresses.GetAll().FirstOrDefault(x => x.Id == id);

      if (address == null)
        return HttpNotFound();

      address.Delete();

      return RedirectToAction("Index", "Home");
    }
  }
}