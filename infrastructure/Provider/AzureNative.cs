
namespace MeldRx.Infrastructure.Provider
{
    public class AzureNative
    {
        public Pulumi.AzureNative.Provider provider { get; set; }
        private ProjectConfig p = new ProjectConfig();

        public AzureNative(string pulumiName, string subscriptionId)
        {
            provider = new Pulumi.AzureNative.Provider(pulumiName, new Pulumi.AzureNative.ProviderArgs()
            {
                SubscriptionId = subscriptionId,
                ClientId = p.servicePrincipalId,
                ClientSecret = p.servicePrincipalSecret,
                TenantId = p.tenantId,
                PartnerId = p.partnerId,
                UseMsi = false
            });
        }
    }
}
