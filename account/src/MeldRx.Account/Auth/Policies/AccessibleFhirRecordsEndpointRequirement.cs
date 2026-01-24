namespace MeldRx.Services.Api.Auth.Policies
{
    /// <summary>
    /// The requirement that will ensure a user has access to the accessible fhir records
    /// endpoint
    /// </summary>
    public class AccessibleFhirRecordsEndpointRequirement : IAuthorizationRequirement
    {
    }
}