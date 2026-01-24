using Meldh.Fhir.Core.Interfaces;
using MeldRx.Services.Shared.Repositories;

namespace MeldRx.Services.Api.Auth.Policies
{
    public class FhirRecordRequirementHandler : BaseFhirRecordRequirementHandler<FhirRecordRequirement>
    {
        public FhirRecordRequirementHandler(
            IHttpContextAccessor contextAccessor,
            IFhirRecordGrantsProvider fhirRecordGrantsProvider,
            IExternalApplicationRepository externalAppRepo,
            ILinkedFhirApiConnectService linkedFhirApiConnectService,
            UserWorkspacePermissions userWorkspacePermissions,
            AppWorkspacePermissions appWorkspacePermissions,
            IFhirServerRepository fhirServerRepository,
            ILogger<FhirRecordRequirementHandler> logger
        )
            : base(
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
        }

        /// <inheritdoc />
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, FhirRecordRequirement requirement)
        {
            if (!context.User.Identity.IsAuthenticated)
            {
                // If the user is not authenticated with an identity provider, check to see if it is
                // an external application
                if (!await IsExternalApplicationValidAsync())
                {
                    return;
                }
            }

            var (success, failReason) = await IsFhirRecordValidAsync();
            if (success)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail(
                    new AuthorizationFailureReason(this, failReason)
                );
            }
        }
    }
}
