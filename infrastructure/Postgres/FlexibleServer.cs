using Pulumi;
using System.IO;
using System.Linq;

namespace MeldRx.Infrastructure.Postgres
{
    public class FlexibleServer
    {
        public Pulumi.AzureNative.DBforPostgreSQL.Server psqlServer { get; set; }
        public Output<string> Name { get; set; }
        public Output<string> FullyQualifiedDomainName { get; set; }
        private ProjectConfig p = new ProjectConfig();
        public FlexibleServer(string pulumiName, string resourceName, MeldRx.Infrastructure.Random.Password password, MeldRx.Infrastructure.ResourceGroup rg)
        {
            // https://learn.microsoft.com/en-us/azure/postgresql/flexible-server/how-to-configure-sign-in-azure-ad-authentication
            // New-AzureADServicePrincipal -AppId 5657e26c-cc92-45d9-bc47-9da6cfdb4ed9
            // https://github.com/pulumi/pulumi-azure-native/blob/d5b3a2e559efe19e8bb623e95866b875b749d1d5/examples/postgres/index.ts#L38
            psqlServer = new Pulumi.AzureNative.DBforPostgreSQL.Server("psql-flex", new()
            {
                ServerName = $"psql{p.projectName}{p.env}",
                ResourceGroupName = rg.Name,
                Sku = new Pulumi.AzureNative.DBforPostgreSQL.Inputs.SkuArgs
                {
                    Tier = p.env_FlexibleServer_SKU_Tier,
                    Name = p.env_FlexibleServer_SKU_Name,
                },
                AdministratorLogin = p.sqlServerLogin,
                AdministratorLoginPassword = password.Result,
                Version = "14",
                ReplicationRole = "Primary",
                AvailabilityZone = p.env_FlexibleServer_AvailabilityZone,
                HighAvailability = new Pulumi.AzureNative.DBforPostgreSQL.Inputs.HighAvailabilityArgs
                {
                    Mode = "Disabled",
                },
                Storage = new Pulumi.AzureNative.DBforPostgreSQL.Inputs.StorageArgs
                {
                    StorageSizeGB = int.Parse(p.env_FlexibleServer_Storage),
                },
                Backup = new Pulumi.AzureNative.DBforPostgreSQL.Inputs.BackupArgs
                {
                    BackupRetentionDays = 7,
                    GeoRedundantBackup = "Disabled"
                },
                AuthConfig = new Pulumi.AzureNative.DBforPostgreSQL.Inputs.AuthConfigArgs
                {
                    ActiveDirectoryAuth = Pulumi.AzureNative.DBforPostgreSQL.ActiveDirectoryAuthEnum.Enabled,
                    PasswordAuth = Pulumi.AzureNative.DBforPostgreSQL.PasswordAuthEnum.Enabled,
                    TenantId = p.tenantId
                },
                DataEncryption = new Pulumi.AzureNative.DBforPostgreSQL.Inputs.DataEncryptionArgs
                {
                    Type = "SystemManaged"
                },
                Tags = { { "env", p.env } },
            }, new CustomResourceOptions { Protect = bool.Parse(p.env_General_ResourceProtection) });

            // Enable connections from Azure resources
            addFirewallRule(pulumiName, "azure", "AccessByAzureServices", "0.0.0.0", "0.0.0.0", rg);

            // Potentially enable connections from the Engineering team
            if (p.env_FlexibleServer_FirewallRules == "true")
            {
                addDarenaIPs(pulumiName, rg);
            }

            addConfiguration("azureextensions", "azure.extensions", "PGCRYPTO,POSTGRES_FDW,PG_TRGM,AZURE_STORAGE", rg);
            addConfiguration("enableseqscan", "enable_seqscan", "Off", rg);
            addConfiguration("maxparallelworkerspergather", "max_parallel_workers_per_gather", "4", rg);
            addConfiguration("randompagecost", "random_page_cost", ".002", rg);
            addConfiguration("seqpagecost", "seq_page_cost", "10", rg);

            if(p.env_FlexibleServer_SetMaxConnections == "true")
            {
                addConfiguration("maxconnections", "max_connections", p.env_FlexibleServer_MaxConnections, rg);
            }

            if (p.env_FlexibleServer_TrackActivityQuerySize == "true")
            {
                addConfiguration("trackactivityquerysize", "track_activity_query_size", "51200", rg);
            }

            Name = psqlServer.Name;
            FullyQualifiedDomainName = psqlServer.FullyQualifiedDomainName;
        }

        private void addFirewallRule(string pulumiName, string resourceName, string ruleName, string startIp, string endIp, MeldRx.Infrastructure.ResourceGroup rg)
        {
            var fw_rule = new Pulumi.AzureNative.DBforPostgreSQL.FirewallRule($"{pulumiName}-fw-rule-{resourceName}", new()
            {
                FirewallRuleName = ruleName,
                StartIpAddress = startIp,
                EndIpAddress = endIp,
                ResourceGroupName = rg.Name,
                ServerName = psqlServer.Name,
            });
        }

        private void addDarenaIPs(string pulumiName, MeldRx.Infrastructure.ResourceGroup rg)
        {
            var darenaIPs = File.ReadAllLines("darena-database-fw-ip-allowlist.txt")
                                .Where(t => t.Length > 0)
                                .Where(t => !t.Contains("//"))
                                .ToArray();

            foreach (var ip in darenaIPs)
            {
                var ipData = ip.Split(',');

                addFirewallRule(pulumiName, ipData[0], ipData[1], ipData[2], ipData[3], rg);
            }
        }

        private void addConfiguration(string pulumiName, string configurationName, string configurationValue, MeldRx.Infrastructure.ResourceGroup rg)
        {
            var configuration = new Pulumi.AzureNative.DBforPostgreSQL.Configuration($"psql-config-{pulumiName}", new()
            {
                ConfigurationName = configurationName,
                Value = configurationValue,
                ServerName = psqlServer.Name,
                ResourceGroupName = rg.Name,
                Source = "user-override",
            });
        }
    }
}
