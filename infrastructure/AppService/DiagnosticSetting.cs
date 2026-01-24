using Pulumi;

namespace MeldRx.Infrastructure.AppService
{
    public class DiagnosticSetting
    {
        public Pulumi.AzureNative.Insights.DiagnosticSetting diagnosticSetting { get; set; }

        private ProjectConfig p = new ProjectConfig();

        public DiagnosticSetting(string pulumiName, string resourceName, Pulumi.AzureNative.Web.WebApp app, MeldRx.Infrastructure.LogAnalytics.Workspace log, MeldRx.Infrastructure.ResourceGroup rg)
        {
            diagnosticSetting = new Pulumi.AzureNative.Insights.DiagnosticSetting($"diagnosticSetting-{pulumiName}", new()
            {
                Name = resourceName,
                ResourceUri = app.Id,
                WorkspaceId = log.Id,
                LogAnalyticsDestinationType = null,
                Logs = new[]
                {
                    new Pulumi.AzureNative.Insights.Inputs.LogSettingsArgs
                    {
                        Category = "AppServiceAntivirusScanAuditLogs",
                        Enabled = true,
                        RetentionPolicy = new Pulumi.AzureNative.Insights.Inputs.RetentionPolicyArgs
                        {
                            Days = 0,
                            Enabled = false,
                        },
                    },
                    new Pulumi.AzureNative.Insights.Inputs.LogSettingsArgs
                    {
                        Category = "AppServiceHTTPLogs",
                        Enabled = true,
                        RetentionPolicy = new Pulumi.AzureNative.Insights.Inputs.RetentionPolicyArgs
                        {
                            Days = 0,
                            Enabled = false,
                        },
                    },
                    new Pulumi.AzureNative.Insights.Inputs.LogSettingsArgs
                    {
                        Category = "AppServiceConsoleLogs",
                        Enabled = true,
                        RetentionPolicy = new Pulumi.AzureNative.Insights.Inputs.RetentionPolicyArgs
                        {
                            Days = 0,
                            Enabled = false,
                        },
                    },
                    new Pulumi.AzureNative.Insights.Inputs.LogSettingsArgs
                    {
                        Category = "AppServiceAppLogs",
                        Enabled = true,
                        RetentionPolicy = new Pulumi.AzureNative.Insights.Inputs.RetentionPolicyArgs
                        {
                            Days = 0,
                            Enabled = false,
                        },
                    },
                    new Pulumi.AzureNative.Insights.Inputs.LogSettingsArgs
                    {
                        Category = "AppServiceAuditLogs",
                        Enabled = true,
                        RetentionPolicy = new Pulumi.AzureNative.Insights.Inputs.RetentionPolicyArgs
                        {
                            Days = 0,
                            Enabled = false,
                        },
                    },
                    new Pulumi.AzureNative.Insights.Inputs.LogSettingsArgs
                    {
                        Category = "AppServiceIPSecAuditLogs",
                        Enabled = true,
                        RetentionPolicy = new Pulumi.AzureNative.Insights.Inputs.RetentionPolicyArgs
                        {
                            Days = 0,
                            Enabled = false,
                        },
                    },
                    new Pulumi.AzureNative.Insights.Inputs.LogSettingsArgs
                    {
                        Category = "AppServicePlatformLogs",
                        Enabled = true,
                        RetentionPolicy = new Pulumi.AzureNative.Insights.Inputs.RetentionPolicyArgs
                        {
                            Days = 0,
                            Enabled = false,
                        },
                    },
                    new Pulumi.AzureNative.Insights.Inputs.LogSettingsArgs
                    {
                        Category = "AppServiceAuthenticationLogs",
                        Enabled = true,
                        RetentionPolicy = new Pulumi.AzureNative.Insights.Inputs.RetentionPolicyArgs
                        {
                            Days = 0,
                            Enabled = false,
                        },
                    },
                    new Pulumi.AzureNative.Insights.Inputs.LogSettingsArgs
                    {
                        Category = "AppServiceFileAuditLogs",
                        Enabled = true,
                        RetentionPolicy = new Pulumi.AzureNative.Insights.Inputs.RetentionPolicyArgs
                        {
                            Days = 0,
                            Enabled = false,
                        },
                    },
                },
                Metrics = new[]
                {
                    new Pulumi.AzureNative.Insights.Inputs.MetricSettingsArgs
                    {
                        Category = "AllMetrics",
                        Enabled = true,
                        RetentionPolicy = new Pulumi.AzureNative.Insights.Inputs.RetentionPolicyArgs
                        {
                            Days = 0,
                            Enabled = false,
                        },
                    },
                }
            });
        }
    }
}
