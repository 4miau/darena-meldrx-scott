using Pulumi;

namespace MeldRx.Infrastructure.Container
{
    public class ContainerRegistry
    {
        public Pulumi.AzureNative.ContainerRegistry.Registry registry { get; set; }

        private ProjectConfig p = new ProjectConfig();

        public ContainerRegistry(string pulumiName, string resourceName, MeldRx.Infrastructure.ResourceGroup rg)
        {
            // Azure Container Registry
            registry = new Pulumi.AzureNative.ContainerRegistry.Registry(pulumiName, new()
            {
                RegistryName = $"acr{resourceName}",
                ResourceGroupName = rg.Name,
                AdminUserEnabled = true,
                Sku = new Pulumi.AzureNative.ContainerRegistry.Inputs.SkuArgs
                {
                    Name = "Standard",
                },
                Tags = { { "env", p.env } }
            });
        }
    }
}