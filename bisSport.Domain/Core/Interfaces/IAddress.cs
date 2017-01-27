namespace bisSport.Domain.Core.Interfaces
{
  public interface IAddress : IEntity
  {
    string PostAddress { get; set; }

    double? Latitude { get; set; }

    double? Longitude { get; set; }

    string Phone { get; set; }
  }
}