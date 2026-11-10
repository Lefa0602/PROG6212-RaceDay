
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
    public class CategoriesController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public CategoriesController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/Categories
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Categories
                .Select(c => new
                {
                    c.CategoryId,
                    c.EventId,
                    EventName = c.Event.EventName,
                    c.CategoryName,
                    c.DistanceKm,
                    c.MaxParticipants,
                    c.EntryFee
                })
                .ToListAsync();

            return Ok(categories);
        }

        // GET: api/Categories/1
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _context.Categories
                .Where(c => c.CategoryId == id)
                .Select(c => new
                {
                    c.CategoryId,
                    c.EventId,
                    EventName = c.Event.EventName,
                    c.CategoryName,
                    c.DistanceKm,
                    c.MaxParticipants,
                    c.EntryFee
                })
                .FirstOrDefaultAsync();

            if (category == null)
            {
                return NotFound(new { Message = "Category not found." });
            }

            return Ok(category);
        }

        // GET: api/Categories/event/1
        [HttpGet("event/{eventId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategoriesByEvent(int eventId)
        {
            var eventExists = await _context.Events
                .AnyAsync(e => e.EventId == eventId);

            if (!eventExists)
            {
                return NotFound(new { Message = "Event not found." });
            }

            var categories = await _context.Categories
                .Where(c => c.EventId == eventId)
                .Select(c => new
                {
                    c.CategoryId,
                    c.EventId,
                    c.CategoryName,
                    c.DistanceKm,
                    c.MaxParticipants,
                    c.EntryFee
                })
                .ToListAsync();

            return Ok(categories);
        }

        // POST: api/Categories
        // Only an Organiser can create categories for their own events.
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> CreateCategory(
            [FromBody] CreateCategoryRequest request)
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

            var eventItem = await _context.Events
                .FirstOrDefaultAsync(e => e.EventId == request.EventId);

            if (eventItem == null)
            {
                return BadRequest(new
                {
                    Message = "The selected event does not exist."
                });
            }

            if (eventItem.OrganiserId != organiserId)
            {
                return Forbid();
            }

            var category = new Category
            {
                EventId = request.EventId,
                CategoryName = request.CategoryName,
                DistanceKm = request.DistanceKm,
                MaxParticipants = request.MaxParticipants,
                EntryFee = request.EntryFee
            };

            _context.Categories.Add(category);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetCategory),
                new { id = category.CategoryId },
                new
                {
                    category.CategoryId,
                    category.EventId,
                    category.CategoryName,
                    category.DistanceKm,
                    category.MaxParticipants,
                    category.EntryFee
                });
        }

        // PUT: api/Categories/1
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> UpdateCategory(
            int id,
            [FromBody] UpdateCategoryRequest request)
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

            var category = await _context.Categories
                .Include(c => c.Event)
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null)
            {
                return NotFound(new { Message = "Category not found." });
            }

            if (category.Event.OrganiserId != organiserId)
            {
                return Forbid();
            }

            category.CategoryName = request.CategoryName;
            category.DistanceKm = request.DistanceKm;
            category.MaxParticipants = request.MaxParticipants;
            category.EntryFee = request.EntryFee;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Category updated successfully.",
                category.CategoryId,
                category.EventId,
                category.CategoryName,
                category.DistanceKm,
                category.MaxParticipants,
                category.EntryFee
            });
        }

        // DELETE: api/Categories/1
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> DeleteCategory(int id)
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

            var category = await _context.Categories
                .Include(c => c.Event)
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null)
            {
                return NotFound(new { Message = "Category not found." });
            }

            if (category.Event.OrganiserId != organiserId)
            {
                return Forbid();
            }

            var hasEnrolments = await _context.Enrolments
                .AnyAsync(e => e.CategoryId == id);

            if (hasEnrolments)
            {
                return Conflict(new
                {
                    Message = "This category cannot be deleted because it has existing enrolments."
                });
            }

            _context.Categories.Remove(category);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Category deleted successfully.",
                CategoryId = id
            });
        }
    }

    public class CreateCategoryRequest
    {
        [Required]
        public int EventId { get; set; }

        [Required]
        public string CategoryName { get; set; } = string.Empty;

        [Range(typeof(decimal), "0.01", "1000")]
        public decimal DistanceKm { get; set; }

        [Range(1, 100000)]
        public int MaxParticipants { get; set; }

        [Range(typeof(decimal), "0", "100000")]
        public decimal EntryFee { get; set; }
    }

    public class UpdateCategoryRequest
    {
        [Required]
        public string CategoryName { get; set; } = string.Empty;

        [Range(typeof(decimal), "0.01", "1000")]
        public decimal DistanceKm { get; set; }

        [Range(1, 100000)]
        public int MaxParticipants { get; set; }

        [Range(typeof(decimal), "0", "100000")]
        public decimal EntryFee { get; set; }
    }
}

