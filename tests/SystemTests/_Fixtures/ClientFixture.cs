using MeldRx.Sdk.ApiClient;
using MeldRx.Sdk.ApiClient.MeldRxApi;
using MeldRx.Sdk.ApiClientModels;
using MeldRx.Sdk.Constants;
using MeldRx.Sdk.Fhir;

namespace SystemTests._Fixtures;

public abstract class ClientFixture(string baseUrl, Guid workspaceId, string workspaceName)
{
    public abstract IAccessTokenProvider TokenProvider { get; }
    public Guid WorkspaceId => workspaceId;
    public string WorkspaceName => workspaceName;

    public FhirClientLite Fhir => new FhirClientLite(
        TokenProvider,
        new HttpClient(),
        baseUrl.TrimEnd('/') + $"/{UrlConstants.BlueButtonProFhirPath}/{workspaceName}"
    );

    public InvitesClient Invites => new InvitesClient(
        TokenProvider,
        new HttpClient(),
        new MeldRxSdkOptions(baseUrl)
    );

    public ImportsClient Imports => new ImportsClient(
        TokenProvider,
        new HttpClient(),
        new MeldRxSdkOptions(baseUrl)
    );

    public BackgroundJobsClient BackgroundJobs => new BackgroundJobsClient(
        TokenProvider,
        new HttpClient(),
        new MeldRxSdkOptions(baseUrl)
    );

    public DownloadsClient Downloads => new DownloadsClient(
        TokenProvider,
        new HttpClient(),
        new MeldRxSdkOptions(baseUrl)
    );

    public DirectoryListClient Directory => new DirectoryListClient(
        TokenProvider,
        new HttpClient(),
        new MeldRxSdkOptions(baseUrl)
    );

    public DocumentsClient Documents => new DocumentsClient(
        TokenProvider,
        new HttpClient(),
        new MeldRxSdkOptions(baseUrl)
    );

    public PatientClient Patients => new PatientClient(
        TokenProvider,
        new HttpClient(),
        new MeldRxSdkOptions(baseUrl)
    );
}
