
namespace MeldRx.Infrastructure.Provider
{
    public class AzureAD
    {
        public Pulumi.AzureAD.Provider provider { get; set; }
        private ProjectConfig p = new ProjectConfig();

        public AzureAD(string pulumiName)
        {
            // REQUIRED - Service Principal must have Azure AD Role: Privileged role administrator
            provider = new Pulumi.AzureAD.Provider(pulumiName, new Pulumi.AzureAD.ProviderArgs()
            {
                ClientId = p.servicePrincipalId,
                ClientSecret = p.servicePrincipalSecret,
                TenantId = p.tenantId,
                MetadataHost = ""
            });
        }
    }
}
