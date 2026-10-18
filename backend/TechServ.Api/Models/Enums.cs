namespace TechServ.Api.Models;

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
