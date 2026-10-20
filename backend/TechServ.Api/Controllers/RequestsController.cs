using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechServ.Api.Data;
using TechServ.Api.Dtos;
using TechServ.Api.Extensions;
using TechServ.Api.Models;

namespace TechServ.Api.Controllers;

// Customers only. No token gives 401; an agent's token is valid but has the wrong role, so it gives 403.
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
        // The user ID always comes from the signed token, never from the URL or the body.
        var userId = User.GetUserId();

        // The query is built step by step; EF Core only sends the SQL when the results are needed.
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

        // Status and date are set here by the server, never by the frontend, so nobody can create a request that is already Completed.
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

        // One SaveChanges: the request and its first history row are inserted together.
        _db.ServiceRequests.Add(request);
        await _db.SaveChangesAsync();

        // 201 Created, with a Location header pointing to the new request and its id for the frontend.
        return CreatedAtAction(nameof(GetById), new { id = request.Id }, new { id = request.Id });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = User.GetUserId();

        // Filtering by owner means another customer's request looks exactly like a missing one: 404.
        // This blocks IDOR (changing the id in the URL to see someone else's data).
        // Include loads the related rows in the same query (SQL JOIN): the customer and the history with who made each change.
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

        // The rule lives on the model and is checked here, because anyone can call this endpoint without our frontend.
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

        // Answer with the updated detail, so the page refreshes without a second call.
        return await GetById(id);
    }
}
