using Pulumi;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using MeldRx.Infrastructure.FrontDoor;
using Pulumi.AzureNative.Web;
using Pulumi.AzureNative.Web.Inputs;

namespace MeldRx.Infrastructure.AppService
{
    public class App
    {
        public Pulumi.AzureNative.Web.WebApp app { get; set; }
        public string PulumiName { get; set; }
        public Output<string> Name { get; set; }
        public Output<string> Id { get; set; }
        public Output<string> Identity { get; set; }
        public Output<string> Url { get; set; }
        public MeldRx.Infrastructure.FrontDoor.Endpoint? ep { get; set; }
        public MeldRx.Infrastructure.FrontDoor.CustomDomain? domain { get; set; }
        public MeldRx.Infrastructure.FrontDoor.CustomDomain? domain_alt_1 { get; set; }
        private ProjectConfig p = new ProjectConfig();

        public App(
            string pulumiName,
            string resourceName,
            string domainName,
            string zoneName,
            MeldRx.Infrastructure.Provider.AzureNative provider,
            MeldRx.Infrastructure.FrontDoor.Ruleset? ruleset,
            MeldRx.Infrastructure.AppService.App? app_base,
            MeldRx.Infrastructure.FrontDoor.Profile? fd,
            MeldRx.Infrastructure.LogAnalytics.Workspace log,
            MeldRx.Infrastructure.AppService.Plan plan,
            MeldRx.Infrastructure.ResourceGroup rg,
            List<MeldRx.Infrastructure.AppService.App>? allow_connections_from = null,
            MeldRx.Infrastructure.Storage.StorageAccount? sa = null,
            MeldRx.Infrastructure.Storage.BlobContainer? container = null
        )
        {
            PulumiName = pulumiName;
            // Apps behind Azure FrontDoor
            app = new Pulumi.AzureNative.Web.WebApp($"app-{pulumiName}", new()
            {
                Name = $"app-{p.projectName}-{resourceName}-{p.env}",
                ResourceGroupName = rg.Name,
                HttpsOnly = true,
                ClientAffinityEnabled = true,
                // ClientAffinityProxyEnabled = (pulumiName == "app" ? true : false),
                ServerFarmId = plan.Id,
                Kind = "app,linux,container",
                Identity = new Pulumi.AzureNative.Web.Inputs.ManagedServiceIdentityArgs
                {
                    Type = Pulumi.AzureNative.Web.ManagedServiceIdentityType.SystemAssigned
                },
                SiteConfig = new Pulumi.AzureNative.Web.Inputs.SiteConfigArgs
                {
                    AlwaysOn = true,
                    FtpsState = "Disabled",
                    Http20Enabled = true,
                    HttpLoggingEnabled = true,
                    NetFrameworkVersion = "v4.0",
                    MinTlsVersion = "1.3",
                    Use32BitWorkerProcess = false,
                    AppCommandLine = "",
                    AutoHealEnabled = false,
                    DetailedErrorLoggingEnabled = false,
                    DefaultDocuments = new[]
                    {
                        "Default.htm",
                        "Default.html",
                        "Default.asp",
                        "index.htm",
                        "index.html",
                        "iisstart.htm",
                        "default.aspx",
                        "index.php",
                        "hostingstart.html",
                    },
                    ElasticWebAppScaleLimit = 0,
                    Experiments = new Pulumi.AzureNative.Web.Inputs.ExperimentsArgs
                    {
                        RampUpRules = []
                    },
                    FunctionsRuntimeScaleMonitoringEnabled = false,
                    LoadBalancing = Pulumi.AzureNative.Web.SiteLoadBalancing.LeastRequests,
                    LogsDirectorySizeLimit = 35,
                    ManagedPipelineMode = Pulumi.AzureNative.Web.ManagedPipelineMode.Integrated,
                    MinimumElasticInstanceCount = 0,
                    NodeVersion = "",
                    PhpVersion = "",
                    PowerShellVersion = "",
                    PreWarmedInstanceCount = 0,
                    PublishingUsername = $"$app-{p.projectName}-{resourceName}-{p.env}",
                    PythonVersion = "",
                    RemoteDebuggingEnabled = false,
                    RequestTracingEnabled = false,
                    ScmIpSecurityRestrictions = new[]
                    {
                        new Pulumi.AzureNative.Web.Inputs.IpSecurityRestrictionArgs
                        {
                            Action = "Allow",
                            Description = "Allow all access",
                            IpAddress = "Any",
                            Name = "Allow all",
                            Priority = 2147483647,
                        },
                    },
                    ScmIpSecurityRestrictionsUseMain = false,
                    ScmMinTlsVersion = "1.2",
                    ScmType = "None",
                    VirtualApplications = new[]
                    {
                        new Pulumi.AzureNative.Web.Inputs.VirtualApplicationArgs
                        {
                            PhysicalPath = "site\\wwwroot",
                            PreloadEnabled = true,
                            VirtualPath = "/",
                        },
                    },
                    VnetName = "",
                    VnetPrivatePortsCount = 0,
                    VnetRouteAllEnabled = false,
                    WebSocketsEnabled = false,
                    Cors = new Pulumi.AzureNative.Web.Inputs.CorsSettingsArgs
                    {
                        AllowedOrigins = "*",
                        SupportCredentials = false
                    },
                    IpSecurityRestrictions = getIpSecurityRestrictions(fd, allow_connections_from),
                    AzureStorageAccounts = sa != null && container != null
                        ? new()
                        {
                            [container.PulumiName] = new Pulumi.AzureNative.Web.Inputs.AzureStorageInfoValueArgs
                            {
                                AccessKey = sa.PrimaryKey,
                                AccountName = sa.Name,
                                MountPath = "/usr/src/app/config",
                                ShareName = container.container.Name,
                                Type = AzureStorageType.AzureBlob,
                            }
                        }
                        : default!
                },
                VnetContentShareEnabled = false,
                VnetImagePullEnabled = false,
                VnetRouteAllEnabled = false,
                Tags = { { "env", p.env } }
            },
            new CustomResourceOptions { IgnoreChanges = { "siteConfig.linuxFxVersion", "tags" } });

            Name = app.Name;
            Id = app.Id;
            Identity = app.Identity.Apply(id => { return id?.PrincipalId ?? ""; });
            Url = app.Name.Apply(v => $"{v}.azurewebsites.net");

            if (fd != null)
            {
                createFrontDoorApplication(pulumiName, resourceName, domainName, zoneName, provider, app_base, ruleset, fd, plan, rg);
            }

            if(log != null)
            {
                new MeldRx.Infrastructure.AppService.DiagnosticSetting(pulumiName, resourceName, app, log, rg);
            }
        }

