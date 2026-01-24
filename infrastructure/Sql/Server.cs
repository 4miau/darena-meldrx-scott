using Pulumi;
using System.IO;
using System.Linq;

namespace MeldRx.Infrastructure.Sql
{
    public class Server
    {
        public Pulumi.AzureNative.Sql.Server sqlServer { get; set; }
        public MeldRx.Infrastructure.Sql.ElasticPool epool { get; set; }
        public Output<string> Name { get; set; }

        private ProjectConfig p = new ProjectConfig();

        public Server(string pulumiName, string resourceName, Output<string> password, Pulumi.ProviderResource provider, MeldRx.Infrastructure.Storage.StorageAccount sa, MeldRx.Infrastructure.ResourceGroup rg)
        {
            sqlServer = new Pulumi.AzureNative.Sql.Server(pulumiName, new()
            {
                ResourceGroupName = rg.Name,
                ServerName = $"sqlserver-{resourceName}-{p.env}",
                AdministratorLogin = p.sqlServerLogin,
                AdministratorLoginPassword = password,
                Administrators = new Pulumi.AzureNative.Sql.Inputs.ServerExternalAdministratorArgs
                {
                    AdministratorType = Pulumi.AzureNative.Sql.AdministratorType.ActiveDirectory,
                    AzureADOnlyAuthentication = true,
                    Login = p.env_SqlServer_Login,
                    PrincipalType = p.env_SqlServer_PrincipalType,
                    Sid = p.servicePrincipalId,
                    TenantId = p.tenantId,
                },
                Identity = new Pulumi.AzureNative.Sql.Inputs.ResourceIdentityArgs
                {
                    Type = Pulumi.AzureNative.Sql.IdentityType.SystemAssigned
                },
                MinimalTlsVersion = "1.2",
                RestrictOutboundNetworkAccess = "Disabled",
                Tags = { { "env", p.env } }
            }, new CustomResourceOptions { Protect = bool.Parse(p.env_General_ResourceProtection) });

            // Creates the Elastic Pool
            epool = new MeldRx.Infrastructure.Sql.ElasticPool(pulumiName, resourceName, sqlServer, rg);

            // Creates the Database Auditing Policy
            createDatabaseAuditingPolicy(pulumiName, sa, rg);

            // Adds the ability for AAD Login
            addSqlServerToAADReadersRole(pulumiName, provider);

            // Enable connections from Azure resources
            addFirewallRule(pulumiName, "azure", "AccessByAzureServices", "0.0.0.0", "0.0.0.0", rg);

            // Potentially enable connections from the Engineering team
            if (p.env_SqlServer_FirewallRules == "true")
            {
                addDarenaIPs(pulumiName, rg);
            }

            Name = sqlServer.Name;
        }

        private void createDatabaseAuditingPolicy(string pulumiName, MeldRx.Infrastructure.Storage.StorageAccount sa, MeldRx.Infrastructure.ResourceGroup rg)
        {
            // Azure SQL audit policy
            var sqlserver_audit = new Pulumi.AzureNative.Sql.ExtendedServerBlobAuditingPolicy($"{pulumiName}-audit", new()
            {
                BlobAuditingPolicyName = "default",
                ServerName = sqlServer.Name,
                ResourceGroupName = rg.Name,
                IsManagedIdentityInUse = false,
                StorageAccountSubscriptionId = p.subscriptionId,
                AuditActionsAndGroups = new[]
                {
                    "SUCCESSFUL_DATABASE_AUTHENTICATION_GROUP",
                    "FAILED_DATABASE_AUTHENTICATION_GROUP",
                    "BATCH_COMPLETED_GROUP",
                    "APPLICATION_ROLE_CHANGE_PASSWORD_GROUP",
                    "BACKUP_RESTORE_GROUP",
                    "DATABASE_LOGOUT_GROUP",
                    "DATABASE_OBJECT_CHANGE_GROUP",
                    "DATABASE_OBJECT_OWNERSHIP_CHANGE_GROUP",
                    "DATABASE_OBJECT_PERMISSION_CHANGE_GROUP",
                    "DATABASE_OPERATION_GROUP",
                    "DATABASE_PERMISSION_CHANGE_GROUP",
                    "DATABASE_PRINCIPAL_CHANGE_GROUP",
                    "DATABASE_PRINCIPAL_IMPERSONATION_GROUP",
                    "DATABASE_ROLE_MEMBER_CHANGE_GROUP",
                    "FAILED_DATABASE_AUTHENTICATION_GROUP",
                    "SCHEMA_OBJECT_ACCESS_GROUP",
                    "SCHEMA_OBJECT_CHANGE_GROUP",
                    "SCHEMA_OBJECT_OWNERSHIP_CHANGE_GROUP",
                    "SCHEMA_OBJECT_PERMISSION_CHANGE_GROUP",
                    "SUCCESSFUL_DATABASE_AUTHENTICATION_GROUP",
                    "USER_CHANGE_PASSWORD_GROUP",
                    "BATCH_STARTED_GROUP",
                    "BATCH_COMPLETED_GROUP",
                },
                IsAzureMonitorTargetEnabled = true,
                IsStorageSecondaryKeyInUse = false,
                PredicateExpression = "object_name = 'SensitiveData'",
                QueueDelayMs = 4000,
                RetentionDays = 365,
                State = Pulumi.AzureNative.Sql.BlobAuditingPolicyState.Enabled,
                StorageAccountAccessKey = sa.PrimaryKey,
                StorageEndpoint = sa.PrimaryBlobEndpoint,
            }, new CustomResourceOptions { Protect = false });
        }

        private void addSqlServerToAADReadersRole(string pulumiName, Pulumi.ProviderResource provider)
        {
            // Add SQL Server Managed Identity to Azure AD Directory Readers role
            var sqlServerRole = new Pulumi.AzureAD.DirectoryRoleAssignment($"{pulumiName}-role", new()
            {
                PrincipalObjectId = sqlServer.Identity.Apply(v => v.PrincipalId ?? ""),
                RoleId = "88d8e3e3-8f55-4a1e-953a-9b9898b8876b",
            }, new CustomResourceOptions() { Provider = provider });
        }

        private void addFirewallRule(string pulumiName, string resourceName, string ruleName, string startIp, string endIp, MeldRx.Infrastructure.ResourceGroup rg)
        {
            var fw_rule = new Pulumi.AzureNative.Sql.FirewallRule($"{pulumiName}-fw-rule-{resourceName}", new()
            {
                FirewallRuleName = ruleName,
                StartIpAddress = startIp,
                EndIpAddress = endIp,
                ResourceGroupName = rg.Name,
                ServerName = sqlServer.Name,
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
    }
}
