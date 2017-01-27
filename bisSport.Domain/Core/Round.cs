using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using bisSport.Domain.Core.Base;
using bisSport.Domain.Core.Interfaces;
using bisSport.Domain.Enums;
using bisSport.Domain.Events;

namespace bisSport.Domain.Core
{
  public class Round : Entity
  {
    [Required(ErrorMessage = "Не заполнено обязательное поле")]
    public virtual RoundType Type { get; set; }

    public virtual int? MinParticipants { get; set; }

    public virtual int? MaxParticipants { get; set; }

    public virtual int RoundNumber { get; set; }

    public virtual int Periods { get; set; }

    public virtual int? MaxPeriodPoints { get; set; }

    public virtual int Games { get; set; }

    public virtual int WinnerCount { get; set; }

    public virtual int? ParticipantsPerGroup { get; set; }

    public virtual IList<Group> Groups { get; set; }

    public virtual IList<Point> Points { get; set; }

    public Round()
    {
      Points = new List<Point>();
      Groups = new List<Group>();
    }

    protected override void BeforeSave(BeforeEntitySaveEventArgs e)
    {
      base.BeforeSave(e);

      if (string.IsNullOrEmpty(Name))
        Name = "Круг " + RoundNumber;
    }
  }
}