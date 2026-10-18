using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechServ.Api.Data;
using TechServ.Api.Dtos;
using TechServ.Api.Extensions;
using TechServ.Api.Models;

namespace TechServ.Api.Controllers;

[ApiController]
[Route("api/requests")]
[Authorize(Roles = "Customer")]
public class RequestsController : ControllerBase
{
    private readonly AppDbContext _db;

    public RequestsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetMine(RequestStatus? status, int page = 1, int pageSize = 10)
    {
        var userId = User.GetUserId();

        var query = _db.ServiceRequests.Where(r => r.UserId == userId);
        if (status != null)
        {
            query = query.Where(r => r.Status == status);
        }

        return Ok(await PagedRequestsDto.FromQueryAsync(query, page, pageSize));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateServiceRequestDto dto)
    {
        var userId = User.GetUserId();
        var now = DateTime.UtcNow;

        var request = new ServiceRequest
        {
            UserId = userId,
            Title = dto.Title.Trim(),
            Description = dto.Description.Trim(),
            Category = dto.Category!.Value,
            Priority = dto.Priority!.Value,
            Status = RequestStatus.Open,
            CreatedAt = now
        };

        // The creation is the first line of the status history.
        request.StatusChanges.Add(new StatusChange
        {
            FromStatus = null,
            ToStatus = RequestStatus.Open,
            ChangedByUserId = userId,
            ChangedAt = now
        });

        _db.ServiceRequests.Add(request);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = request.Id }, new { id = request.Id });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = User.GetUserId();

        // Filtering by owner means another customer's request looks exactly like a missing one: 404.
        var request = await _db.ServiceRequests
            .Include(r => r.User)
            .Include(r => r.StatusChanges).ThenInclude(s => s.ChangedByUser)
            .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

        if (request == null)
        {
            return NotFound();
        }

        return Ok(ServiceRequestDetailDto.FromEntity(request));
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        var userId = User.GetUserId();

        var request = await _db.ServiceRequests
            .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

        if (request == null)
        {
            return NotFound();
        }

        if (!request.CanBeCancelled())
        {
            return BadRequest(new { message = "Only open requests can be cancelled." });
        }

        request.StatusChanges.Add(new StatusChange
        {
            FromStatus = request.Status,
            ToStatus = RequestStatus.Cancelled,
            ChangedByUserId = userId,
            ChangedAt = DateTime.UtcNow
        });
        request.Status = RequestStatus.Cancelled;

        // One SaveChanges: the new status and its history row are saved together or not at all.
        await _db.SaveChangesAsync();

        return await GetById(id);
    }
}
