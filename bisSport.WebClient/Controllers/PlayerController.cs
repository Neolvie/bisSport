using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using bisSport.Domain.Core;
using bisSport.Server.Helpers;
using bisSport.Server.Repository;
using bisSport.WebClient.Models;

namespace bisSport.WebClient.Controllers
{
  public class PlayerController : BaseController
  {
    // GET: Player
    public ActionResult Index(int id)
    {
      var player = Repository.Players.GetAll().FirstOrDefault(x => x.Id == id);

      if (player == null)
        return HttpNotFound();

      var matches = Repository.Matches.GetAll().Where(x => x.Result != null)
        .Where(x => x.Result.Scores.Any(s => s.Point.Scorers.Any(g => g.Participant.Id == player.Id))).ToList();

      var model = new PlayerInfoViewModel(player, matches);

      return View(model);
    }

    [Description("Создать игрока")]
    public ActionResult Create()
    {
      var player = Repository.Players.Create();

      var teams = Repository.Teams.GetAll().Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() });

      ViewData["Teams"] = new SelectList(teams, "Value", "Text", player.Team?.Id.ToString() ?? "Не указана");

      return View("Edit", player);
    }

    [Description("Создать игрока")]
    public PartialViewResult CreatePlayerRow(string name)
    {
      var player = Repository.Players.Create();
      player.Name = name;

      return PartialView("~/Views/Team/PlayerRow.cshtml", player);
    }

    [HttpPost]
    public ActionResult Create(Player player)
    {
      if (ModelState.IsValid)
      {
        player.Save();
        return RedirectToAction("Index", new { id = player.Id });
      }

      var teams = Repository.Teams.GetAll().Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() });
      ViewData["Teams"] = new SelectList(teams, "Value", "Text", player.Team?.Id.ToString() ?? "Не указана");

      return View("Edit", player);
    }

    public ActionResult Edit(int id)
    {
      var player = Repository.Players.GetAll().SingleOrDefault(x => x.Id == id);

      if (player == null)
        return HttpNotFound();

      var teams = Repository.Teams.GetAll().Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() });

      ViewData["Teams"] = new SelectList(teams, "Value", "Text", player.Id);

      return View(player);
    }

    [HttpPost]
    public ActionResult Edit(Player player)
    {
      if (ModelState.IsValid)
      {
        player.Save();
        return RedirectToAction("Index", new { id = player.Id });
      }

      var teams = Repository.Teams.GetAll().Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() });
      ViewData["Teams"] = new SelectList(teams, "Value", "Text", player.Team?.Id.ToString() ?? "Не указана");

      return View(player);
    }

    public ActionResult Delete(int id)
    {
      var player = Repository.Players.GetAll().FirstOrDefault(x => x.Id == id);

      if (player == null)
        return HttpNotFound();

      player.Delete();

      return RedirectToAction("Index", "Home");
    }
  }
}