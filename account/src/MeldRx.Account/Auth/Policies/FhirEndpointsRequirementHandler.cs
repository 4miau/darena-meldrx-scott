using Meldh.Fhir.Core.Interfaces;
using MeldRx.Services.Shared.Repositories;
using HeaderNames = Microsoft.Net.Http.Headers.HeaderNames;

namespace MeldRx.Services.Api.Auth.Policies
{
    public class FhirEndpointsRequirementHandler : BaseFhirRecordRequirementHandler<FhirEndpointsRequirement>
    {
        private readonly IFhirRecordGrantsProvider _fhirRecordGrantsProvider;
        private readonly MeldRxSettings _settings;

        public FhirEndpointsRequirementHandler(
            IHttpContextAccessor contextAccessor,
            IFhirRecordGrantsProvider fhirRecordGrantsProvider,
            IExternalApplicationRepository externalAppRepo,
            ILinkedFhirApiConnectService linkedFhirApiConnectService,
            UserWorkspacePermissions userWorkspacePermissions,
            AppWorkspacePermissions appWorkspacePermissions,
            IFhirServerRepository fhirServerRepository,
            MeldRxSettings settings,
            ILogger<FhirEndpointsRequirementHandler> logger
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
            _fhirRecordGrantsProvider = fhirRecordGrantsProvider;
            _settings = settings;
        }

        /// <inheritdoc />
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, FhirEndpointsRequirement requirement)
        {
            // If the user is not authenticated, we need to check for two situations. One being an external application
            // has made the request, in this situation OAuth is not applicable. The other situation is the requirement for
            // anonymous read access to a fhir server
            if (!context.User.Identity.IsAuthenticated)
            {
                var request = ContextAccessor.HttpContext.Request;
                if (request.Query.ContainsKey(HttpConstants.ExternalApplicationClientIdQueryParameterName))
                {
                    // If a client id query parameter exists in the URL, then this needs to be assumed an external application
                    // is making the request. Validate the external application
                    if (!await IsExternalApplicationValidAsync())
                    {
                        return;
                    }

                    return;
                }

                if (ContextAccessor.HttpContext.Request.Headers.ContainsKey(HeaderNames.Authorization))
                {
                    // If the request has an 'Aut1orization' header, then the request was intended to be authorized using
                    // a bearer token, and not anonymous access, so just return here without succeeding requirement
                    return;
                }

                // If we got here determine if the specified FHIR server has anonymous read access allowed. This also
                // means that reject any calls that are not GET calls
                if (ContextAccessor.HttpContext.Request.Method != HttpMethod.Get.ToString())
                {
                    return;
                }

                var fhirServerName = ContextAccessor.HttpContext.GetRouteData().Values[HttpConstants.ServerNameRouteParameter]?.ToString();
                if (string.IsNullOrWhiteSpace(fhirServerName))
                {
                    return;
                }

                var fhirServer = await FhirServerRepository.FindByDisplayNameAsync(fhirServerName);
                if (fhirServer == null)
                {
                    return;
                }

                if (fhirServer.AllowAnonymousReadAccess)
                {
                    _fhirRecordGrantsProvider.WorkspacePermission = new WorkspacePermission(fhirServer.Map(_settings))
                    {
                        IsAnonymousReadAccess = true,
                    };
                    context.Succeed(requirement);
                }

                return;
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
