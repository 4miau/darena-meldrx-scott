using Hl7.Fhir.Model;
using MeldRx.Sdk.Dtos;
using Shouldly;
using SystemTests._Fixtures;
using SystemTests.BlueButtonPro.SystemAppTests;
using Task = System.Threading.Tasks.Task;

namespace SystemTests.Endpoints;

public class DirectoryEndpointsTests
{
    private AnonymousUserFixture anonymous = new AnonymousUserFixture(GlobalState.Settings.FhirServerId, GlobalState.Settings.FhirServerName);

    [Fact]
    public async Task access_directory_anonymously()
    {
        var endpoints = await anonymous.Directory.GetEndpoints();
        endpoints.Success.ShouldBeTrue(endpoints.RawResponseContent);
        endpoints.DeserializedContent.ShouldNotBeNull();
        endpoints.DeserializedContent.Entry.Count.ShouldBeGreaterThanOrEqualTo(1);

        var endpoint = endpoints.DeserializedContent
                                .Entry
                                .Select(x => x.Resource)
                                .Cast<Endpoint>()
                                .Single(x => x.Name == GlobalState.Settings.OrganizationName);


        var providers = await anonymous.Directory.GetProviders(new Guid(endpoint.Id));
        providers.Success.ShouldBeTrue(providers.RawResponseContent);
        providers.DeserializedContent.ShouldNotBeNull();
        providers.DeserializedContent.Resources.Count.ShouldBeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task search_directories_anonymously()
    {
        var endpoints = await anonymous.Directory.Search(new DirectorySearchDto()
        {
            Organization = GlobalState.Settings.OrganizationName
        });
        endpoints.Success.ShouldBeTrue(endpoints.RawResponseContent);
        endpoints.DeserializedContent.ShouldNotBeNull();
        endpoints.DeserializedContent.Resources.Count.ShouldBeGreaterThanOrEqualTo(1);
    }
}
