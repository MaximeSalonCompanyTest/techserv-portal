using Microsoft.AspNetCore.Identity;
using TechServ.Api.Models;

namespace TechServ.Api.Data;

public static class SeedData
{
    public static void Initialize(AppDbContext db)
    {
        // Only seed an empty database, so restarting the API never creates duplicates.
        if (db.Users.Any())
        {
            return;
        }

        var hasher = new PasswordHasher<User>();
        var now = DateTime.UtcNow;

        var customer = new User
        {
            Name = "Carla Mendes",
            Email = "customer@techserv.com",
            Phone = "11 91234-5678",
            Role = UserRole.Customer,
            CreatedAt = now.AddDays(-10)
        };
        customer.PasswordHash = hasher.HashPassword(customer, "Customer123!");

        // Agent accounts only come from here: sign-up always creates customers.
        var agent = new User
        {
            Name = "Diego Alves",
            Email = "agent@techserv.com",
            Phone = "11 98765-4321",
            Role = UserRole.Agent,
            CreatedAt = now.AddDays(-10)
        };
        agent.PasswordHash = hasher.HashPassword(agent, "Agent123!");

        db.Users.AddRange(customer, agent);

        var completed = NewRequest(customer, "Install printer in reception", "New printer arrived, needs setup on the network.",
            RequestCategory.Installation, RequestPriority.Medium, now.AddDays(-7));
        AddChange(completed, RequestStatus.Open, RequestStatus.InProgress, agent, now.AddDays(-6));
        AddChange(completed, RequestStatus.InProgress, RequestStatus.Completed, agent, now.AddDays(-5));

        var inProgress = NewRequest(customer, "Laptop overheating", "My laptop shuts down after an hour of use.",
            RequestCategory.Maintenance, RequestPriority.High, now.AddDays(-3));
        AddChange(inProgress, RequestStatus.Open, RequestStatus.InProgress, agent, now.AddDays(-2));

        var cancelled = NewRequest(customer, "Reset email password", "Forgot my email password.",
            RequestCategory.Support, RequestPriority.Low, now.AddDays(-2));
        AddChange(cancelled, RequestStatus.Open, RequestStatus.Cancelled, customer, now.AddDays(-2).AddHours(1));

        var open = NewRequest(customer, "VPN not connecting", "The VPN client shows a timeout error from home.",
            RequestCategory.Support, RequestPriority.High, now.AddHours(-5));

        db.ServiceRequests.AddRange(completed, inProgress, cancelled, open);
        db.SaveChanges();
    }

    // Creates an Open request with its first history line, like the create endpoint does.
    private static ServiceRequest NewRequest(User customer, string title, string description,
        RequestCategory category, RequestPriority priority, DateTime createdAt)
    {
        var request = new ServiceRequest
        {
            User = customer,
            Title = title,
            Description = description,
            Category = category,
            Priority = priority,
            Status = RequestStatus.Open,
            CreatedAt = createdAt
        };
        request.StatusChanges.Add(new StatusChange
        {
            FromStatus = null,
            ToStatus = RequestStatus.Open,
            ChangedByUser = customer,
            ChangedAt = createdAt
        });
        return request;
    }

    private static void AddChange(ServiceRequest request, RequestStatus from, RequestStatus to, User changedBy, DateTime changedAt)
    {
        request.StatusChanges.Add(new StatusChange
        {
            FromStatus = from,
            ToStatus = to,
            ChangedByUser = changedBy,
            ChangedAt = changedAt
        });
        request.Status = to;
    }
}
