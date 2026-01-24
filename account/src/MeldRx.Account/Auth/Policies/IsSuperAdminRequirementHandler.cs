namespace MeldRx.Services.Api.Auth.Policies
{
    public class IsSuperAdminRequirementHandler:AuthorizationHandler<IsSuperAdminRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsSuperAdminRequirement requirement)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                if (context.User.HasClaim("role", AuthConstants.SuperAdminRole))
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }

    
}
