using System.Linq;
using System.Web.Mvc;
using bisSport.Domain.Core;
using bisSport.Domain.Core.Base;
using bisSport.Domain.Helpers;
using bisSport.Server.Repository;
using bisSport.WebClient.Models;

namespace bisSport.WebClient.Controllers
{
  public class ParticipantController : BaseController
  {
    // GET: Participant
    public ActionResult Index(int id)
    {
      var participant = Repository.Participants.GetAll().FirstOrDefault(x => x.Id == id);

      if (participant == null)
        return HttpNotFound();

      var participantType = CoreTypes.GetEntityType(participant.TypeGuid);

      return RedirectToAction("Index", participantType.Name, new { id });
    }
  }
}