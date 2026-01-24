using Pulumi;
using System.IO;
using System.Linq;

namespace MeldRx.Infrastructure.Sql
{
    public class ElasticPool
    {
        public Pulumi.AzureNative.Sql.ElasticPool epool { get; set; }
        public Output<string> Id { get; set; }

        private ProjectConfig p = new ProjectConfig();

        public ElasticPool(string pulumiName, string resourceName, Pulumi.AzureNative.Sql.Server sqlServer, MeldRx.Infrastructure.ResourceGroup rg)
        {
            epool = new Pulumi.AzureNative.Sql.ElasticPool($"{pulumiName}-epool", new()
            {
                ElasticPoolName = $"sqlserver-epool-{resourceName}-{p.env}",
                PerDatabaseSettings = new Pulumi.AzureNative.Sql.Inputs.ElasticPoolPerDatabaseSettingsArgs
                {
                    MaxCapacity = double.Parse(p.env_SqlServer_ElasticPool_MaxCapacity),
                    MinCapacity = 0.25,
                },
                ResourceGroupName = rg.Name,
                ServerName = sqlServer.Name,
                Sku = new Pulumi.AzureNative.Sql.Inputs.SkuArgs
                {
                    Capacity = int.Parse(p.env_SqlServer_ElasticPool_SKU_Capacity),
                    Name = p.env_SqlServer_ElasticPool_SKU_Name,
                    Tier = p.env_SqlServer_ElasticPool_SKU_Tier,
                },
                Tags = { { "env", p.env } }
            }, new CustomResourceOptions { Protect = false });

            Id = epool.Id;
        }
    }
}
