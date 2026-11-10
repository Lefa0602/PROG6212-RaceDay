using System.Net.Http.Json;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace RaceDay.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var client = _httpClientFactory.CreateClient("RaceDayAPI");

                var response = await client.PostAsJsonAsync(
                    "api/Auth/login",
                    new { email = model.Email, password = model.Password });

                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError("", "Invalid email or password.");
                    return View(model);
                }

                var login = await response.Content.ReadFromJsonAsync<LoginResponse>();

                if (login == null || string.IsNullOrWhiteSpace(login.Token))
                {
                    ModelState.AddModelError("", "Invalid login response.");
                    return View(model);
                }

                HttpContext.Session.SetString("AuthToken", login.Token);
                HttpContext.Session.SetString("UserId", login.UserId.ToString());
                HttpContext.Session.SetString("UserName", login.FirstName + " " + login.LastName);
                HttpContext.Session.SetString("UserRole", login.Role);

                return RedirectToAction("Index", "Dashboard");
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError("", "Cannot connect to RaceDay API.");
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Dashboard");
        }
    }

    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}

