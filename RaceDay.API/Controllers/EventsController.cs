
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace RaceDay.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EventsController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/Events
        // Public access: everyone can view events.
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetEvents()
        {
            var events = await _context.Events
                .Select(e => new
                {
                    e.EventId,
                    e.OrganiserId,
                    e.EventName,
                    e.Description,
                    e.EventDate,
                    e.Location,
                    e.EventType,
                    e.CreatedAt,

                    Categories = e.Categories.Select(c => new
                    {
                        c.CategoryId,
                        c.CategoryName,
                        c.DistanceKm,
                        c.MaxParticipants,
                        c.EntryFee
                    }).ToList()
                })
                .ToListAsync();

            return Ok(events);
        }

        // GET: api/Events/1
        // Public access: everyone can view event details.
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetEvent(int id)
        {
            var eventItem = await _context.Events
                .Where(e => e.EventId == id)
                .Select(e => new
                {
                    e.EventId,
                    e.OrganiserId,
                    e.EventName,
                    e.Description,
                    e.EventDate,
                    e.Location,
                    e.EventType,
                    e.CreatedAt,

                    Categories = e.Categories.Select(c => new
                    {
                        c.CategoryId,
                        c.CategoryName,
                        c.DistanceKm,
                        c.MaxParticipants,
                        c.EntryFee
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (eventItem == null)
            {
                return NotFound(new { Message = "Event not found." });
            }

            return Ok(eventItem);
        }

        // POST: api/Events
        // Only Organisers can create events.
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> CreateEvent(
            [FromBody] CreateEventRequest request)
        {
            var userIdValue = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdValue, out var organiserId))
            {
                return Unauthorized(new
                {
                    Message = "Invalid organiser authentication."
                });
            }

            var newEvent = new Event
            {
                OrganiserId = organiserId,
                EventName = request.EventName,
                Description = request.Description,
                EventDate = request.EventDate,
                Location = request.Location,
                EventType = request.EventType,
                CreatedAt = DateTime.Now
            };

            _context.Events.Add(newEvent);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetEvent),
                new { id = newEvent.EventId },
                new
                {
                    newEvent.EventId,
                    newEvent.OrganiserId,
                    newEvent.EventName,
                    newEvent.Description,
                    newEvent.EventDate,
                    newEvent.Location,
                    newEvent.EventType,
                    newEvent.CreatedAt
                });
        }

        // PUT: api/Events/1
        // Only the event's Organiser can update it.
        [HttpPut("{id}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> UpdateEvent(
            int id,
            [FromBody] UpdateEventRequest request)
        {
            var userIdValue = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdValue, out var organiserId))
            {
                return Unauthorized(new
                {
                    Message = "Invalid organiser authentication."
                });
            }

            var eventItem = await _context.Events.FindAsync(id);

            if (eventItem == null)
            {
                return NotFound(new { Message = "Event not found." });
            }

            if (eventItem.OrganiserId != organiserId)
            {
                return Forbid();
            }

            eventItem.EventName = request.EventName;
            eventItem.Description = request.Description;
            eventItem.EventDate = request.EventDate;
            eventItem.Location = request.Location;
            eventItem.EventType = request.EventType;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Event updated successfully.",
                eventItem.EventId,
                eventItem.OrganiserId,
                eventItem.EventName,
                eventItem.Description,
                eventItem.EventDate,
                eventItem.Location,
                eventItem.EventType
            });
        }

        // DELETE: api/Events/1
        // Only the event's Organiser can delete it.
        [HttpDelete("{id}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var userIdValue = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdValue, out var organiserId))
            {
                return Unauthorized(new
                {
                    Message = "Invalid organiser authentication."
                });
            }

            var eventItem = await _context.Events.FindAsync(id);

            if (eventItem == null)
            {
                return NotFound(new { Message = "Event not found." });
            }

            if (eventItem.OrganiserId != organiserId)
            {
                return Forbid();
            }

            _context.Events.Remove(eventItem);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Event deleted successfully.",
                EventId = id
            });
        }
    }

    public class CreateEventRequest
    {
        [Required]
        public string EventName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required]
        public DateOnly EventDate { get; set; }

        [Required]
        public string Location { get; set; } = string.Empty;

        [Required]
        public string EventType { get; set; } = string.Empty;
    }

    public class UpdateEventRequest
    {
        [Required]
        public string EventName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required]
        public DateOnly EventDate { get; set; }

        [Required]
        public string Location { get; set; } = string.Empty;

        [Required]
        public string EventType { get; set; } = string.Empty;
    }
}

