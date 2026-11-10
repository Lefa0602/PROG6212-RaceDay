namespace RaceDay.MVC.Models
{
    public class ApiEventViewModel
    {
        public int EventId { get; set; }
        public string EventName { get; set; } = "";
        public string Description { get; set; } = "";
        public DateOnly EventDate { get; set; }
        public string Location { get; set; } = "";
        public string EventType { get; set; } = "";
        public List<ApiCategoryViewModel> Categories { get; set; } = new();
    }

    public class ApiCategoryViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = "";
        public decimal DistanceKm { get; set; }
        public int MaxParticipants { get; set; }
        public decimal EntryFee { get; set; }
    }
}
