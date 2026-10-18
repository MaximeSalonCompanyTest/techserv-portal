namespace TechServ.Api.Models;

public class ServiceRequest
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public RequestCategory Category { get; set; }
    public RequestPriority Priority { get; set; }
    public RequestStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<StatusChange> StatusChanges { get; set; } = new();

    // Status rules live here, in one place, so every endpoint applies the same ones.
    public bool CanBeCancelled()
    {
        return Status == RequestStatus.Open;
    }

    // Allowed flow: Open -> InProgress -> Completed, and Open -> Cancelled.
    public bool CanMoveTo(RequestStatus newStatus)
    {
        if (Status == RequestStatus.Open)
        {
            return newStatus == RequestStatus.InProgress || newStatus == RequestStatus.Cancelled;
        }

        if (Status == RequestStatus.InProgress)
        {
            return newStatus == RequestStatus.Completed;
        }

        // Completed and Cancelled are final.
        return false;
    }
}
