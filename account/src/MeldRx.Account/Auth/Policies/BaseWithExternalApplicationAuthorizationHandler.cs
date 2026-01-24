using MeldRx.Services.Shared.Repositories;

namespace MeldRx.Services.Api.Auth.Policies
{
    /// <summary>
    /// The base authorization handler that contains a method to validate an external application.
    /// Any authorization handlers that need external application validation, should inherit this
    /// base class as it contains a common method to validate it
    /// </summary>
    /// <typeparam name="T">The type of the requirement to handle</typeparam>
    public abstract class BaseWithExternalApplicationAuthorizationHandler<T> : AuthorizationHandler<T>
        where T : IAuthorizationRequirement
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IExternalApplicationRepository _externalAppRepo;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseWithExternalApplicationAuthorizationHandler{T}"/> class
        /// </summary>
        /// <param name="contextAccessor">The <see cref="IHttpContextAccessor"/> implementation</param>
        /// <param name="externalAppRepo">The <see cref="IExternalApplicationRepository"/> implementation</param>
        protected BaseWithExternalApplicationAuthorizationHandler(IHttpContextAccessor contextAccessor, IExternalApplicationRepository externalAppRepo)
        {
            _contextAccessor = contextAccessor;
            _externalAppRepo = externalAppRepo;
        }

        /// <summary>
        /// Validates the credentials of an external application. **NOTE: Ensure that the user is not
        /// authenticated by calling context.User.Identity.IsAuthenticated. An external application will
        /// never be authenticated by OAuth, and instead will use a more less secure method of authentication
        /// by using a set of credentials in that are given by query parameters
        /// </summary>
        /// <returns><c>true</c> if the external application's credentials are valid</returns>
        protected async Task<bool> IsExternalApplicationValidAsync()
        {
            var httpContext = _contextAccessor.HttpContext;
            var requestQuery = httpContext.Request.Query;

            var clientId = requestQuery[HttpConstants.ExternalApplicationClientIdQueryParameterName].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(clientId))
            {
                return false;
            }

            var clientSecret = requestQuery[HttpConstants.ExternalApplicationClientSecretQueryParameterName].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(clientSecret))
            {
                return false;
            }

            return await _externalAppRepo.ValidateAsync(clientId, clientSecret);
        }
    }
}