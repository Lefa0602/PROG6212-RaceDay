using System;
using System.Collections.Generic;

namespace RaceDay.API.Models;

public partial class Event
{
    public int EventId { get; set; }

    public int OrganiserId { get; set; }

    public string EventName { get; set; } = null!;

    public string? Description { get; set; }

    public DateOnly EventDate { get; set; }

    public string Location { get; set; } = null!;

    public string EventType { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();

    public virtual ICollection<EventRoute> EventRoutes { get; set; } = new List<EventRoute>();

    public virtual User Organiser { get; set; } = null!;
}
