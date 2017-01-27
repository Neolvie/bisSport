using System;
using bisSport.Domain.Enums;

namespace bisSport.Domain.Core.Interfaces
{
  public interface IEntity
  {
    int Id { get; }

    string TypeGuid { get; }

    string Name { get; set; }

    Status Status { get; set; }
  }
}