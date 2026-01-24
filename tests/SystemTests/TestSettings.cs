namespace SystemTests;

public class TestSettings
{
    public string AuthorityUrl { get; set; } = "";
    public string ApiUrl { get; set; } = "";
    public string SuperAdminToken { get; set; } = "";
    public string AppClientId { get; set; } = "";
    public string AppClientSecret { get; set; } = "";
    public Guid FhirServerId { get; set; }
    public string FhirServerName { get; set; } = "";
    public string OrganizationName { get; set; } = "";
}
