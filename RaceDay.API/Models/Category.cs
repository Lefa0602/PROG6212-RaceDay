using System;
using System.Collections.Generic;

namespace RaceDay.API.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public int EventId { get; set; }

    public string CategoryName { get; set; } = null!;

    public decimal DistanceKm { get; set; }

    public int MaxParticipants { get; set; }

    public decimal EntryFee { get; set; }

    public virtual ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();

    public virtual Event Event { get; set; } = null!;
}
