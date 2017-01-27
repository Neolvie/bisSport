using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using bisSport.Server.Repository;
using bisSport.WebClient.Models;

namespace bisSport.WebClient.Controllers
{
  public class HomeController : BaseController
  {
    public ActionResult Index()
    {   
      return RedirectToAction("Index", "Events");
    }
  }
}