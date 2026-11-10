using Microsoft.AspNetCore.Mvc;

namespace RaceDay.MVC.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(role))
            {
                return RedirectToAction("Login", "Account");
            }

            if (role == "Organiser")
            {
                return View("Organiser");
            }

            if (role == "Participant")
            {
                return View("Participant");
            }

            return RedirectToAction("Login", "Account");
        }
    }
}
