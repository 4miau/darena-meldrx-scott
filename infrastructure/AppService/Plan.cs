using System.Xml;
using Pulumi;

namespace MeldRx.Infrastructure.AppService
{
    public class Plan
    {
        public Pulumi.AzureNative.Web.AppServicePlan plan { get; set; }
        public Output<string> Id { get; set; }
        public Output<string> PrimaryKey { get; set; }

        private ProjectConfig p = new ProjectConfig();

        public Plan(string pulumiName, string resourceName, int size, MeldRx.Infrastructure.ResourceGroup rg)
        {
            plan = new Pulumi.AzureNative.Web.AppServicePlan(pulumiName, new()
            {
                Name = $"plan-{resourceName}-{p.env}",
                ResourceGroupName = rg.Name,
                Kind = "linux",
                Reserved = true,
                ElasticScaleEnabled = false,
                ZoneRedundant = false,
                Sku = new Pulumi.AzureNative.Web.Inputs.SkuDescriptionArgs
                {
                    Name = p.env_AppServicePlan_Apps_SKU_Name,
                    Tier = p.env_AppServicePlan_Apps_SKU_Tier,
                    Family = p.env_AppServicePlan_Apps_SKU_Family,
                    Size = p.env_AppServicePlan_Apps_SKU_Size,
                    Capacity = size,
                },
                Tags = { { "env", p.env } }
            });

            Id = plan.Id;
            PrimaryKey = Output.Create("");
        }
    }
}
