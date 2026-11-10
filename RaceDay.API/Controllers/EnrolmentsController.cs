
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace RaceDay.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EnrolmentsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EnrolmentsController(RaceDayDbContext context)
        {
            _context = context;
        }

        private int? GetCurrentUserId()
        {
            return int.TryParse(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                out var userId)
                ? userId
                : null;
        }

        // GET: api/Enrolments
        // Organisers can view enrolments for their own events only.
        [HttpGet]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> GetEnrolments()
        {
            var organiserId = GetCurrentUserId();

            if (organiserId == null)
                return Unauthorized();

            var enrolments = await _context.Enrolments
                .Where(e => e.Category.Event.OrganiserId == organiserId.Value)
                .Select(e => new
                {
                    e.EnrolmentId,
                    e.ParticipantId,
                    ParticipantName = e.Participant.FirstName + " " +
                                      e.Participant.LastName,
                    e.CategoryId,
                    CategoryName = e.Category.CategoryName,
                    EventName = e.Category.Event.EventName,
                    e.EnrolmentDate,
                    e.Status
                })
                .ToListAsync();

            return Ok(enrolments);
        }

        // GET: api/Enrolments/1
        // A participant can view their own enrolment.
        // An organiser can view enrolments for their own events.
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetEnrolment(int id)
        {
            var enrolment = await _context.Enrolments
                .Include(e => e.Participant)
                .Include(e => e.Category)
                    .ThenInclude(c => c.Event)
                .FirstOrDefaultAsync(e => e.EnrolmentId == id);

            if (enrolment == null)
                return NotFound(new { Message = "Enrolment not found." });

            var currentUserId = GetCurrentUserId();

            if (currentUserId == null)
                return Unauthorized();

            bool isParticipantOwner =
                User.IsInRole("Participant") &&
                enrolment.ParticipantId == currentUserId.Value;

            bool isEventOrganiser =
                User.IsInRole("Organiser") &&
                enrolment.Category.Event.OrganiserId == currentUserId.Value;

            if (!isParticipantOwner && !isEventOrganiser)
                return Forbid();

            return Ok(new
            {
                enrolment.EnrolmentId,
                enrolment.ParticipantId,
                ParticipantName = enrolment.Participant.FirstName + " " +
                                  enrolment.Participant.LastName,
                enrolment.CategoryId,
                CategoryName = enrolment.Category.CategoryName,
                EventName = enrolment.Category.Event.EventName,
                enrolment.EnrolmentDate,
                enrolment.Status
            });
        }

        // GET: api/Enrolments/participant/1
        // Participants can view their own enrolments only.
        [HttpGet("participant/{participantId:int}")]
        [Authorize(Roles = "Participant")]
        public async Task<IActionResult> GetParticipantEnrolments(
            int participantId)
        {
            var currentUserId = GetCurrentUserId();

            if (currentUserId == null)
                return Unauthorized();

            if (participantId != currentUserId.Value)
                return Forbid();

            var enrolments = await _context.Enrolments
                .Where(e => e.ParticipantId == currentUserId.Value)
                .Select(e => new
                {
                    e.EnrolmentId,
                    e.ParticipantId,
                    e.CategoryId,
                    CategoryName = e.Category.CategoryName,
                    EventName = e.Category.Event.EventName,
                    e.EnrolmentDate,
                    e.Status
                })
                .ToListAsync();

            return Ok(enrolments);
        }

        // POST: api/Enrolments
        // Participants can enrol themselves only.
        [HttpPost]
        [Authorize(Roles = "Participant")]
        public async Task<IActionResult> CreateEnrolment(
            [FromBody] CreateEnrolmentRequest request)
        {
            var participantId = GetCurrentUserId();

            if (participantId == null)
                return Unauthorized();

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryId == request.CategoryId);

            if (category == null)
            {
                return BadRequest(new
                {
                    Message = "Category does not exist."
                });
            }

            var existingEnrolment = await _context.Enrolments
                .AnyAsync(e =>
                    e.ParticipantId == participantId.Value &&
                    e.CategoryId == request.CategoryId &&
                    e.Status != "Cancelled");

            if (existingEnrolment)
            {
                return Conflict(new
                {
                    Message = "You are already enrolled in this category."
                });
            }

            var currentEnrolments = await _context.Enrolments
                .CountAsync(e =>
                    e.CategoryId == request.CategoryId &&
                    e.Status != "Cancelled");

            if (currentEnrolments >= category.MaxParticipants)
            {
                return Conflict(new
                {
                    Message = "This category is full."
                });
            }

            var enrolment = new Enrolment
            {
                ParticipantId = participantId.Value,
                CategoryId = request.CategoryId,
                EnrolmentDate = DateTime.Now,
                Status = "Pending"
            };

            _context.Enrolments.Add(enrolment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetEnrolment),
                new { id = enrolment.EnrolmentId },
                new
                {
                    enrolment.EnrolmentId,
                    enrolment.ParticipantId,
                    enrolment.CategoryId,
                    enrolment.EnrolmentDate,
                    enrolment.Status
                });
        }

        // PUT: api/Enrolments/1/status
        // Only the organiser who owns the event can update enrolment status.
        [HttpPut("{id:int}/status")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> UpdateEnrolmentStatus(
            int id,
            [FromBody] UpdateEnrolmentStatusRequest request)
        {
            var organiserId = GetCurrentUserId();

            if (organiserId == null)
                return Unauthorized();

            var enrolment = await _context.Enrolments
                .Include(e => e.Category)
                    .ThenInclude(c => c.Event)
                .FirstOrDefaultAsync(e => e.EnrolmentId == id);

            if (enrolment == null)
                return NotFound(new { Message = "Enrolment not found." });

            if (enrolment.Category.Event.OrganiserId != organiserId.Value)
                return Forbid();

            var allowedStatuses = new[]
            {
                "Pending",
                "Confirmed",
                "Cancelled"
            };

            if (!allowedStatuses.Contains(
                request.Status,
                StringComparer.OrdinalIgnoreCase))
            {
                return BadRequest(new
                {
                    Message = "Status must be Pending, Confirmed or Cancelled."
                });
            }

            enrolment.Status = allowedStatuses.First(s =>
                s.Equals(request.Status, StringComparison.OrdinalIgnoreCase));

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Enrolment status updated successfully.",
                enrolment.EnrolmentId,
                enrolment.Status
            });
        }

        // DELETE: api/Enrolments/1
        // A participant can delete their own enrolment.
        // An organiser can delete enrolments for their own events.
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteEnrolment(int id)
        {
            var currentUserId = GetCurrentUserId();

            if (currentUserId == null)
                return Unauthorized();

            var enrolment = await _context.Enrolments
                .Include(e => e.Category)
                    .ThenInclude(c => c.Event)
                .FirstOrDefaultAsync(e => e.EnrolmentId == id);

            if (enrolment == null)
                return NotFound(new { Message = "Enrolment not found." });

            bool isParticipantOwner =
                User.IsInRole("Participant") &&
                enrolment.ParticipantId == currentUserId.Value;

            bool isEventOrganiser =
                User.IsInRole("Organiser") &&
                enrolment.Category.Event.OrganiserId == currentUserId.Value;

            if (!isParticipantOwner && !isEventOrganiser)
                return Forbid();

            var hasResults = await _context.Results
                .AnyAsync(r => r.EnrolmentId == id);

            if (hasResults)
            {
                return Conflict(new
                {
                    Message = "This enrolment cannot be deleted because it has a result."
                });
            }

            _context.Enrolments.Remove(enrolment);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Enrolment deleted successfully.",
                EnrolmentId = id
            });
        }
    }

    public class CreateEnrolmentRequest
    {
        [Range(1, int.MaxValue)]
        public int CategoryId { get; set; }
    }

    public class UpdateEnrolmentStatusRequest
    {
        [Required]
        public string Status { get; set; } = string.Empty;
    }
}

