using System.Collections.Generic;
using bisSport.Domain.Core.Base;

namespace bisSport.WebClient.Models
{
  public class EventListViewModel
  {
    public List<EventBase> Events { get; set; }

    public EventListViewModel()
    {
      Events = new List<EventBase>();
    }

    public EventListViewModel(List<EventBase> events)
    {
      Events = events;
    } 
  }
}