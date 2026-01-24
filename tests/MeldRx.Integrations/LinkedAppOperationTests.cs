using Duende.IdentityServer.Models;
using Hl7.Fhir.Model;
using Meldh.Fhir.Core.Interfaces;
using MeldRx.Sdk.ApiClient;
using MeldRx.Sdk.Dtos;
using MeldRx.Sdk.Dtos.DynamicRegistration;
using MeldRx.Sdk.Enums;
using MeldRx.Sdk.Extensions;
using MeldRx.Sdk.Fhir;
using MeldRx.Sdk.Utilities;
using MeldRx.Services.Tests._Fixtures;
using MeldRx.Services.Tests._Fixtures.MockServices.Auth;
using MeldRx.Services.Tests.MeldRx.Sdk.Tests;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Task = System.Threading.Tasks.Task;

namespace MeldRx.Integrations;

public class LinkedAppOperationTests : LinkedWorkspaceTest
{
    public LinkedAppOperationTests(IntegrationFixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
        SearchService = Services.GetRequiredService<IFhirSearchService>();
        FhirClient = new FhirClientLite(
            DefaultWorkspaceUserToken,
            Api,
            Api.BaseAddress.GetBlueButtonProWorkspaceUrl(DefaultWorkspace.FhirDatabaseDisplayName)
        );
    }

    public IFhirSearchService SearchService { get; private set; }
    public FhirClientLite FhirClient { get; private set; }

    protected override CreateDeveloperWorkspaceCommand CreateWorkspaceCommand => new CreateDeveloperWorkspaceCommand("Test Org")
    {
        LinkedFhirApi = new()
        {
            FhirApiProviderMeldRxIdentifier = DefaultFhirApiProvider.MeldRxIdentifier,
            BaseUrl = _Config.GetConfig().BackendEhrCredentials.EpicBackendSandbox.FhirUrl,
        }
    };

    [Theory]
    [InlineData(Operations.Height, "8302-2")]
    [InlineData(Operations.SmokingStatus, "72166-2")]
    [InlineData(Operations.HemoglobinA1c, "4548-4")]
    public async Task TestOperationToAppLinkedWithEpic(Operations operation,string expectedCode)
    {
        // ARRANGE
        var appClient = new AppsClient(
            DeveloperOrganizationUserAccessToken,
            Api,
            new MeldRxSdkOptions(Api.BaseAddress.GetBaseUrlWithSuffix("api"))
        );

        var appResult = await appClient.CreateAsync(
            new CreateAppCommand()
            {
                Name = Guid.NewGuid().ToString(),
                UserType = SoFAppUserType.System,
                Scope = "system/*.read",
                JwksUri = MockExternalCalls.JsonWebKeySet,
            }
        );
        appResult.Success.ShouldBeTrue(appResult.RawResponseContent);

        var linkedAppResult = await appClient.CreateLinkedAppAsync(
            new LinkedAppCreateDto
            {
                FhirApiProviderMeldRxIdentifier = DefaultFhirApiProvider.MeldRxIdentifier,
                MeldRxClientId = appResult.DeserializedContent.ClientId,
                ClientId = "93865437-e287-4078-96b8-5ca014f088a5",
                ClientName = "abc",
                Scopes = "system/*.read",
                SecretType = SecretType.PrivateKey,
                ClientSecret = _Config.PrivateKey,
            }
        );
        linkedAppResult.Success.ShouldBeTrue(appResult.RawResponseContent);

        var token = await FhirBackendUtils.GetBackendAccessCodeForJWKS(
            Api,
            _Config.PrivateKey,
            appResult.DeserializedContent.ClientId,
            Api.BaseAddress.GetBaseUrlWithSuffix(Workspace.FhirDatabaseDisplayName) + "/connect/token",
            _Config.JwksKid,
            _Config.JwksAlg
        );

        Assert.NotNull(token);
        Assert.NotNull(token.AccessToken);
        Assert.NotEmpty(token.AccessToken);

        //add permission for the app for workspace
        var addPermission = await WorkspacesClient.CreateAppPermissionAsync(new CreateAppPermissionCommand
        {
            AppRole = AppRole.Administrator,
            ClientId = appResult.DeserializedContent.ClientId,
            WorkspaceSlug = Workspace.FhirDatabaseDisplayName,
        });
        addPermission.Success.ShouldBe(true);

        var fhirClient = new FhirClientLite(
            new MockAccessTokenProvider(token.AccessToken),
            Api,
            Api.BaseAddress.GetBlueButtonProWorkspaceUrl(Workspace.Id.ToString())
        );
        // ACT
        var patientId = "e63wRTbPfr1p8UW81d8Seiw3";
        /*This is a patient on the Epic Test Server
         *  ("given", "Theodore"),
         *  ("family", "MyChart"),
         *  ("birthdate", "1948-07-07")*/

        var operationResult = await fhirClient.ReadOperation<Patient>(patientId, operation.ToString());
        var bundle = operationResult.Data;

        // ASSERT
        // The operations bundle contains observations
        Assert.NotEmpty(bundle.Entry);
        // The observations contain the correct code for the height operation.
        Assert.All(bundle.Entry.Select(x => x.Resource), (resource =>
        {
            var observation = Assert.IsType<Observation>(resource);
            Assert.Contains(expectedCode, observation.Code.Coding.Select(x => x.Code));
        }));
    }
}
