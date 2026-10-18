using System.ComponentModel.DataAnnotations;
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
