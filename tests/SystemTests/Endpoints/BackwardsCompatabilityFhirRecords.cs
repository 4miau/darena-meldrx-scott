using System.Net.Http.Headers;
using System.Net.Http.Json;
using MeldRx.Sdk.Dtos;
using MeldRx.Sdk.Fhir;
using Shouldly;
using SystemTests._Fixtures;
using SystemTests.BlueButtonPro.SystemAppTests;
using Task = System.Threading.Tasks.Task;

namespace SystemTests.Endpoints;

public class BackwardsCompatabilityFhirRecords
{
    private readonly SystemAppFixture _systemAppFixture = new(GlobalState.Settings.FhirServerId, GlobalState.Settings.FhirServerName);

    [Fact]
    public async Task fhir_records_endpoint_can_be_reached()
    {
        var token = await _systemAppFixture.TokenProvider.GetAccessTokenAsync();

        var http = new HttpClient();
        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await http.GetAsync($"{GlobalState.Settings.ApiUrl}/FhirServers/records");
        response.IsSuccessStatusCode.ShouldBeTrue(response.StatusCode.ToString());

        var content = await response.Content.ReadFromJsonAsync<List<FhirRecordGrantDto>>(FhirJsonSerializerOptionsFactory.GetFhirJsonSerializerOptions());
        content.ShouldNotBeNull();
        content.Count.ShouldBeGreaterThanOrEqualTo(0);
    }
}
