using System.Text.Json;
using Hl7.Fhir.Model;
using MeldRx.Sdk.ApiClient;
using MeldRx.Sdk.ApiClient.MeldRxApi;
using MeldRx.Sdk.Dtos;
using MeldRx.Sdk.Enums;
using MeldRx.Sdk.Extensions;
using MeldRx.Sdk.Fhir;
using MeldRx.Sdk.Fhir.SmartOnFhir;
using MeldRx.Sdk.Internal.Services;
using MeldRx.Sdk.Utilities;
using MeldRx.Services.Shared;
using MeldRx.Services.Tests._Fixtures;
using MeldRx.Services.Tests._Fixtures.MockServices;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Task = System.Threading.Tasks.Task;

namespace MeldRx.Integrations;

public class SmartHealthItLinkedWorkspaceTests(IntegrationFixture fixture, ITestOutputHelper output) : LinkedWorkspaceTest(fixture, output)
{
    protected override CreateDeveloperWorkspaceCommand CreateWorkspaceCommand => new CreateDeveloperWorkspaceCommand("SMART-Health-IT")
    {
        LinkedFhirApi = new CreateDeveloperWorkspaceCommand.LinkedFhirApiForm
        {
            FhirApiProviderMeldRxIdentifier = DefaultFhirApiProvider.MeldRxIdentifier,
            BaseUrl = "https://launch.smarthealthit.org/v/r4/fhir",
        }
    };

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        var distributedCache = Services.GetRequiredService<IDistributedCacheManager>();
        var cacheKey = CacheKeys.CreateLinkedApiTokenCacheKey(
            MockConstants.ClientId,
            WorkspaceUserAccessToken.ExternalApiSubjectId,
            Workspace.LinkedFhirApiDto!.Id.ToString()
        );

        await distributedCache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(new SmartAuthResponse
            {
                AccessToken = "",
                IdToken = "id_token",
                PatientId = "dummy-patient-id"
            })
        );
    }

    [Fact]
    public async Task SearchPatientByName_FindsResults()
    {
        var fhirLiteClient = new FhirClientLite(WorkspaceUserAccessToken, Api, Api.BaseAddress.GetBlueButtonProWorkspaceUrl(Workspace.Id.ToString()));

        //  Perform the search...
        var searchResult = await fhirLiteClient.Search<Patient>(new[] { "given=gerardo" });
        var bundle = searchResult.Data;
        var entries = bundle.Entry;
        var firstEntry = entries.FirstOrDefault()?.Resource as Patient;

        // Verify we found the correct result...
        Assert.Equal(1, entries.Count); // Should only have one result
        Assert.Equal("8de3051f-6298-43e6-9b7f-2aa6443ee760", firstEntry!.Id);
        Assert.Equal("2010-09-07", firstEntry.BirthDate);
        Assert.Equal("Botello", firstEntry.Name[0].Family);
    }

    [Fact]
    public async Task CanReadPatientById_FindsPatient()
    {
        var fhirLiteClient = new FhirClientLite(WorkspaceUserAccessToken, Api, Api.BaseAddress.GetBlueButtonProWorkspaceUrl(Workspace.Id.ToString()));

        var workspacesCleint = new WorkspacesClient(
            DeveloperOrganizationUserAccessToken,
            Api,
            new MeldRxSdkOptions(Api.BaseAddress.GetBaseUrlWithSuffix("api"))
        );

        var result = await workspacesCleint.CreateDataRule(
            Workspace.Id,
            new(LinkedApiAction.Read, ResourceType.Patient, LinkedApiActionTarget.External, Workspace.LinkedFhirApiDto.Id, null));

        result.Success.ShouldBeTrue(result.RawResponseContent);

        // Read a patient by ID...
        const string patientIdGerardoBotello = "8de3051f-6298-43e6-9b7f-2aa6443ee760";
        var readResult = await fhirLiteClient.GetById<Patient>(patientIdGerardoBotello);
        Assert.Equal(patientIdGerardoBotello, readResult.Data?.Id);
    }

    [Fact]
    public async Task SearchPatientResources_FindsResults()
    {
        var fhirClient = new FhirClientLite(WorkspaceUserAccessToken, Api, Api.BaseAddress.GetBlueButtonProWorkspaceUrl(Workspace.Id.ToString()));

        //  Search for patient resources...
        var patientIdGerardoBotello = "8de3051f-6298-43e6-9b7f-2aa6443ee760";
        var allergyBundle = await fhirClient.Search<AllergyIntolerance>(new[] { $"patient={patientIdGerardoBotello}" });
        var conditionBundle = await fhirClient.Search<Condition>(new[] { $"patient={patientIdGerardoBotello}" });
        var immunizationBundle = await fhirClient.Search<Immunization>(new[] { $"patient={patientIdGerardoBotello}" });

        // Verify we got the correct results...
        Assert.Equal(1, allergyBundle.Data.Entry.Count);
        Assert.Equal(6, conditionBundle.Data.Entry.Count);
        Assert.Equal(27, immunizationBundle.Data.Entry.Count);
    }

    [Fact]
    public async Task search_both_internal_and_external()
    {
        var workspacesClient = new WorkspacesClient(
            DeveloperOrganizationUserAccessToken,
            Api,
            new MeldRxSdkOptions(Api.BaseAddress.GetBaseUrlWithSuffix("api"))
        );

        var result = await workspacesClient.CreateDataRule(
            Workspace.Id,
            new(LinkedApiAction.Search, ResourceType.Patient, LinkedApiActionTarget.Both, Workspace.LinkedFhirApiDto.Id, null)
        );
        result.Success.ShouldBeTrue(result.RawResponseContent);

        var fhirLiteClient = new FhirClientLite(WorkspaceUserAccessToken, Api, Api.BaseAddress.GetBlueButtonProWorkspaceUrl(Workspace.Id.ToString()));

        var internalPatientFamilyName = "Bortelito";
        var externalPatientFamilyName = "Botello";
        var patient = MockingUtilities.CreatePatient("gerardo", internalPatientFamilyName, new DateTime(1990, 09, 09), AdministrativeGender.Unknown);

        var createResult = await fhirLiteClient.Create(patient);
        createResult.Success.ShouldBeTrue(createResult.ErrorMessage);

        //  Perform the search...
        var searchResult = await fhirLiteClient.Search<Patient>(new[] { "given=gerardo" });
        var bundle = searchResult.Data;
        var entries = bundle.Entry;
        Assert.Equal(2, entries.Count);

        var bundleOneEntry = entries[0];
        var bundleOne = Assert.IsType<Bundle>(bundleOneEntry.Resource);
        var bundleTwoEntry = entries[1];
        var bundleTwo = Assert.IsType<Bundle>(bundleTwoEntry.Resource);

        // we can't expect the order to be consistent so assert together
        Bundle[] bundles = { bundleOne, bundleTwo };

        Assert.Single(bundles.SelectMany(x => x.Entry).Where(x => (x.Resource as Patient)!.Name.First().Family == internalPatientFamilyName));
        Assert.Single(bundles.SelectMany(x => x.Entry).Where(x => (x.Resource as Patient)!.Name.First().Family == externalPatientFamilyName));
    }
}
