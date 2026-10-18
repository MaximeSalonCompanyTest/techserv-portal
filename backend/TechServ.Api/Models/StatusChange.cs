namespace TechServ.Api.Models;

public class StatusChange
{
    public int Id { get; set; }
    public int ServiceRequestId { get; set; }
    // Null for the row written when the request is created.
    public RequestStatus? FromStatus { get; set; }
    public RequestStatus ToStatus { get; set; }
    public int ChangedByUserId { get; set; }
    public User? ChangedByUser { get; set; }
    public DateTime ChangedAt { get; set; }
}
