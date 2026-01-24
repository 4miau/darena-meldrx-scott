namespace MeldRx.Services.Api.Auth.Policies;

public class AnonymousRequirementHandler : AuthorizationHandler<AnonymousRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AnonymousRequirement requirement)
    {
        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}