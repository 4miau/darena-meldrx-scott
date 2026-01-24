using MeldRx.Services.Shared.Repositories;

namespace MeldRx.Services.Api.Auth.Policies
{
    /// <summary>
    /// The requirement handler that handles the <see cref="AccessibleFhirRecordsEndpointRequirement"/> requirement
    /// </summary>
    public class AccessibleFhirRecordsEndpointRequirementHandler : BaseWithExternalApplicationAuthorizationHandler<AccessibleFhirRecordsEndpointRequirement>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccessibleFhirRecordsEndpointRequirementHandler"/> class
        /// </summary>
        /// <param name="contextAccessor">The <see cref="IHttpContextAccessor"/> implementation</param>
        /// <param name="externalAppRepo">The <see cref="IExternalApplicationRepository"/> implementation</param>
        public AccessibleFhirRecordsEndpointRequirementHandler(IHttpContextAccessor contextAccessor, IExternalApplicationRepository externalAppRepo)
            : base(contextAccessor, externalAppRepo)
        {
        }

        /// <inheritdoc />
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AccessibleFhirRecordsEndpointRequirement requirement)
        {
            if (!context.User.Identity.IsAuthenticated &&
                !await IsExternalApplicationValidAsync())
            {
                return;
            }

            context.Succeed(requirement);
        }
    }
}