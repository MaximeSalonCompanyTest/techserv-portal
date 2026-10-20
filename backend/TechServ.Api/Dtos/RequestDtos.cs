using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using TechServ.Api.Models;

namespace TechServ.Api.Dtos;

// The "Dto" suffix keeps these names apart from the ServiceRequest entity.

public class CreateServiceRequestDto
{
    [Required, MaxLength(150)]
    public string Title { get; set; } = "";

    [Required, MaxLength(2000)]
    public string Description { get; set; } = "";

    // Nullable so that a missing value fails [Required] instead of silently becoming the first enum value.
    [Required, EnumDataType(typeof(RequestCategory))]
    public RequestCategory? Category { get; set; }

    [Required, EnumDataType(typeof(RequestPriority))]
    public RequestPriority? Priority { get; set; }
}

public class ChangeStatusDto
{
    [Required, EnumDataType(typeof(RequestStatus))]
    public RequestStatus? Status { get; set; }
}

public class ServiceRequestListItemDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public RequestCategory Category { get; set; }
    public RequestPriority Priority { get; set; }
    public RequestStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CustomerName { get; set; } = "";
}

public class PagedRequestsDto
{
    public List<ServiceRequestListItemDto> Items { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }

    // Shared by the customer and agent lists: they only differ in how the query is filtered.
    public static async Task<PagedRequestsDto> FromQueryAsync(IQueryable<ServiceRequest> query, int page, int pageSize)
    {
        // Safety limits, so nobody can ask for page 0 or a million rows at once.
        if (page < 1)
        {
            page = 1;
        }
        if (pageSize < 1 || pageSize > 50)
        {
            pageSize = 10;
        }

        // The total lets the frontend know how many pages exist.
        var totalCount = await query.CountAsync();

        // Newest first. Page 2 with size 10 skips 10 rows and takes 10 (SQL OFFSET 10 LIMIT 10).
        // Select reads only the columns the list needs.
        var items = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(r => new ServiceRequestListItemDto
            {
                Id = r.Id,
                Title = r.Title,
                Category = r.Category,
                Priority = r.Priority,
                Status = r.Status,
                CreatedAt = r.CreatedAt,
                CustomerName = r.User!.Name
            })
            .ToListAsync();

        return new PagedRequestsDto
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}

public class StatusChangeDto
{
    public RequestStatus? FromStatus { get; set; }
    public RequestStatus ToStatus { get; set; }
    public string ChangedByName { get; set; } = "";
    public DateTime ChangedAt { get; set; }
}

public class ServiceRequestDetailDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public RequestCategory Category { get; set; }
    public RequestPriority Priority { get; set; }
    public RequestStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CustomerName { get; set; } = "";
    public List<StatusChangeDto> History { get; set; } = new();

    // Shared by the customer and agent detail endpoints.
    // Expects User and StatusChanges.ChangedByUser to be loaded with Include.
    public static ServiceRequestDetailDto FromEntity(ServiceRequest request)
    {
        return new ServiceRequestDetailDto
        {
            Id = request.Id,
            Title = request.Title,
            Description = request.Description,
            Category = request.Category,
            Priority = request.Priority,
            Status = request.Status,
            CreatedAt = request.CreatedAt,
            CustomerName = request.User!.Name,
            History = request.StatusChanges
                .OrderBy(s => s.ChangedAt)
                .Select(s => new StatusChangeDto
                {
                    FromStatus = s.FromStatus,
                    ToStatus = s.ToStatus,
                    ChangedByName = s.ChangedByUser!.Name,
                    ChangedAt = s.ChangedAt
                })
                .ToList()
        };
    }
}
