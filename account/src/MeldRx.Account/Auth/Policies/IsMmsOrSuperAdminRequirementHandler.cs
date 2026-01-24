namespace MeldRx.Services.Api.Auth.Policies;

public class IsMmsOrSuperAdminRequirementHandler : AuthorizationHandler<IsMmsOrSuperAdminRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsMmsOrSuperAdminRequirement requirement)
    {
        if (context.User.Identity.IsAuthenticated)
        {
            if (context.User.HasClaim("role", AuthConstants.SuperAdminRole))
            {
                context.Succeed(requirement);
            }
            if (context.User.HasClaim("role", AuthConstants.MmsAdminRole))
            {
                context.Succeed(requirement);
            }
        }

        return Task.CompletedTask;
    }
}