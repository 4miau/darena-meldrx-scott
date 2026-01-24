using MeldRx.Sdk.ApiClient.Iam;
using MeldRx.Services.Tests._Fixtures;
using Shouldly;

namespace MeldRx.Integrations;

public class ExternalFhirUrlValidationTest(IntegrationFixture fixture, ITestOutputHelper output) : IntegrationTest(fixture) {

    [Fact]
    public async Task ValidateEpicFhirUrl()
    {
        var client = new FhirApiProviderClient(
            SuperAdminAccessTokenProvider,
            Api,
            MeldRxSdkOptions
        );

        var result = await client.ValidateAsync("https://fhir.epic.com/interconnect-fhir-oauth/api/FHIR/R4/");

        result.Success.ShouldBeTrue();
        var data = result.DeserializedContent;

        data.AuthorizationUrl.ShouldBe("https://fhir.epic.com/interconnect-fhir-oauth/oauth2/authorize");
        data.TokenUrl.ShouldBe("https://fhir.epic.com/interconnect-fhir-oauth/oauth2/token");
    }
}
