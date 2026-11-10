
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
    [Authorize]
    public class ResultsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public ResultsController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/Results
        // Participants see only their own results.
        // Organisers see results for their own events.
        [HttpGet]
        public async Task<IActionResult> GetResults()
        {
            if (!int.TryParse(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                out int userId))
            {
                return Unauthorized();
            }

            var query = _context.Results.AsQueryable();

            if (User.IsInRole("Participant"))
            {
                query = query.Where(r =>
                    r.Enrolment.ParticipantId == userId);
            }
            else if (User.IsInRole("Organiser"))
            {
                query = query.Where(r =>
                    r.Enrolment.Category.Event.OrganiserId == userId);
            }
            else
            {
                return Forbid();
            }

            var results = await query
                .Select(r => new
                {
                    r.ResultId,
                    r.EnrolmentId,
                    r.Enrolment.ParticipantId,
                    ParticipantName = r.Enrolment.Participant.FirstName
                        + " " + r.Enrolment.Participant.LastName,
                    EventName = r.Enrolment.Category.Event.EventName,
                    CategoryName = r.Enrolment.Category.CategoryName,
                    r.FinishTime,
                    r.Position,
                    r.ResultStatus,
                    r.RecordedAt
                })
                .ToListAsync();

            return Ok(results);
        }

        // GET: api/Results/1
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetResult(int id)
        {
            if (!int.TryParse(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                out int userId))
            {
                return Unauthorized();
            }

            var query = _context.Results
                .Where(r => r.ResultId == id);

            if (User.IsInRole("Participant"))
            {
                query = query.Where(r =>
                    r.Enrolment.ParticipantId == userId);
            }
            else if (User.IsInRole("Organiser"))
            {
                query = query.Where(r =>
                    r.Enrolment.Category.Event.OrganiserId == userId);
            }
            else
            {
                return Forbid();
            }

            var result = await query
                .Select(r => new
                {
                    r.ResultId,
                    r.EnrolmentId,
                    r.Enrolment.ParticipantId,
                    ParticipantName = r.Enrolment.Participant.FirstName
                        + " " + r.Enrolment.Participant.LastName,
                    EventName = r.Enrolment.Category.Event.EventName,
                    CategoryName = r.Enrolment.Category.CategoryName,
                    r.FinishTime,
                    r.Position,
                    r.ResultStatus,
                    r.RecordedAt
                })
                .FirstOrDefaultAsync();

            if (result == null)
            {
                return NotFound(new
                {
                    Message = "Result not found."
                });
            }

            return Ok(result);
        }

        // GET: api/Results/participant/2
        [HttpGet("participant/{participantId:int}")]
        public async Task<IActionResult> GetParticipantResults(int participantId)
        {
            if (!int.TryParse(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                out int userId))
            {
                return Unauthorized();
            }

            var participantExists = await _context.Users
                .AnyAsync(u =>
                    u.UserId == participantId &&
                    u.Role == "Participant");

            if (!participantExists)
            {
                return NotFound(new
                {
                    Message = "Participant not found."
                });
            }

            var query = _context.Results
                .Where(r => r.Enrolment.ParticipantId == participantId);

            if (User.IsInRole("Participant"))
            {
                if (userId != participantId)
                {
                    return Forbid();
                }
            }
            else if (User.IsInRole("Organiser"))
            {
                query = query.Where(r =>
                    r.Enrolment.Category.Event.OrganiserId == userId);
            }
            else
            {
                return Forbid();
            }

            var results = await query
                .Select(r => new
                {
                    r.ResultId,
                    r.EnrolmentId,
                    EventName = r.Enrolment.Category.Event.EventName,
                    CategoryName = r.Enrolment.Category.CategoryName,
                    r.FinishTime,
                    r.Position,
                    r.ResultStatus,
                    r.RecordedAt
                })
                .ToListAsync();

            return Ok(results);
        }

        // POST: api/Results
        // Only organisers can record results for their own events.
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> CreateResult(
            [FromBody] CreateResultRequest request)
        {
            if (!int.TryParse(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                out int organiserId))
            {
                return Unauthorized();
            }

            var enrolment = await _context.Enrolments
                .Include(e => e.Category)
                    .ThenInclude(c => c.Event)
                .FirstOrDefaultAsync(e =>
                    e.EnrolmentId == request.EnrolmentId);

            if (enrolment == null)
            {
                return BadRequest(new
                {
                    Message = "Enrolment does not exist."
                });
            }

            if (enrolment.Category.Event.OrganiserId != organiserId)
            {
                return Forbid();
            }

            if (enrolment.Status != "Confirmed")
            {
                return BadRequest(new
                {
                    Message = "A result can only be recorded for a confirmed enrolment."
                });
            }

            var existingResult = await _context.Results
                .AnyAsync(r => r.EnrolmentId == request.EnrolmentId);

            if (existingResult)
            {
                return Conflict(new
                {
                    Message = "A result already exists for this enrolment."
                });
            }

            var allowedStatuses = new[]
            {
                "Finished",
                "Disqualified",
                "Did Not Finish"
            };

            if (!allowedStatuses.Contains(
                request.ResultStatus,
                StringComparer.OrdinalIgnoreCase))
            {
                return BadRequest(new
                {
                    Message = "Result status must be Finished, Disqualified or Did Not Finish."
                });
            }

            if (request.ResultStatus.Equals(
                "Finished",
                StringComparison.OrdinalIgnoreCase))
            {
                if (request.FinishTime == null ||
                    request.Position == null)
                {
                    return BadRequest(new
                    {
                        Message = "Finish time and position are required for a finished result."
                    });
                }
            }

            var result = new Result
            {
                EnrolmentId = request.EnrolmentId,
                FinishTime = request.FinishTime,
                Position = request.Position,
                ResultStatus = allowedStatuses.First(s =>
                    s.Equals(
                        request.ResultStatus,
                        StringComparison.OrdinalIgnoreCase)),
                RecordedAt = DateTime.Now
            };

            _context.Results.Add(result);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetResult),
                new { id = result.ResultId },
                new
                {
                    result.ResultId,
                    result.EnrolmentId,
                    result.FinishTime,
                    result.Position,
                    result.ResultStatus,
                    result.RecordedAt
                });
        }

        // PUT: api/Results/1
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> UpdateResult(
            int id,
            [FromBody] UpdateResultRequest request)
        {
            if (!int.TryParse(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                out int organiserId))
            {
                return Unauthorized();
            }

            var result = await _context.Results
                .Include(r => r.Enrolment)
                    .ThenInclude(e => e.Category)
                        .ThenInclude(c => c.Event)
                .FirstOrDefaultAsync(r => r.ResultId == id);

            if (result == null)
            {
                return NotFound(new
                {
                    Message = "Result not found."
                });
            }

            if (result.Enrolment.Category.Event.OrganiserId != organiserId)
            {
                return Forbid();
            }

            var allowedStatuses = new[]
            {
                "Finished",
                "Disqualified",
                "Did Not Finish"
            };

            if (!allowedStatuses.Contains(
                request.ResultStatus,
                StringComparer.OrdinalIgnoreCase))
            {
                return BadRequest(new
                {
                    Message = "Result status must be Finished, Disqualified or Did Not Finish."
                });
            }

            if (request.ResultStatus.Equals(
                "Finished",
                StringComparison.OrdinalIgnoreCase))
            {
                if (request.FinishTime == null ||
                    request.Position == null)
                {
                    return BadRequest(new
                    {
                        Message = "Finish time and position are required for a finished result."
                    });
                }
            }

            result.FinishTime = request.FinishTime;
            result.Position = request.Position;
            result.ResultStatus = allowedStatuses.First(s =>
                s.Equals(
                    request.ResultStatus,
                    StringComparison.OrdinalIgnoreCase));
            result.RecordedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Result updated successfully.",
                result.ResultId,
                result.EnrolmentId,
                result.FinishTime,
                result.Position,
                result.ResultStatus,
                result.RecordedAt
            });
        }

        // DELETE: api/Results/1
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> DeleteResult(int id)
        {
            if (!int.TryParse(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                out int organiserId))
            {
                return Unauthorized();
            }

            var result = await _context.Results
                .Include(r => r.Enrolment)
                    .ThenInclude(e => e.Category)
                        .ThenInclude(c => c.Event)
                .FirstOrDefaultAsync(r => r.ResultId == id);

            if (result == null)
            {
                return NotFound(new
                {
                    Message = "Result not found."
                });
            }

            if (result.Enrolment.Category.Event.OrganiserId != organiserId)
            {
                return Forbid();
            }

            _context.Results.Remove(result);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Result deleted successfully.",
                ResultId = id
            });
        }
    }

    public class CreateResultRequest
    {
        [Range(1, int.MaxValue)]
        public int EnrolmentId { get; set; }

        public TimeOnly? FinishTime { get; set; }

        [Range(1, int.MaxValue)]
        public int? Position { get; set; }

        [Required]
        public string ResultStatus { get; set; } = string.Empty;
    }

    public class UpdateResultRequest
    {
        public TimeOnly? FinishTime { get; set; }

        [Range(1, int.MaxValue)]
        public int? Position { get; set; }

        [Required]
        public string ResultStatus { get; set; } = string.Empty;
    }
}
