
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;

namespace RaceDay.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DatabaseTestController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public DatabaseTestController(RaceDayDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userCount = await _context.Users.CountAsync();
            var eventCount = await _context.Events.CountAsync();

            return Ok(new
            {
                Message = "RaceDay database connection successful!",
                TotalUsers = userCount,
                TotalEvents = eventCount
            });
        }
    }
}
