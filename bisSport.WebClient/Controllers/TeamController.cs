using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using bisSport.Domain.Core;
using bisSport.Domain.Enums;
using bisSport.Server.Helpers;
using bisSport.Server.Repository;
using bisSport.WebClient.Models;

namespace bisSport.WebClient.Controllers
{
  public class TeamController : BaseController
  {
    // GET: Team
    public ActionResult Index(int id)
    {
      var team = Repository.Teams.GetAll().FirstOrDefault(x => x.Id == id);

      if (team == null)
        return HttpNotFound();

      return View(team);
    }

    [Description("Создать команду")]
    public ActionResult Create()
    {
      var team = Repository.Teams.Create();

      return View("Edit", team);
    }

    [HttpPost]
    public ActionResult Create(Team team)
    {
      if (ModelState.IsValid)
      {
        team.Save();
        return RedirectToAction("Index", new { id = team.Id });
      }

      return View("Edit", team);
    }

    public ActionResult Edit(int id)
    {
      var team = Repository.Teams.Get(id);

      if (team == null)
        return HttpNotFound();

      return View(team);
    }

    [HttpPost]
    public ActionResult Edit(Team team)
    {
      if (ModelState.IsValid)
      {
        team.Save();
        return RedirectToAction("Index", new { id = team.Id });
      }

      return View(team);
    }

    public ActionResult Delete(int id)
    {
      var team = Repository.Teams.GetAll().FirstOrDefault(x => x.Id == id);

      if (team == null)
        return HttpNotFound();

      var hasInvolvedInMatches = Repository.Matches.GetAll()
        .Any(x => x.Participants.Any(p => p.Id == team.Id &&
                                          p.Type == ParticipantType.Team));
      if (hasInvolvedInMatches)
      {
        team.Status = Status.Closed;
        team.Save();
      }
      else
        team.Delete();

      return RedirectToAction("Index", "Home");
    }

    public PartialViewResult PlayerAdd()
    {
      return PartialView("PlayerAdd", Repository.Players.Create());
    }
  }
}