using MeldRx.Services.Shared.Repositories;

namespace MeldRx.Services.Api.Auth.Policies;

/// <summary>
/// 
/// </summary>
public class AccessiblePersonIdRequirementHandler : AuthorizationHandler<AccessiblePersonIdRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserPersonRelationshipRepository _userPersonRelationshipRepository;

    public AccessiblePersonIdRequirementHandler(IHttpContextAccessor httpContextAccessor, IUserPersonRelationshipRepository userPersonRelationshipRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        _userPersonRelationshipRepository = userPersonRelationshipRepository;
    }
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AccessiblePersonIdRequirement requirement)
    {
        if (context.User.Identity.IsAuthenticated)
        {
            if (context.User.HasClaim("role", AuthConstants.SuperAdminRole))
            {
                context.Succeed(requirement);
            }
            var httpContext = _httpContextAccessor.HttpContext;
            var personId = httpContext.Request.RouteValues[HttpConstants.PersonIdRouteParameter].ToString();
            if (personId == null)
            {
                return;
            }

            if (!Guid.TryParse(personId, out _))
            {
                return;
            }

            //We only allow the user to update the image, not a client
            var userContextInfo = UserIdOrClientIdResult.Create(httpContext);
            if (!userContextInfo.UserIdHasValue)
            {
                return;
            }
            var people = await _userPersonRelationshipRepository.FindByUserIdAsync(userContextInfo.UserId, includePersonFhirServer: false);
            var peopleIds = people.Select(x => x.PersonId);
            if (peopleIds.Any(i => i == Guid.Parse(personId)))
            {
                context.Succeed(requirement);
            }

        }
    }
}