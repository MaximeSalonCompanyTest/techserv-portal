namespace TechServ.Api.Models;

// The fixed lists from the test. An enum can only hold one of its listed values, so a typo like "Opne" doesn't compile.
// Saved as text in the database (see AppDbContext) and sent as text in JSON (see Program.cs).

public enum UserRole
{
    Customer,
    Agent
}

public enum RequestCategory
{
    Installation,
    Maintenance,
    Support
}

public enum RequestPriority
{
    Low,
    Medium,
    High
}

public enum RequestStatus
{
    Open,
    InProgress,
    Completed,
    Cancelled
}
