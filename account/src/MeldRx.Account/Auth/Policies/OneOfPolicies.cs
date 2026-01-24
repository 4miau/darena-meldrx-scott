namespace MeldRx.Account.ServicesRegistration.Policies
{
    public class OneOfPoliciesHandler : AuthorizationHandler<OneOfPoliciesRequirement>
    {
        private readonly IAuthorizationServiceResolver _authorizationServiceResolver;

        public OneOfPoliciesHandler(IAuthorizationServiceResolver authorizationServiceResolver)
        {
            _authorizationServiceResolver = authorizationServiceResolver;
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OneOfPoliciesRequirement requirement)
        {
            var authorizationService = _authorizationServiceResolver.Resolve();
            if (authorizationService != null)
            {
                foreach (var policyName in requirement.PolicyNames)
                {
                    if ((await authorizationService.AuthorizeAsync(context.User, policyName)).Succeeded)
                    {
                        context.Succeed(requirement);
                        return;
                    }
                }
            }
        }
    }

    public interface IAuthorizationServiceResolver
    {
        IAuthorizationService Resolve();
    }

    public class HttpContextAuthorizationServiceResolver : IAuthorizationServiceResolver
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpContextAuthorizationServiceResolver(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public IAuthorizationService Resolve()
        {
            return _httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IAuthorizationService>();
        }
    }
}
