namespace MeldRx.Account.Auth;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequireWorkspaceContextAttribute : Attribute
{
    public bool DisallowVirtualWorkspace { get; set; } = false;
}
