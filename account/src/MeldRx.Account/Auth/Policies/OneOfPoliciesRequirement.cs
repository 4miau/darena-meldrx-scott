namespace MeldRx.Account.ServicesRegistration.Policies;

public class OneOfPoliciesRequirement : IAuthorizationRequirement
{
    public string[] PolicyNames { get; private set; }

    public OneOfPoliciesRequirement(params string[] policyNames)
    {
        PolicyNames = policyNames;
    }
}