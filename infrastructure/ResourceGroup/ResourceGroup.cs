using Pulumi;

namespace MeldRx.Infrastructure
{
    public class ResourceGroup
    {
        public Pulumi.AzureNative.Resources.ResourceGroup rg { get; set; }
        public Output<string> Name { get; set; }

        private ProjectConfig p = new ProjectConfig();

        public ResourceGroup(string pulumiName, string resourceName, Pulumi.ProviderResource provider)
        {
            rg = new Pulumi.AzureNative.Resources.ResourceGroup(pulumiName, new()
            {
                ResourceGroupName = $"rg-{resourceName}{(p.env == "cdn" ? "" : $"-{p.env}")}",
                Location = p.location,
                Tags = { { "env", p.env } }
            }, new CustomResourceOptions() { Provider = provider });

            Name = rg.Name;
        }
    }
}
