namespace TechServ.Api.Models;

// One line of a request's history: from which status, to which status, who and when.
// A new row is added in the same SaveChanges as every status change, so the history can't drift out of sync.
public class StatusChange
{
    public int Id { get; set; }
    public int ServiceRequestId { get; set; } // foreign key: the request this line belongs to
    // Null for the row written when the request is created.
    public RequestStatus? FromStatus { get; set; }
    public RequestStatus ToStatus { get; set; }
    public int ChangedByUserId { get; set; } // foreign key: the customer or agent who made the change
    public User? ChangedByUser { get; set; }
    public DateTime ChangedAt { get; set; }
}
