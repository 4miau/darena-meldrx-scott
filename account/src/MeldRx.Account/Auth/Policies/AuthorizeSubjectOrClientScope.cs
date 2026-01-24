using Meldh.Fhir.Core.Interfaces;
using MeldRx.Services.Api.Auth.Policies;
using MeldRx.Services.Shared.Repositories;

namespace MeldRx.Account.Auth.Policies;

public class AuthorizeSubjectOrClientScopeAttribute : AuthorizeAttribute,
    IAuthorizationRequirement,
    IAuthorizationRequirementData
{
    private readonly string _scope;

    public AuthorizeSubjectOrClientScopeAttribute(string scope = "")
    {
        _scope = scope;
        AuthenticationSchemes = $"{IdentityConstants.ApplicationScheme},{IdentityServerConstants.LocalApi.AuthenticationScheme}";
    }

    public string Scope => _scope;

    public IEnumerable<IAuthorizationRequirement> GetRequirements()
    {
        yield return this;
    }

    public bool Check(ClaimsPrincipal user) => string.IsNullOrEmpty(_scope) || user.HasClaim(JwtClaimTypes.Scope, _scope);
}

public class SubjectOrClientScopeRequirementHandler : BaseFhirRecordRequirementHandler<AuthorizeSubjectOrClientScopeAttribute>
{
    private readonly IHttpContextAccessor _contextAccessor;

    public SubjectOrClientScopeRequirementHandler(
        IHttpContextAccessor contextAccessor,
        IFhirRecordGrantsProvider fhirRecordGrantsProvider,
        IExternalApplicationRepository externalAppRepo,
        ILinkedFhirApiConnectService linkedFhirApiConnectService,
        UserWorkspacePermissions userWorkspacePermissions,
        AppWorkspacePermissions appWorkspacePermissions,
        IFhirServerRepository fhirServerRepository,
        ILogger<SubjectOrClientScopeRequirementHandler> logger
    ) : base(
        contextAccessor,
        fhirRecordGrantsProvider,
        externalAppRepo,
        linkedFhirApiConnectService,
        userWorkspacePermissions,
        appWorkspacePermissions,
        fhirServerRepository,
        logger
    )
    {
        _contextAccessor = contextAccessor;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AuthorizeSubjectOrClientScopeAttribute requirement
    )
    {
        var user = context.User;
        if (!user.IsAuthenticated())
        {
            context.Fail(new AuthorizationFailureReason(this, "user not authenticated"));
            return;
        }

        if (user.HasClaim(c => c.Type is JwtClaimTypes.Subject))
        {
            var webAppUser = user.Identity?.AuthenticationType == IdentityConstants.ApplicationScheme;
            if (!webAppUser && !requirement.Check(user))
            {
                context.Fail(new AuthorizationFailureReason(this, $"endpoint requires 'user with sub claim and {requirement.Scope} scope'"));
                return;
            }
        }
        else if (user.HasClaim(c => c.Type == JwtClaimTypes.ClientId))
        {
            if (!requirement.Check(user))
            {
                context.Fail(new AuthorizationFailureReason(this, $"endpoint requires 'a user' or 'client with client_id claim and {requirement.Scope} scope'"));
                return;
            }
        }
        else
        {
            context.Fail(new AuthorizationFailureReason(this, "only user or client may call this endpoint."));
            return;
        }

        var endpoint = _contextAccessor.HttpContext?.GetEndpoint()!;
        var requireWorkspaceContext = endpoint?.Metadata.GetMetadata<RequireWorkspaceContextAttribute>() != null;
        if (requireWorkspaceContext)
        {
            var (success, failureReason) = await IsFhirRecordValidAsync();
            if (!success)
            {
                context.Fail(new AuthorizationFailureReason(this, failureReason));
                return;
            }
        }

        context.Succeed(requirement);
    }
}
