using MeldRx.Sdk.ApiClientModels;
using SystemTests._Fixtures;

namespace SystemTests.BlueButtonPro.SystemAppTests;

public class AnonymousUserFixture(Guid workspaceId, string workspaceName)
    : ClientFixture(GlobalState.Settings.ApiUrl, workspaceId, workspaceName)
{
    public override IAccessTokenProvider TokenProvider => new SimpleAccessTokenProvider();
}
