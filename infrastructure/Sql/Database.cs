using Pulumi;
using System;

namespace MeldRx.Infrastructure.Sql
{
    public class Database
    {
        public Pulumi.AzureNative.Sql.Database db { get; set; }
        public Output<string> Name { get; set; }

        private ProjectConfig p = new ProjectConfig();

        public Database(string pulumiName, string resourceName, MeldRx.Infrastructure.Sql.Server sqlServer, MeldRx.Infrastructure.ResourceGroup rg)
        {
            db = new Pulumi.AzureNative.Sql.Database($"db-{pulumiName}", new()
            {
                DatabaseName = resourceName,
                ResourceGroupName = rg.Name,
                ServerName = sqlServer.Name,
                Sku = new Pulumi.AzureNative.Sql.Inputs.SkuArgs
                {
                    Capacity = 0,
                    Name = "ElasticPool",
                    Tier = p.env_SqlServer_Database_SKU_Tier,
                },
                ElasticPoolId = sqlServer.epool.Id,
                CatalogCollation = "SQL_Latin1_General_CP1_CI_AS",
                Collation = "SQL_Latin1_General_CP1_CI_AS",
                MaxSizeBytes = Double.Parse(p.env_SqlServer_Database_MaxSizeInBytes),
                RequestedBackupStorageRedundancy = "Geo",
                ReadScale = "Disabled",
                IsLedgerOn = false,
                ZoneRedundant = false,
                MaintenanceConfigurationId = $"/subscriptions/{p.subscriptionId}/providers/Microsoft.Maintenance/publicMaintenanceConfigurations/SQL_Default",
                Tags = { { "env", p.env } }
            }, new CustomResourceOptions { Protect = bool.Parse(p.env_General_ResourceProtection) });

            Name = db.Name;
        }
    }
}
