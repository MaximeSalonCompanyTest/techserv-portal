using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechServ.Api.Data;
using TechServ.Api.Dtos;
using TechServ.Api.Extensions;
using TechServ.Api.Models;

namespace TechServ.Api.Controllers;

// Agents only. A customer's token is valid but has the wrong role, so it gives 403.
[ApiController]
[Route("api/agent")]
[Authorize(Roles = "Agent")]
public class AgentController : ControllerBase
{
    private readonly AppDbContext _db;

    public AgentController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("requests")]
    public async Task<IActionResult> GetAll(RequestStatus? status, int page = 1, int pageSize = 10)
    {
        // Same list as the customer's, without the owner filter: agents see every request.
        IQueryable<ServiceRequest> query = _db.ServiceRequests;
        if (status != null)
        {
            query = query.Where(r => r.Status == status);
        }

        return Ok(await PagedRequestsDto.FromQueryAsync(query, page, pageSize));
    }

    [HttpGet("requests/{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        // Same as the customer detail, without the owner filter: an agent can open any request.
        var request = await _db.ServiceRequests
            .Include(r => r.User)
            .Include(r => r.StatusChanges).ThenInclude(s => s.ChangedByUser)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (request == null)
        {
            return NotFound();
        }

        return Ok(ServiceRequestDetailDto.FromEntity(request));
    }

    [HttpPut("requests/{id}/status")]
    public async Task<IActionResult> ChangeStatus(int id, ChangeStatusDto dto)
    {
        var request = await _db.ServiceRequests.FirstOrDefaultAsync(r => r.Id == id);
        if (request == null)
        {
            return NotFound();
        }

        // CanMoveTo enforces the flow Open -> InProgress -> Completed, or Open -> Cancelled. Anything else is a 400.
        var newStatus = dto.Status!.Value;
        if (!request.CanMoveTo(newStatus))
        {
            return BadRequest(new { message = $"Cannot change status from {request.Status} to {newStatus}." });
        }

        request.StatusChanges.Add(new StatusChange
        {
            FromStatus = request.Status,
            ToStatus = newStatus,
            ChangedByUserId = User.GetUserId(), // the agent's ID from the token, so the history shows who made the change
            ChangedAt = DateTime.UtcNow
        });
        request.Status = newStatus;

        // One SaveChanges: the new status and its history row are saved together or not at all.
        await _db.SaveChangesAsync();

        return await GetById(id);
    }
}
