namespace MeldRx.Services.Api.Auth.Policies
{
    /// <summary>
    /// An authorization requirement that ensures a user has access to the
    /// fhir record specified in a header
    /// </summary>
    public class FhirRecordRequirement : IAuthorizationRequirement
    {
    }
}
