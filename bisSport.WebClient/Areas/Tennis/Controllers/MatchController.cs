using System.Linq;
using System.Web.Mvc;
using bisSport.Server.Repository;
using bisSport.WebClient.Controllers;

namespace bisSport.WebClient.Areas.Tennis.Controllers
{
  public class MatchController : BaseController
  {
    // GET: Match
    public ActionResult Index(int id)
    {
      var match = Repository.Matches.GetAll().FirstOrDefault(x => x.Id == id);

      if (match == null)
        return HttpNotFound();

      return View(match);
    }
  }
}