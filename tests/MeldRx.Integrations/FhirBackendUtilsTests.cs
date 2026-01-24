using MeldRx.Sdk.Fhir;
using MeldRx.Services.Tests.MeldRx.Sdk.Tests;

namespace MeldRx.Integrations;

public class FhirBackendUtilsTests
{
    [Fact]
    public async void GetBackendAccessCodeForPrivateKey_Epic_GetsToken()
    {
        var httpClient = new HttpClient();
        var config = _Config.GetConfig().BackendEhrCredentials.EpicBackendSandbox;
        var token = await FhirBackendUtils.GetBackendAccessCodeForPrivateKey(httpClient, config.PrivateKey, config.ClientId, config.TokenUrl);
        Assert.NotNull(token);
        Assert.NotNull(token.AccessToken);
        Assert.NotEmpty(token.AccessToken);
    }

    [Fact]
    public async void GetBackendAccessCodeForPrivateKey_InvalidClientId_ReturnsNull()
    {
        var httpClient = new HttpClient();
        var config = _Config.GetConfig().BackendEhrCredentials.EpicBackendSandbox;
        var token = await FhirBackendUtils.GetBackendAccessCodeForPrivateKey(httpClient, config.PrivateKey, "123", config.TokenUrl);
        Assert.Null(token);
    }

    [Fact]
    public async void GetBackendAccessCodeForJwksUrl_Epic_GetsToken()
    {
        var httpClient = new HttpClient();
        var config = _Config.GetConfig().BackendEhrCredentials.EpicBackendSandbox;
        var token = await FhirBackendUtils.GetBackendAccessCodeForJWKS(httpClient, config.PrivateKey, config.ClientId, config.TokenUrl, config.Kid, config.Alg);
        Assert.NotNull(token);
        Assert.NotNull(token.AccessToken);
        Assert.NotEmpty(token.AccessToken);
    }

    [Fact]
    public async void GetBackendAccessCodeForJwksUrl_InvalidClientId_ReturnsNull()
    {
        var httpClient = new HttpClient();
        var config = _Config.GetConfig().BackendEhrCredentials.EpicBackendSandbox;
        var token = await FhirBackendUtils.GetBackendAccessCodeForJWKS(httpClient, config.PrivateKey, "123", config.TokenUrl, config.Kid, config.Alg);
        Assert.Null(token);
    }

    [Fact]
    public async void GetBackendAccessCodeForClientSecret_Cerner_GetsToken()
    {
        var httpClient = new HttpClient();
        var config = _Config.GetConfig().BackendEhrCredentials.CernerBackendSandbox;
        var token = await FhirBackendUtils.GetBackendAccessCodeForClientSecret(httpClient, config.ClientId, config.ClientSecret, config.TokenUrl, config.Scope);
        Assert.NotNull(token);
        Assert.NotNull(token.AccessToken);
        Assert.NotEmpty(token.AccessToken);
    }

    [Fact]
    public async void GetBackendAccessCodeForClientSecret_AthenaHealth_GetsToken()
    {
        var httpClient = new HttpClient();
        var config = _Config.GetConfig().BackendEhrCredentials.AthenaBackendSandbox;
        var token = await FhirBackendUtils.GetBackendAccessCodeForClientSecret(httpClient, config.ClientId, config.ClientSecret, config.TokenUrl, config.Scope);
        Assert.NotNull(token);
        Assert.NotNull(token.AccessToken);
        Assert.NotEmpty(token.AccessToken);
    }
}
