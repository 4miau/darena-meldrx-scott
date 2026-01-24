using MeldRx.Sdk.ApiClientModels;
using SystemTests._Fixtures;

namespace SystemTests.BlueButtonPro.SystemAppTests;

public class SystemAppFixture(Guid workspaceId, string workspaceName)
    : ClientFixture(GlobalState.Settings.ApiUrl, workspaceId, workspaceName)
{
    public override IAccessTokenProvider TokenProvider { get; } = new ClientCredentialsTokenManager(
        new HttpClient(),
        new ClientCredentialsTokenManagerOptions()
        {
            AuthorityTokenUrl = GlobalState.Settings.AuthorityUrl.TrimEnd('/') + "/connect/token",
            ClientId = GlobalState.Settings.AppClientId,
            ClientSecret = GlobalState.Settings.AppClientSecret,
            Scopes = ""
        }
    );
}
