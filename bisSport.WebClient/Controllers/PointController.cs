using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using bisSport.Domain.Core;
using bisSport.Domain.Core.Base;
using bisSport.Server.Helpers;
using bisSport.Server.Repository;
using bisSport.WebClient.Models;
using WebGrease.Css.Extensions;

namespace bisSport.WebClient.Controllers
{
  public class PointController : BaseController
  {
    // GET: Point
    public ActionResult Index(int id)
    {
      var point = Repository.Points.Get(id);

      if (point == null)
        return HttpNotFound();

      return View(point);
    }

    public ActionResult Edit(int id)
    {
      var point = Repository.Points.Get(id);

      if (point == null)
        return HttpNotFound();

      return View(point);
    }

    [HttpPost]
    public ActionResult Edit(Point point)
    {
      if (ModelState.IsValid)
      {
        point.Save();
        return RedirectToAction("Index", new { id = point.Id });
      }

      return View(point);
    }

    [Description("Добавить очкующего")]
    public PartialViewResult AddScorerRow(int playerId, int min, int sec)
    {
      var scorer = Repository.Scorers.Create();
      scorer.Participant = Repository.Participants.Get(playerId);
      scorer.Time = new TimeSpan(0, 0, min, sec);

      return PartialView("ScorerRow", scorer);
    }

    class JsonParticipant
    {
      public string text { get; set; }
      public int id { get; set; }
    }

    public JsonResult GetParticipants(int pointId)
    {
      var results = Repository.Results.GetAll().Where(x => x.Scores.Any(y => y.Point.Id == pointId));
      var matches = Repository.Matches.GetAll().Where(x => results.Contains(x.Result));
      var participants = matches.SelectMany(x => x.Participants);
      var players = new List<Player>();
      foreach (var participant in participants)
      {
        if (participant.Is<Team>())
        {
          players.AddRange(participant.As<Team>().Players);
        }
        else
        {
          players.Add(participant.As<Player>());
        }
      }

      return Json(players.Select(x => new JsonParticipant() {id = x.Id, text = x.Name}), JsonRequestBehavior.AllowGet);
    }
  }
}