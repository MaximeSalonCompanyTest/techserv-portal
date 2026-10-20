namespace TechServ.Api.Models;

// One class = one table, one property = one column. EF Core creates the table from this class through the migration.
public class ServiceRequest
{
    public int Id { get; set; } // primary key
    public int UserId { get; set; } // foreign key: the customer who opened the request
    public User? User { get; set; } // navigation property: the customer object, filled when loaded with Include
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
