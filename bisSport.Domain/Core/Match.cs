using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using bisSport.Domain.Core.Base;
using bisSport.Domain.Core.Interfaces;
using bisSport.Domain.Events;
using bisSport.Domain.Enums;

namespace bisSport.Domain.Core
{
  public class Match : Entity
  {
    public virtual IList<IParticipant> Participants { get; set; }

    [Required(ErrorMessage = "Не заполнено обязательное поле")]
    public virtual Round Round { get; set; }

    public virtual DateTime BeginDate { get; set; }

    [Required(ErrorMessage = "Не заполнено обязательное поле")]
    public virtual int Game { get; set; }

    [Required(ErrorMessage = "Не заполнено обязательное поле")]
    public virtual Area Area { get; set; }

    public virtual Result Result { get; set; }

    [Required(ErrorMessage = "Не заполнено обязательное поле")]
    public virtual SingleEvent Event { get; set; }

    public virtual Match PrevMatch { get; set; }

    public Match()
    {
      Participants = new List<IParticipant>();
    }

    protected override void BeforeSave(BeforeEntitySaveEventArgs e)
    {
      base.BeforeSave(e);

      Name = string.Join(" - ", Participants.Select(x => x.Name));
    }

    public virtual IParticipant DefineWinner()
    {
      var wins = new Dictionary<IParticipant, int>();
      foreach (var participant in Participants)
      {
        wins.Add(participant, Result.Scores.Count(x => Equals(x.Participant, participant) && x.Point.GameResult == GameResult.Win));
      }
      return wins.OrderBy(x => x.Value).FirstOrDefault().Key;
    }
  }
}