        private InputList<IpSecurityRestrictionArgs> getIpSecurityRestrictions(Profile? fd, List<App>? allow_connections_from)
        {
            InputList<IpSecurityRestrictionArgs> list = new();
            list.Add(
                new Pulumi.AzureNative.Web.Inputs.IpSecurityRestrictionArgs
                {
                    IpAddress = "AzureKeyVault",
                    Action = "Allow",
                    Tag = "ServiceTag",
                    Priority = 200,
                    Name = "AzureKeyVault",
                }
            );

            list.Add(
                new Pulumi.AzureNative.Web.Inputs.IpSecurityRestrictionArgs
                {
                    IpAddress = "Any",
                    Action = "Deny",
                    Priority = 2147483647,
                    Name = "Deny all",
                    Description = "Deny all access"
                }
            );

            if (fd != null)
            {
                list.Add(new Pulumi.AzureNative.Web.Inputs.IpSecurityRestrictionArgs
                {
                    IpAddress = "AzureFrontDoor.Backend",
                    Action = "Allow",
                    Tag = "ServiceTag",
                    Priority = 100,
                    Name = "MyAzureFrontDoorRule",
                    Headers = fd.FrontDoorId.Apply(v =>
                        {
                            return new Dictionary<string, ImmutableArray<string>>
                            {
                                {"x-azure-fdid", ImmutableArray.Create(new string[1] {v})}
                            };
                        }
                    ),
                });
            }

            if (allow_connections_from is { Count: > 0 })
            {
                foreach (var app in allow_connections_from)
                {
                    var appIpRules = app.app.OutboundIpAddresses
                        .Apply(allIps =>
                            allIps.Split(',').Select(ip =>
                                new Pulumi.AzureNative.Web.Inputs.IpSecurityRestrictionArgs
                                {
                                    IpAddress = $"{ip}/32",
                                    Action = "Allow",
                                    Tag = IpFilterTag.Default,
                                    Priority = 110,
                                    Name = $"sa:{app.PulumiName}:{ip}",
                                }
                            )
                        );

                    list.AddRange(appIpRules);
                }
            }

            return list;
        }

