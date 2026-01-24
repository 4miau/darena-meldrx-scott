using DarenaSolutions.Bbp.Api.Extensions;
using JasperFx.Core;
using Meldh.Fhir.Core.Constants;
using Meldh.Fhir.Core.Interfaces;
using MeldRx.Services.Shared.Repositories;
using WorkspacePermission = MeldRx.Account.Services.WorkspacePermission;

#nullable enable

namespace MeldRx.Services.Api.Auth.Policies
{
    /// <summary>
    /// The base class for handling a fhir record requirement. Any requirement handlers that need
    /// to validate that a user has access to the specified fhir server should inherit this class as
    /// it contains a common method to validate it
    /// </summary>
    /// <typeparam name="T">The type of requirement to handle</typeparam>
    public abstract class BaseFhirRecordRequirementHandler<T> : BaseWithExternalApplicationAuthorizationHandler<T>
        where T : IAuthorizationRequirement
    {
        private readonly ILinkedFhirApiConnectService _linkedFhirApiConnectService;
        private readonly UserWorkspacePermissions _userWorkspacePermissions;
        private readonly AppWorkspacePermissions _appWorkspacePermissions;
        protected readonly IFhirServerRepository FhirServerRepository;

        protected BaseFhirRecordRequirementHandler(
            IHttpContextAccessor contextAccessor,
            IFhirRecordGrantsProvider fhirRecordGrantsProvider,
            IExternalApplicationRepository externalAppRepo,
            ILinkedFhirApiConnectService linkedFhirApiConnectService,
            UserWorkspacePermissions userWorkspacePermissions,
            AppWorkspacePermissions appWorkspacePermissions,
            IFhirServerRepository fhirServerRepository,
            ILogger<BaseFhirRecordRequirementHandler<T>> logger
            )
            : base(contextAccessor, externalAppRepo)
        {
            _linkedFhirApiConnectService = linkedFhirApiConnectService;
            _userWorkspacePermissions = userWorkspacePermissions;
            _appWorkspacePermissions = appWorkspacePermissions;
            FhirServerRepository = fhirServerRepository;
            ContextAccessor = contextAccessor;
            FhirRecordGrantsProvider = fhirRecordGrantsProvider;
            Logger = logger;
        }

        /// <summary>
        /// Gets the <see cref="IHttpContextAccessor"/> implementation
        /// </summary>
        protected IHttpContextAccessor ContextAccessor { get; }

        /// <summary>
        /// Gets the <see cref="IFhirRecordGrantsProvider"/> implementation
        /// </summary>
        protected IFhirRecordGrantsProvider FhirRecordGrantsProvider { get; }

        public ILogger<BaseFhirRecordRequirementHandler<T>> Logger { get; }


        /// <summary>
        /// Validates that the current user has access to the specified fhir server. This method will set <see cref="IFhirRecordGrantsProvider.FhirRecords"/>
        /// if the fhir server is accessible for that user
        /// </summary>
        /// <returns><c>true</c> if the user has access to the specified fhir server</returns>
        protected async Task<(bool Success, string FailureReason)> IsFhirRecordValidAsync()
        {
            var httpContext = ContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return (false, "called outside of http context");
            }

            if (!httpContext.User.IsAuthenticated()) //Check for anonymous access
            {
                // Since this situation can only occur in the FHIR endpoints (FhirController), we only need to check the
                var fhirServerName = httpContext.GetRouteData().Values[HttpConstants.ServerNameRouteParameter]?.ToString();
                if (string.IsNullOrWhiteSpace(fhirServerName))
                {
                    return (false, "Not Allowed");
                }

                return (false, "No Authorization");
            }

            var serverName = httpContext.Request.RouteValues[HttpConstants.ServerNameRouteParameter]?.ToString();
            var fhirServerId = httpContext.Request.Headers[HeaderNames.FhirServerIdContext].FirstOrDefault();
            var orgTin = httpContext.Request.Headers[HeaderNames.TinContext].FirstOrDefault() ??
                         httpContext.Request.Headers[HeaderNames.WsIdentifier].FirstOrDefault();

            //Validate if this is a Linked FHIR API. If it is, we shortcircuit the validation to the claim in token
            var linkedApiResult = await _linkedFhirApiConnectService.LoadAsync(serverName, fhirServerId, orgTin);
            if (!linkedApiResult.Success)
            {
                return (false, linkedApiResult.ErrorMessage);
            }

            var userContextInfo = UserIdOrClientIdResult.Create(httpContext);
            if (linkedApiResult is { Success: true, Data: true })
            {
                //Set Permissions based on LinkedAPI and Users
                OperationResult<WorkspacePermission> linkedApiPermission = userContextInfo.UserIdHasValue
                    ? await _userWorkspacePermissions.CheckById(
                        _linkedFhirApiConnectService.LinkedFhirApiRequest.LinkedFhirApi.FhirServer.Id,
                        userContextInfo.UserId
                    )
                    : await _appWorkspacePermissions.CheckAsync(
                        _linkedFhirApiConnectService.LinkedFhirApiRequest.LinkedFhirApi.FhirServer.Id.ToString(),
                        userContextInfo.ClientId
                    );

                if (!linkedApiPermission.Success)
                {
                    return (false, linkedApiPermission.ErrorMessage);
                }

                FhirRecordGrantsProvider.WorkspacePermission = linkedApiPermission.Data;
                return (true, "");
            }

            var endpoint = httpContext.GetEndpoint()!;
            var requireWorkspaceContext = endpoint.Metadata.GetMetadata<RequireWorkspaceContextAttribute>();
            if (requireWorkspaceContext == null && !httpContext.IsBackgroundJobsController() && !httpContext.IsFhirServerNameBasedController())
            {
                return (false, "fhir server context not needed for request.");
            }

            OperationResult<WorkspacePermission>? permission = null;
            if (!string.IsNullOrEmpty(fhirServerId))
            {
                Logger.LogWarning("Legacy usage of 'FhirServerId-Context' header parameter: {FhirServerId}", fhirServerId);
                permission = userContextInfo switch
                {
                    { IsSuperAdmin: true } => await _userWorkspacePermissions.ForSuperAdmin(fhirServerId),
                    { UserIdHasValue: true } => await _userWorkspacePermissions.CheckById(Guid.Parse(fhirServerId), userContextInfo.UserId),
                    { ClientIdHasValue: true } => await _appWorkspacePermissions.CheckAsync(fhirServerId, userContextInfo.ClientId!),
                    _ => OperationResult<WorkspacePermission>.Error("unknown user request"),
                };
            }
            else if (!string.IsNullOrEmpty(orgTin))
            {
                Logger.LogWarning("Legacy usage of 'Tin-Context' or 'WsIdentifier' header parameter: {OrganizationTin}", orgTin);
                permission = userContextInfo switch
                {
                    { IsSuperAdmin: true } => await _userWorkspacePermissions.ForSuperAdmin(orgTin),
                    { UserIdHasValue: true } => await _userWorkspacePermissions.CheckByOrganizationTin(orgTin, userContextInfo.UserId),
                    { ClientIdHasValue: true } => await _appWorkspacePermissions.CheckAsync(orgTin, userContextInfo.ClientId!),
                    _ => OperationResult<WorkspacePermission>.Error("unknown user request"),
                };
            }
            else if(!string.IsNullOrEmpty(serverName))
            {
                permission = userContextInfo switch
                {
                    { IsSuperAdmin: true } => await _userWorkspacePermissions.ForSuperAdmin(serverName),
                    { UserIdHasValue: true } => await _userWorkspacePermissions.CheckAsync(serverName, userContextInfo.UserId),
                    { ClientIdHasValue: true } => await _appWorkspacePermissions.CheckAsync(serverName, userContextInfo.ClientId!),
                    _ => OperationResult<WorkspacePermission>.Error("unknown user request"),
                };
            }
            else
            {
                return (false, "Not Allowed");
            }

            if (permission is not {Success:true, Data: not null })
            {
                return (false, "Not Allowed");
            }

            var disallowForVirtualWorkspace = permission.Data.Workspace.Type == FhirServerType.Virtual && requireWorkspaceContext is { DisallowVirtualWorkspace: true };
            if (disallowForVirtualWorkspace)
            {
                return (false, "Operation forbidden for virtual workspaces");
            }

            if (permission.Success && (permission.Data.HasOrgOrAppAccess() || permission.Data.HasPersonAccess()))
            {
                FhirRecordGrantsProvider.WorkspacePermission = permission.Data;
                return (true, "");
            }

            return (false, "Not Allowed");
        }
    }
}
