using Microsoft.AspNetCore.Mvc.Rendering;

namespace RaceDay.MVC.Models
{
    public class EnrolmentCreateViewModel
    {
        public int CategoryId { get; set; }

        public List<SelectListItem> Categories { get; set; } = new();

        public string? Message { get; set; }
    }

    public class MyEnrolmentViewModel
    {
        public int EnrolmentId { get; set; }
        public int ParticipantId { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = "";
        public string EventName { get; set; } = "";
        public DateTime EnrolmentDate { get; set; }
        public string Status { get; set; } = "";
    }
}