        private void createFrontDoorApplication(string pulumiName, string resourceName, string domainName, string zoneName, MeldRx.Infrastructure.Provider.AzureNative provider, MeldRx.Infrastructure.AppService.App? app_base,
            MeldRx.Infrastructure.FrontDoor.Ruleset? ruleset, MeldRx.Infrastructure.FrontDoor.Profile fd, MeldRx.Infrastructure.AppService.Plan plan, MeldRx.Infrastructure.ResourceGroup rg)
        {
            var og = new MeldRx.Infrastructure.FrontDoor.OriginGroup(pulumiName, resourceName, fd, rg);
            var origin = new MeldRx.Infrastructure.FrontDoor.Origin(pulumiName, resourceName, this.Url, og, fd, rg);

            if (pulumiName != "ccda" && pulumiName != "developer" && pulumiName != "mymipsscore")
            {
                ep = new MeldRx.Infrastructure.FrontDoor.Endpoint(pulumiName, resourceName, fd, rg);
                domain = new MeldRx.Infrastructure.FrontDoor.CustomDomain(domainName, domainName, zoneName, fd, rg);
                var CNAME = new MeldRx.Infrastructure.Dns.CNAME(domainName, domainName, zoneName, provider, ep);
                var TXT = new MeldRx.Infrastructure.Dns.TXT(domainName, domainName, zoneName, provider, domain);

                if (pulumiName.Contains("app") && p.env_FD_IncludeLegacyRedirects == "true")
                {
                    domain_alt_1 = new MeldRx.Infrastructure.FrontDoor.CustomDomain("hub", "hub", zoneName, fd, rg);
                    var CNAME_alt_1 = new MeldRx.Infrastructure.Dns.CNAME("hub", "hub", zoneName, provider, ep);
                    var TXT_alt_1 = new MeldRx.Infrastructure.Dns.TXT("hub", "hub", zoneName, provider, domain_alt_1);

                    var domains = new List<MeldRx.Infrastructure.FrontDoor.CustomDomain> { domain, domain_alt_1 };
                    var route = new MeldRx.Infrastructure.FrontDoor.Route(pulumiName, resourceName, false, ep, og, domains, ruleset, fd, rg);
                }
                else
                {
                    var domains = new List<MeldRx.Infrastructure.FrontDoor.CustomDomain> { domain };
                    var route = new MeldRx.Infrastructure.FrontDoor.Route(pulumiName, resourceName, false, ep, og, domains, ruleset, fd, rg);
                }
            }
            else
            {
                var domains = new List<MeldRx.Infrastructure.FrontDoor.CustomDomain> { app_base.domain };
                var route = new MeldRx.Infrastructure.FrontDoor.Route(pulumiName, resourceName, false, app_base.ep, og, domains, null, fd, rg);
            }
        }
    }
}
