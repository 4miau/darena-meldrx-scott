using Pulumi;

namespace MeldRx.Infrastructure.ServiceBus
{
    public class Namespace
    {
        public Pulumi.AzureNative.ServiceBus.Namespace sb { get; set; }
        public Output<string> Name { get; set; }
        public Output<string> PrimaryConnectionString { get; set; }

        private ProjectConfig p = new ProjectConfig();
        public Namespace(string pulumiName, string resourceName, MeldRx.Infrastructure.ResourceGroup rg)
        {
            sb = new Pulumi.AzureNative.ServiceBus.Namespace(pulumiName, new()
            {
                NamespaceName = $"sb-{resourceName}-{(p.env == "sb" ? "sandbox" : p.env)}",
                ResourceGroupName = rg.Name,
                Sku = new Pulumi.AzureNative.ServiceBus.Inputs.SBSkuArgs
                {
                    Name = Pulumi.AzureNative.ServiceBus.SkuName.Standard,
                    Tier = Pulumi.AzureNative.ServiceBus.SkuTier.Standard,
                },
                DisableLocalAuth = false,
                MinimumTlsVersion = "1.2",
                PrivateEndpointConnections = [],
                PublicNetworkAccess = "Enabled",
                ZoneRedundant = bool.Parse(p.env_ServiceBus_Zone_Redundant),
                Tags = { { "env", p.env } },
            });

            Name = sb.Name;
            PrimaryConnectionString = getServiceBusConnectionString(rg);
        }

        private Output<string> getServiceBusConnectionString(MeldRx.Infrastructure.ResourceGroup rg)
        {
            // Service Bus access keys
            var serviceBusKeys = Pulumi.AzureNative.ServiceBus.ListNamespaceKeys.Invoke(new Pulumi.AzureNative.ServiceBus.ListNamespaceKeysInvokeArgs
            {
                ResourceGroupName = rg.Name,
                NamespaceName = sb.Name,
                AuthorizationRuleName = "RootManageSharedAccessKey"
            });

            return serviceBusKeys.Apply(sbKeys =>
            {
                var connectingString = sbKeys.PrimaryConnectionString;
                return Output.CreateSecret(connectingString);
            });
        }
    }
}
