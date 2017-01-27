using System;
using System.Linq;
using System.Web.Mvc;
using bisSport.Domain.Enums;
using bisSport.Server.Repository;
using bisSport.WebClient.Areas.Tennis.Models;
using bisSport.WebClient.Controllers;

namespace bisSport.WebClient.Areas.Tennis.Controllers
{
  public class RoundController : BaseController
  {
    // GET: Round
    public ActionResult Index(int id)
    {
      var round = Repository.Rounds.GetAll().FirstOrDefault(x => x.Id == id);

      if (round == null)
        return HttpNotFound();

      return View(round);
    }

    public ActionResult MatchList(int roundId)
    {
      var round = Repository.Rounds.GetAll().FirstOrDefault(x => x.Id == roundId);

      if (round == null)
        return HttpNotFound();

      var matches = Repository.Matches.GetAll().Where(x => x.Round.Id == roundId).ToList();

      var model = new MatchListViewModel(round, matches);

      switch (round.Type)
      {
        case RoundType.AllPlayAll:
          return View("~/Areas/Tennis/Views/Round/AllPlayAllRound.cshtml", model);
        case RoundType.Playoff:
          return View("~/Areas/Tennis/Views/Round/PlayOffRound.cshtml", model);
        default:
          throw new ArgumentOutOfRangeException();
      }
    }
  }
}