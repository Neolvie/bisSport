using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using bisSport.Domain.Core.Exceptions;
using bisSport.Domain.Core.Interfaces;
using bisSport.Domain.Enums;
using bisSport.Domain.Events;
using bisSport.Domain.Helpers;
using ValidationException = bisSport.Domain.Core.Exceptions.ValidationException;

namespace bisSport.Domain.Core.Base
{
  public abstract class Entity : IEntity
  {
    #region Поля и свойства

    public virtual int Id { get; set; }

    public virtual string TypeGuid { get; protected set; }

    [Display(Name = "Название")]
    public virtual string Name { get; set; }

    [Display(Name = "Статус записи")]
    [Required(ErrorMessage = "Не заполнено обязательное поле")]
    public virtual Status Status { get; set; }

    #endregion

    #region Базовый класс

    public override string ToString()
    {
      return $"{Name}, Id={Id}";
    }

    public override bool Equals(object obj)
    {
      var entity = obj as Entity;

      if (entity == null)
        return false;

      return entity.Id == this.Id && entity.TypeGuid == this.TypeGuid;
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return (Id * 397) ^ (TypeGuid.GetHashCode());
      }
    }

    #endregion

    #region Конструктор

    protected Entity()
    {
      TypeGuid = this.GetEntityGuid();
      Status = Status.Active;
    }

    #endregion

    #region Обработчики событий

    protected virtual void BeforeSave(BeforeEntitySaveEventArgs e)
    {

    }

    protected virtual void BeforeDelete(BeforeEntityDeleteEventArgs e)
    {

    }

    protected void Validate()
    {
      var type = this.GetType();
      var allErrors = new List<string>();

      foreach (var propertyInfo in type.GetProperties())
      {
        var value = propertyInfo.GetValue(this);
        var context = new ValidationContext(this, null, null);
        var results = new List<ValidationResult>();
        var validationAttributes = propertyInfo.GetCustomAttributes(true).OfType<ValidationAttribute>().ToArray();

        if (!Validator.TryValidateValue(value, context, results, validationAttributes))
        {
          allErrors.AddRange(results.Select(x => $"{x.ErrorMessage} {propertyInfo.Name} ({type.Name})"));
        }
      }

      if (allErrors.Any())
        throw new ValidationException(string.Join("; ", allErrors));
    }

    #endregion
  }
}