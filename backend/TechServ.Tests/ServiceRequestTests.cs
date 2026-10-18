using TechServ.Api.Models;

namespace TechServ.Tests;

public class ServiceRequestTests
{
    [Theory]
    [InlineData(RequestStatus.Open, true)]
    [InlineData(RequestStatus.InProgress, false)]
    [InlineData(RequestStatus.Completed, false)]
    [InlineData(RequestStatus.Cancelled, false)]
    public void CanBeCancelled_IsTrueOnlyWhenOpen(RequestStatus status, bool expected)
    {
        var request = new ServiceRequest { Status = status };

        Assert.Equal(expected, request.CanBeCancelled());
    }

    [Theory]
    // Allowed moves
    [InlineData(RequestStatus.Open, RequestStatus.InProgress, true)]
    [InlineData(RequestStatus.Open, RequestStatus.Cancelled, true)]
    [InlineData(RequestStatus.InProgress, RequestStatus.Completed, true)]
    // Skipping a step, going back, or leaving a final status
    [InlineData(RequestStatus.Open, RequestStatus.Completed, false)]
    [InlineData(RequestStatus.Open, RequestStatus.Open, false)]
    [InlineData(RequestStatus.InProgress, RequestStatus.Open, false)]
    [InlineData(RequestStatus.InProgress, RequestStatus.Cancelled, false)]
    [InlineData(RequestStatus.Completed, RequestStatus.InProgress, false)]
    [InlineData(RequestStatus.Cancelled, RequestStatus.Open, false)]
    public void CanMoveTo_FollowsTheStatusFlow(RequestStatus from, RequestStatus to, bool expected)
    {
        var request = new ServiceRequest { Status = from };

        Assert.Equal(expected, request.CanMoveTo(to));
    }
}
