namespace MeldRx.Services.Api.Auth.Policies
{
    /// <summary>
    /// The requirement that will ensure that a user has access to the fhir endpoints
    /// </summary>
    public class FhirEndpointsRequirement : IAuthorizationRequirement
    {
    }
}