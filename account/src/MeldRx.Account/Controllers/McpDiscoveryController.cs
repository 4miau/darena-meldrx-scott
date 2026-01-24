#nullable enable
namespace MeldRx.Account.Controllers;

[ApiController]
public class McpDiscoveryController : ControllerBase
{
    // Route needs to be a base level route as specified in the specs: https://modelcontextprotocol.io/specification/2025-03-26/basic/authorization#2-3-2-authorization-base-url
    [HttpGet("/.well-known/oauth-authorization-server")]
    [AllowAnonymous]
    public async Task<ActionResult<Dictionary<string, object>>> GetDiscoveryDocumentAsync(
        [FromServices] IDiscoveryResponseGenerator discoveryResponseGenerator,
        [FromServices] IIssuerNameService issuerNameService,
        [FromServices] IServerUrls urls
    )
    {
        // Header needs to be set as specified in the specs: https://modelcontextprotocol.io/specification/2025-03-26/basic/authorization#2-3-1-server-metadata-discovery-headers
        Response.Headers.Append("MCP-Protocol-Version", "2025-03-26");

        var issuerUri = await issuerNameService.GetCurrentAsync();
        var baseUrl = urls.BaseUrl;
        return await discoveryResponseGenerator.CreateDiscoveryDocumentAsync(baseUrl, issuerUri);
    }
}
