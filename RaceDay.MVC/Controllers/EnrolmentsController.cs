using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RaceDay.MVC.Models;

namespace RaceDay.MVC.Controllers
{
    public class EnrolmentsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public EnrolmentsController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new EnrolmentCreateViewModel();

            try
            {
                var client = _httpClientFactory.CreateClient("RaceDayAPI");
                var events = await client.GetFromJsonAsync<List<ApiEventViewModel>>(
                    "api/Events");

                if (events != null)
                {
                    model.Categories = events
                        .SelectMany(e => e.Categories.Select(c =>
                            new SelectListItem
                            {
                                Value = c.CategoryId.ToString(),
                                Text = $"{e.EventName} - {c.CategoryName} ({c.DistanceKm} km - R{c.EntryFee:0.00})"
                            }))
                        .ToList();
                }
            }
            catch (HttpRequestException)
            {
                model.Message = "Unable to load event categories. Please try again.";
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EnrolmentCreateViewModel model)
        {
            var token = HttpContext.Session.GetString("AuthToken");

            if (string.IsNullOrWhiteSpace(token))
            {
                return RedirectToAction("Login", "Account");
            }

            if (model.CategoryId <= 0)
            {
                ModelState.AddModelError("CategoryId", "Please select an event category.");
            }

            if (!ModelState.IsValid)
            {
                await LoadCategories(model);
                return View(model);
            }

            try
            {
                var client = _httpClientFactory.CreateClient("RaceDayAPI");
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var response = await client.PostAsJsonAsync(
                    "api/Enrolments",
                    new { categoryId = model.CategoryId });

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Your enrolment has been submitted successfully.";
                    return RedirectToAction(nameof(MyEnrolments));
                }

                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", 
                    $"Enrolment could not be submitted. {error}");
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError("", "Cannot connect to the RaceDay API.");
            }

            await LoadCategories(model);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> MyEnrolments()
        {
            var token = HttpContext.Session.GetString("AuthToken");
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrWhiteSpace(token) ||
                string.IsNullOrWhiteSpace(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var client = _httpClientFactory.CreateClient("RaceDayAPI");
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var enrolments =
                    await client.GetFromJsonAsync<List<MyEnrolmentViewModel>>(
                        $"api/Enrolments/participant/{userId}");

                return View(enrolments ?? new List<MyEnrolmentViewModel>());
            }
            catch (HttpRequestException)
            {
                ViewBag.Error = "Unable to retrieve your enrolments from the API.";
                return View(new List<MyEnrolmentViewModel>());
            }
        }

        private async Task LoadCategories(EnrolmentCreateViewModel model)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("RaceDayAPI");
                var events = await client.GetFromJsonAsync<List<ApiEventViewModel>>(
                    "api/Events");

                model.Categories = events?
                    .SelectMany(e => e.Categories.Select(c =>
                        new SelectListItem
                        {
                            Value = c.CategoryId.ToString(),
                            Text = $"{e.EventName} - {c.CategoryName} ({c.DistanceKm} km - R{c.EntryFee:0.00})"
                        }))
                    .ToList() ?? new List<SelectListItem>();
            }
            catch (HttpRequestException)
            {
                model.Message = "Unable to load event categories.";
            }
        }
    }
}
