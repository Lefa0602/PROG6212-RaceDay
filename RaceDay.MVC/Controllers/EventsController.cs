using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Models;

namespace RaceDay.MVC.Controllers
{
    public class EventsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public EventsController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("RaceDayAPI");

                var events = await client.GetFromJsonAsync<List<ApiEventViewModel>>(
                    "api/Events");

                return View(events ?? new List<ApiEventViewModel>());
            }
            catch (HttpRequestException)
            {
                ViewBag.Error = "Unable to connect to the RaceDay API. Please try again later.";
                return View(new List<ApiEventViewModel>());
            }
        }
    }
}
