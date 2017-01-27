using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using bisSport.Domain.Core.Base;
using bisSport.Domain.Enums;

namespace bisSport.Domain.Core
{
  public class Player : ParticipantBase
  {
    [Display(Name = "Имя")]
    [Required(ErrorMessage = "Не заполнено обязательное поле"), MinLength(1, ErrorMessage = "Не заполнено обязательное поле")]
    public override string Name { get; set; }

    public Player()
    {
      Type = ParticipantType.Player;
    }
  }
}