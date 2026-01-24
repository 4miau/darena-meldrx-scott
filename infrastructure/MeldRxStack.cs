using Pulumi;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using MeldRx.Infrastructure.Models;

class MeldRxStack : Stack
{
    public ProjectConfig p = new ProjectConfig();

    #region Outputs
    [Output]
    public Output<string> AppCcdaName { get; private set; }
    [Output]
    public Output<string> AppDeveloperName { get; private set; }
    [Output]
    public Output<string> AppMeldRxName { get; private set; }
    [Output]
    public Output<string> AppMyMipsScoreName { get; private set; }
    [Output]
    public Output<string> AppCqlServiceName { get; private set; }
    [Output]
    public Output<string> AppCqlCompilerName { get; private set; }
    [Output]
    public Output<string> AppCqlTranslationName { get; private set; }
    [Output]
    public Output<string> KeyVaultName { get; private set; }
    [Output]
    public Output<string> PostgresDbServerName { get; private set; }
    [Output]
    public Output<string> SubscriptionId { get; private set; }
    #endregion

    public MeldRxStack()
    {
        // DNS Zone Constants
        const string meldrx = "meldrx.com";

        // Passwords
        var sqlserver_admin_password = new MeldRx.Infrastructure.Random.Password("sqlserver-admin-password");
        var postgres_admin_password = new MeldRx.Infrastructure.Random.Password("postgres-admin-password");
        var postgres_readonly_password = new MeldRx.Infrastructure.Random.Password("postgres-readonly-password");
        var encryption_key = new MeldRx.Infrastructure.Random.Password("encryption-key");
        var postgres_update_app_client_secret = new MeldRx.Infrastructure.Random.Password("postgres-update-app-client-secret");

        // Registered Providers
        var provider = new MeldRx.Infrastructure.Provider.AzureNative("provider", p.subscriptionId);
        var provider_dns = new MeldRx.Infrastructure.Provider.AzureNative("provider-dns", "de7299f2-f699-4644-a64f-0b93f8448564");
        var provider_aad = new MeldRx.Infrastructure.Provider.AzureAD("provider-aad");

        // Azure Resource Group
        var rg = new MeldRx.Infrastructure.ResourceGroup("rg", p.projectName, provider.provider);

        // Storage Account - Azure Data Lake
        var sa = new MeldRx.Infrastructure.Storage.StorageAccount("sa", p.projectName, false, true, true, rg);
        var defenderForStorage = new MeldRx.Infrastructure.Security.DefenderForStorage("defenderforstorage", sa, rg);
        var container = new MeldRx.Infrastructure.Storage.BlobContainer("container", p.env_Storage_ContainerName, false, sa, rg);
        var container_cql_config = new MeldRx.Infrastructure.Storage.BlobContainer("cql-config", "cql-config", false, sa, rg, folders: new List<string[]>() { new[] { "libraries" }, new[] { "hooks" }, });

        // For public EHR Branding
        var sapublic = new MeldRx.Infrastructure.Storage.StorageAccount("sapublic", p.projectName, true, false, true, rg);
        var sapublic_properties = new MeldRx.Infrastructure.Storage.BlobServiceProperties("properties-sapublic", "default", sapublic, rg);
        var container_public = new MeldRx.Infrastructure.Storage.BlobContainer("container-public", "assets", true, sapublic, rg);

        // Azure SQL Server
        var sqlServer = new MeldRx.Infrastructure.Sql.Server("sqlserver", p.projectName, sqlserver_admin_password.Result, provider_aad.provider, sa, rg);

        // SQL Server Databases
        var db_iam = new MeldRx.Infrastructure.Sql.Database("iam", "iam", sqlServer, rg);

        //Azure Cache for Redis
        var redis = new MeldRx.Infrastructure.Cache.Redis("redis", p.projectName, rg);

        // Postgres Flexible Server
        var psqlserver = new MeldRx.Infrastructure.Postgres.FlexibleServer("psql-flex", p.projectName, postgres_admin_password, rg);
        var backupVault = new MeldRx.Infrastructure.DataProtection.BackupVault("backupVault", p.projectName, psqlserver, rg);
        // Log Analytics Workspace
        var log = new MeldRx.Infrastructure.LogAnalytics.Workspace("log", p.projectName, rg);

        // Azure Front Door (CDN)
        var fd = new MeldRx.Infrastructure.FrontDoor.Profile("fd", p.projectName, log, rg);

        // App Insights
        var appi = new MeldRx.Infrastructure.LogAnalytics.ApplicationInsights("appi", p.projectName, log, rg);

        // App Service Plan
        var plan = new MeldRx.Infrastructure.AppService.Plan("plan", p.projectName, int.Parse(p.env_AppServicePlan_Apps_SKU_Capacity), rg);

        // MeldRx - App Service
        var app_meldrx = new MeldRx.Infrastructure.AppService.App("app", "app", p.env_App_AppName, meldrx, provider_dns, fd.security_and_redirect_ruleset ?? fd.security_ruleset, null, fd, log, plan, rg);
        var app_ccda = new MeldRx.Infrastructure.AppService.App("ccda", "ccda", "ccda", meldrx, provider_dns, fd.security_ruleset, app_meldrx, fd, log, plan, rg);
        var app_developer = new MeldRx.Infrastructure.AppService.App("developer", "developer", "developer", meldrx, provider_dns, fd.security_ruleset, app_meldrx, fd, log, plan, rg);
        var app_mymipsscore = new MeldRx.Infrastructure.AppService.App("mymipsscore", "mymipsscore", "mymipsscore", meldrx, provider_dns, fd.security_ruleset, app_meldrx, fd, log, plan, rg);
        var app_cql = new MeldRx.Infrastructure.AppService.App("cql", "cql", "cql", meldrx, provider_dns, fd.security_ruleset, app_meldrx, null, log, plan, rg, [app_meldrx], sa, container_cql_config);
        var app_cql_compiler = new MeldRx.Infrastructure.AppService.App("cql-compiler", "cql-compiler", "cql-compiler", meldrx, provider_dns, fd.security_ruleset, app_meldrx, null, log, plan, rg, [app_meldrx]);
        var app_cql_translation = new MeldRx.Infrastructure.AppService.App("cql-translation", "cql-translation", "cql-translation", meldrx, provider_dns, fd.security_ruleset, app_meldrx, null, log, plan, rg, [app_cql_compiler]);

        // Azure Service Bus
        var sb = new MeldRx.Infrastructure.ServiceBus.Namespace("sb", p.projectName, rg);
        var queue = new MeldRx.Infrastructure.ServiceBus.Queue("queue", "usage-audit", sb, rg);

        // List of applications for KeyVault
        var kvAppsList = new List<MeldRx.Infrastructure.AppService.App> { app_meldrx, app_developer, app_mymipsscore, app_ccda, app_cql, app_cql_compiler };

        // List of applications for FrontDoor WAF Policy
        var fdAppsList = new List<MeldRx.Infrastructure.AppService.App> { app_meldrx };

        // WAF Policy
        var waf_policy = new MeldRx.Infrastructure.FrontDoor.WafPolicy("waf-policy", p.projectName, rg);

        // Azure Front Door WAF Security Policy
        var fd_waf = new MeldRx.Infrastructure.FrontDoor.Waf("fd-waf", p.projectName, fdAppsList, waf_policy, fd, rg);

        // Azure KeyVault
        var kv = new MeldRx.Infrastructure.KeyVault.Vault("kv", p.projectName, kvAppsList, log, rg);

        // MeldRx App Service Config Sections
        const string CqlConfigStoreSettings = "FileStoreSettings__CqlConfig__";
        const string MeldRxSettings = "MeldRxSettings__";
        const string ChplSettings = "ChplSettings__";
        const string FeatureManagement = "FeatureManagement__";
        const string ExternalAuthenticationGoogle = "ExternalAuthentication__Google";
        const string ExternalAuthenticationMeta = "ExternalAuthentication__Meta";
        const string ExternalAuthenticationGitHub = "ExternalAuthentication__GitHub";
        const string ExternalAuthenticationTwitter = "ExternalAuthentication__Twitter";
        const string HangfireSettings = "HangfireSettings__";
        const string AuditSettings = "MeldRxSettings__AuditSettings__";
        const string StripeBillingSettings = "StripeBillingSettings__";

        // MMS App Service Config Sections
        const string DsMyMipsScoreV2 = "DsMyMipsScoreV2__";
        const string ApiV2Settings = "DsMyMipsScoreV2__ApiV2Settings__";

        // MeldRx KeyVault Secrets
        var secret_AdlsConnectionString = new MeldRx.Infrastructure.KeyVault.Secret("AdlsConnectionString", sa.PrimaryConnectionString, kv, rg);
        var secret_PublicStorageConnectionString = new MeldRx.Infrastructure.KeyVault.Secret("PublicStorageConnectionString", sapublic.PrimaryConnectionString, kv, rg);
        var secret_AzureServiceBusConnectionString = new MeldRx.Infrastructure.KeyVault.Secret("AzureServiceBusConnectionString", sb.PrimaryConnectionString, kv, rg);
        var secret_ChplSettings_ApiKey = new MeldRx.Infrastructure.KeyVault.Secret("ChplSettingsApiKey", p.chplSettings_ApiKey, kv, rg);
        var secret_CmsClientSecret = new MeldRx.Infrastructure.KeyVault.Secret("CmsClientSecret", p.cmsClientSecret, kv, rg);
        var secret_DOCKER_REGISTRY_SERVER_PASSWORD = new MeldRx.Infrastructure.KeyVault.Secret("DOCKERREGISTRYSERVERPASSWORD", p.DOCKER_REGISTRY_SERVER_PASSWORD, kv, rg);
        var secret_EncryptionKey = new MeldRx.Infrastructure.KeyVault.Secret("EncryptionKey", encryption_key.Result, kv, rg);
        var secret_MasterDbConnectionString = new MeldRx.Infrastructure.KeyVault.Secret("MasterDbConnectionString", sqlServer.Name.Apply(v => $"Server={v}.database.windows.net; Authentication=Active Directory Service Principal; Database=master; User Id={p.servicePrincipalId}; Password={p.servicePrincipalSecret}"), kv, rg);
        var secret_MeldRxDbConnectionString = new MeldRx.Infrastructure.KeyVault.Secret("MeldRxDbConnectionString", Output.Tuple(sqlServer.Name, db_iam.Name).Apply(v => $"Server={v.Item1}.database.windows.net; Authentication=Active Directory Service Principal; Database={v.Item2}; User Id={p.servicePrincipalId}; Password={p.servicePrincipalSecret}"), kv, rg);
        var secret_PostgresDbConnectionString = new MeldRx.Infrastructure.KeyVault.Secret("PostgresDbConnectionString", Output.Tuple(psqlserver.Name, psqlserver.FullyQualifiedDomainName, postgres_admin_password.Result).Apply(v => $"Server={v.Item2}; User Id={p.sqlServerLogin}; Database={{dbName}}; Port=5432; Password={v.Item3};SSLMode=Require; Trust Server Certificate = true; Command Timeout = 120; Timeout = 300;{(p.env_General_EnableErrorDetails == "true" ? "Include Error Detail=true;" : "")}"), kv, rg);
        var secret_PostgresDbPassword = new MeldRx.Infrastructure.KeyVault.Secret("PostgresDbPassword", postgres_admin_password.Result, kv, rg);
        var secret_PostgresReadOnlyDbPassword = new MeldRx.Infrastructure.KeyVault.Secret("PostgresReadOnlyDbPassword", postgres_readonly_password.Result, kv, rg);
        var secret_RedisConnectionString = new MeldRx.Infrastructure.KeyVault.Secret("RedisConnectionString", Output.Tuple(redis.Name, redis.PrimaryKey).Apply(v => $"{v.Item1}.redis.cache.windows.net:6380,password={v.Item2},ssl=True,abortConnect=False,connectTimeout=10000,syncTimeout=10000"), kv, rg);
        var secret_SendGridApiKey = new MeldRx.Infrastructure.KeyVault.Secret("SendGridApiKey", p.sendGridApiKey, kv, rg);
        var secret_SeqApiKey = new MeldRx.Infrastructure.KeyVault.Secret("SeqApiKey", p.seqApiKey, kv, rg);
        var secret_socialClientSecret_GitHub = new MeldRx.Infrastructure.KeyVault.Secret("SocialClientSecretGitHub", p.socialClientSecret_GitHub, kv, rg);
        var secret_socialClientSecret_Google = new MeldRx.Infrastructure.KeyVault.Secret("SocialClientSecretGoogle", p.socialClientSecret_Google, kv, rg);
        var secret_socialClientSecret_Meta = new MeldRx.Infrastructure.KeyVault.Secret("SocialClientSecretMeta", p.socialClientSecret_Meta, kv, rg);
        var secret_socialClientSecret_Twitter = new MeldRx.Infrastructure.KeyVault.Secret("SocialClientSecretTwitter", p.socialClientSecret_Twitter, kv, rg);
        var secret_SqlServerAdminPassword = new MeldRx.Infrastructure.KeyVault.Secret("SqlServerAdminPassword", sqlserver_admin_password.Result, kv, rg);
        var secret_StripeApiKey = new MeldRx.Infrastructure.KeyVault.Secret("ApiKey", p.stripeApiKey, kv, rg);
        var secret_StripeWebhookSecret = new MeldRx.Infrastructure.KeyVault.Secret("WebhookSecret", p.stripeWebhookSecret, kv, rg);
        var secret_SuperAdminPassword = new MeldRx.Infrastructure.KeyVault.Secret("SuperAdminPassword", p.superAdminPassword, kv, rg);
        var secret_UmlsApiKey = new MeldRx.Infrastructure.KeyVault.Secret("UmlsApiKey", p.umlsApiKey, kv, rg);

        // MMS KeyVault Secrets
        var secret_CmsDataApiAppToken = new MeldRx.Infrastructure.KeyVault.Secret("CmsDataApiAppToken", p.cmsDataApiAppToken, kv, rg);
        var secret_HealthItGovApiKey = new MeldRx.Infrastructure.KeyVault.Secret("HealthItGovApiKey", p.healthItGovApiKey, kv, rg);
        var secret_MipsPostgresDbConnectionString = new MeldRx.Infrastructure.KeyVault.Secret("MipsPostgresDbConnectionString", Output.Tuple(psqlserver.Name, psqlserver.FullyQualifiedDomainName, postgres_admin_password.Result).Apply(v => $"Server={v.Item2}; User Id={p.sqlServerLogin}; Database=mips; Port=5432; Password={v.Item3}; SSLMode=Require; Persist Security Info = True; Trust Server Certificate = true; Maximum Pool Size=250; Command Timeout = 120; Timeout = 300;{(p.env_General_EnableErrorDetails == "true" ? "Include Error Detail=true;" : "")}"), kv, rg);
        var secret_QppSubmissionsApiToken = new MeldRx.Infrastructure.KeyVault.Secret("QppSubmissionsApiToken", p.qppSubmissionsApiToken, kv, rg);

        // For the postgresql-update-all-workspaces
        var secret_PostgresUpdateAppClientSecret = new MeldRx.Infrastructure.KeyVault.Secret("PostgresUpdateAppClientSecret", postgres_update_app_client_secret.Result, kv, rg);

        // MMS Api V2 App Service Configs
        var config_AuthorityUrlApi = new MeldRx.Infrastructure.AppService.Config(ApiV2Settings, "Authority", Output.Create(customDomainNameBuilder(p.env_App_AppName, meldrx, true)));
        var config_CmsDataApiUrl = new MeldRx.Infrastructure.AppService.Config(ApiV2Settings, "CmsDataApiUrl", Output.Create("https://data.cms.gov/resource/hyj4-55ic.json"));
        var config_HealthItGovUrl = new MeldRx.Infrastructure.AppService.Config(ApiV2Settings, "HealthItGovUrl", Output.Create("https://chpl.healthit.gov/rest"));
        var config_NpiRegistryUrl = new MeldRx.Infrastructure.AppService.Config(ApiV2Settings, "NpiRegistryUrl", Output.Create("https://npiregistry.cms.hhs.gov/api"));
        var config_QppSubmissionsApiUrl = new MeldRx.Infrastructure.AppService.Config(ApiV2Settings, "QppSubmissionsApiUrl", Output.Create(p.qppSubmissionsApiUrl));

        // MMS React App Configs
        var config_REACT_APP_API_URL = new MeldRx.Infrastructure.AppService.Config("", "REACT_APP_API_URL", Output.Create($"{customDomainNameBuilder(p.env_App_AppName, meldrx, true)}/mms-api"));
        var config_REACT_APP_AUTHORITY = new MeldRx.Infrastructure.AppService.Config("", "REACT_APP_AUTHORITY", Output.Create(customDomainNameBuilder(p.env_App_AppName, meldrx, true)));
        var config_REACT_APP_CLIENT_ID = new MeldRx.Infrastructure.AppService.Config("", "REACT_APP_CLIENT_ID", Output.Create("mymipsscore-react-client"));
        var config_REACT_APP_MMS_ENV = new MeldRx.Infrastructure.AppService.Config("", "REACT_APP_MMS_ENV", Output.Create(p.ASPNETCORE_ENVIRONMENT));
        var config_REACT_APP_POST_LOGOUT_REDIRECT_URL = new MeldRx.Infrastructure.AppService.Config("", "REACT_APP_POST_LOGOUT_REDIRECT_URL",  Output.Create($"{customDomainNameBuilder(p.env_App_AppName, meldrx, true)}/mymipsscore"));
        var config_REACT_APP_REDIRECT_URL = new MeldRx.Infrastructure.AppService.Config("", "REACT_APP_REDIRECT_URL", Output.Create($"{customDomainNameBuilder(p.env_App_AppName, meldrx, true)}/mymipsscore?callback"));
        var config_REACT_APP_SCORE_PREVIEW_TESTING_ENABLED = new MeldRx.Infrastructure.AppService.Config("", "REACT_APP_SCORE_PREVIEW_TESTING_ENABLED", Output.Create("true"));
        var config_REACT_APP_SILENT_REDIRECT_URL = new MeldRx.Infrastructure.AppService.Config("", "REACT_APP_SILENT_REDIRECT_URL", Output.Create($"{customDomainNameBuilder(p.env_App_AppName, meldrx, true)}/mymipsscore?silent"));

        // MeldRx App Service Configs
        var config_AppInsightsConnectionString = new MeldRx.Infrastructure.AppService.Config("", "APPLICATIONINSIGHTS_CONNECTION_STRING", appi.InstrumentationKey.Apply(key => $"InstrumentationKey={key}"));
        var config_AppInsightsKey = new MeldRx.Infrastructure.AppService.Config("", "APPINSIGHTS_INSTRUMENTATIONKEY", appi.InstrumentationKey);
        var config_AppInsightsVersion = new MeldRx.Infrastructure.AppService.Config("", "ApplicationInsightsAgent_EXTENSION_VERSION", Output.Create("~3"));
        var config_ASPNETCORE_ENVIRONMENT = new MeldRx.Infrastructure.AppService.Config("", "ASPNETCORE_ENVIRONMENT", Output.Create(p.ASPNETCORE_ENVIRONMENT));
        var config_AspnetEnvironment = new MeldRx.Infrastructure.AppService.Config(MeldRxSettings, "AspnetEnvironment", Output.Create(p.ASPNETCORE_ENVIRONMENT));
        var config_AuditSettings_Enabled = new MeldRx.Infrastructure.AppService.Config(AuditSettings, "Enabled", Output.Create("true"));
        var config_AuthorityUrl = new MeldRx.Infrastructure.AppService.Config(MeldRxSettings, "AuthorityUrl", Output.Create(customDomainNameBuilder(p.env_App_AppName, meldrx, true)));
        var config_AutoApproveOrganizationRequest = new MeldRx.Infrastructure.AppService.Config(MeldRxSettings, "AutoApproveOrganizationRequest", Output.Create(p.env_AutoApproveOrganizationRequest));
        var config_CmsAuthorityUrl = new MeldRx.Infrastructure.AppService.Config(MeldRxSettings, "CmsAuthorityUrl", Output.Create(p.cmsBaseUrl));
        var config_CmsAuthorizationEndpoint = new MeldRx.Infrastructure.AppService.Config(MeldRxSettings, "CmsAuthorizationEndpoint", Output.Create((p.cmsBaseUrl) + "/v2/o/authorize/"));
        var config_CmsClientId = new MeldRx.Infrastructure.AppService.Config(MeldRxSettings, "CmsClientId", Output.Create(p.cmsClientId));
        var config_CmsFhirEndpoint = new MeldRx.Infrastructure.AppService.Config(MeldRxSettings, "CmsFhirEndpoint", Output.Create((p.cmsBaseUrl) + "/v2/fhir"));
        var config_CmsTokenEndpoint = new MeldRx.Infrastructure.AppService.Config(MeldRxSettings, "CmsTokenEndpoint", Output.Create((p.cmsBaseUrl) + "/v2/o/token/"));
        var config_CmsUserInfoEndpoint = new MeldRx.Infrastructure.AppService.Config(MeldRxSettings, "CmsUserInfoEndpoint", Output.Create((p.cmsBaseUrl) + "/v2/connect/userinfo"));
        var config_CookieDomain_App = new MeldRx.Infrastructure.AppService.Config(MeldRxSettings, "CookieDomain", Output.Create(customDomainNameBuilder("app", meldrx, false)));
        var config_CqlCompilerAddress = new MeldRx.Infrastructure.AppService.Config(MeldRxSettings, "CqlCompilerAddress", app_cql_compiler.app.DefaultHostName.Apply(host => $"https://{host}"));
        var config_CqlConfigStore_ConnectionStringPath = new MeldRx.Infrastructure.AppService.Config(CqlConfigStoreSettings, "ConnectionStringPath", Output.Create("MeldRxSettings:AdlsConnectionString"));
        var config_CqlConfigStore_Container = new MeldRx.Infrastructure.AppService.Config(CqlConfigStoreSettings, "Container__0", Output.Create("cql-config"));
        var config_CqlConfigStore_Type = new MeldRx.Infrastructure.AppService.Config(CqlConfigStoreSettings, "Type", Output.Create("AdlsFileStore"));
        var config_CqlServiceAddress = new MeldRx.Infrastructure.AppService.Config(MeldRxSettings, "CqlServiceAddress", app_cql.app.DefaultHostName.Apply(host => $"https://{host}"));
        var config_DefaultEmailTemplateId = new MeldRx.Infrastructure.AppService.Config(MeldRxSettings, "DefaultEmailTemplateId", Output.Create(p.defaultEmailTemplateId));
        var config_DefaultOrganizationUserInviteTemplateId = new MeldRx.Infrastructure.AppService.Config(MeldRxSettings, "DefaultOrganizationUserInviteTemplateId", Output.Create(p.defaultOrganizationUserInviteTemplateId));
        var config_DefaultPersonInviteEmailTemplateId = new MeldRx.Infrastructure.AppService.Config(MeldRxSettings, "DefaultPersonInviteEmailTemplateId", Output.Create(p.defaultPersonInviteEmailTemplateId));
        var config_DistributedCacheKeyPrefix = new MeldRx.Infrastructure.AppService.Config(MeldRxSettings, "DistributedCacheKeyPrefix", Output.Create(p.env.ToUpper() + "-"));
        var config_FeatureManagementIsCaptchaEnabled = new MeldRx.Infrastructure.AppService.Config(FeatureManagement, "IsCaptchaEnabled", Output.Create(p.feature_IsCaptchaEnabled));
        var config_HangfireIsDashboardOnly = new MeldRx.Infrastructure.AppService.Config(HangfireSettings, "IsDashboardOnly", Output.Create("false"));
        var config_HangfireJobQueues = new MeldRx.Infrastructure.AppService.Config(HangfireSettings, "JobQueues", Output.Create(getClientIds($"JobQueues/{p.env}/jobs.txt")));
        var config_HangfireRetryAttempts = new MeldRx.Infrastructure.AppService.Config(HangfireSettings, "RetryAttempts", Output.Create("2"));
        var config_HangfireServerName = new MeldRx.Infrastructure.AppService.Config(HangfireSettings, "ServerName", Output.Create(p.env_App_AppName));
        var config_HangfireWorkerCount = new MeldRx.Infrastructure.AppService.Config(HangfireSettings, "WorkerCount", Output.Create("5"));
        var config_InternalNotificationsEmailAddress = new MeldRx.Infrastructure.AppService.Config(MeldRxSettings, "InternalNotificationsEmailAddress", Output.Create("support@darenasolutions.com"));
        var config_MeldRxSmartAppUrl = new MeldRx.Infrastructure.AppService.Config(MeldRxSettings, "MeldRxSmartAppUrl", Output.Create(p.meldRxSmartAppUrl));
        var config_PostgresHost = new MeldRx.Infrastructure.AppService.Config(MeldRxSettings, "PostgresDbHost", psqlserver.FullyQualifiedDomainName);
        var config_PostgresDbUsername = new MeldRx.Infrastructure.AppService.Config(MeldRxSettings, "PostgresDbUsername", Output.Create(p.sqlServerLogin));
        var config_QueueService = new MeldRx.Infrastructure.AppService.Config(MeldRxSettings, "QueueService", Output.Create("AzureServiceBusQueue"));
        var config_RunHangfireServer = new MeldRx.Infrastructure.AppService.Config(MeldRxSettings, "RunHangfireServer", Output.Create(p.env_Jobs_RunHangfireServer));
        var config_SendGridApiBaseUrl = new MeldRx.Infrastructure.AppService.Config(MeldRxSettings, "SendGridApiBaseUrl", Output.Create(p.sendGridApiBaseUrl));
        var config_SendGridDisplayName = new MeldRx.Infrastructure.AppService.Config(MeldRxSettings, "SendGridDisplayName", Output.Create(p.sendGridDisplayName));
        var config_SendGridFromAddress = new MeldRx.Infrastructure.AppService.Config(MeldRxSettings, "SendGridFromAddress", Output.Create("support@meldrx.com"));
        var config_SeqUrl = new MeldRx.Infrastructure.AppService.Config(MeldRxSettings, "SeqUrl", Output.Create(p.seqUrl));
        var config_socialClientId_GitHub = new MeldRx.Infrastructure.AppService.Config(ExternalAuthenticationGitHub, "ClientId", Output.Create(p.socialClientId_GitHub));
        var config_socialClientId_Google = new MeldRx.Infrastructure.AppService.Config(ExternalAuthenticationGoogle, "ClientId", Output.Create(p.socialClientId_Google));
        var config_socialClientId_Meta = new MeldRx.Infrastructure.AppService.Config(ExternalAuthenticationMeta, "ClientId", Output.Create(p.socialClientId_Meta));
        var config_socialClientId_Twitter = new MeldRx.Infrastructure.AppService.Config(ExternalAuthenticationTwitter, "ClientId", Output.Create(p.socialClientId_Twitter));
        var config_socialEnabled_GitHub = new MeldRx.Infrastructure.AppService.Config(ExternalAuthenticationGitHub, "Enabled", Output.Create(p.socialEnabled_GitHub));
        var config_socialEnabled_Google = new MeldRx.Infrastructure.AppService.Config(ExternalAuthenticationGoogle, "Enabled", Output.Create(p.socialEnabled_Google));
        var config_socialEnabled_Meta = new MeldRx.Infrastructure.AppService.Config(ExternalAuthenticationMeta, "Enabled", Output.Create(p.socialEnabled_Meta));
        var config_socialEnabled_Twitter = new MeldRx.Infrastructure.AppService.Config(ExternalAuthenticationTwitter, "Enabled", Output.Create(p.socialEnabled_Twitter));
        var config_StripeApiBase = new MeldRx.Infrastructure.AppService.Config(StripeBillingSettings, "ApiBase", Output.Create(p.stripeApiBase));
        var config_StripeConnectBase = new MeldRx.Infrastructure.AppService.Config(StripeBillingSettings, "ConnectBase", Output.Create(p.stripeConnectBase));
        var config_StripeFilesBase = new MeldRx.Infrastructure.AppService.Config(StripeBillingSettings, "FilesBase", Output.Create(p.stripeFilesBase));
        var config_SuperAdminEmail = new MeldRx.Infrastructure.AppService.Config(MeldRxSettings, "SuperAdminEmail", Output.Create("admin@meldrx.com"));
        var config_SupportUrl = new MeldRx.Infrastructure.AppService.Config(MeldRxSettings, "SupportUrl", Output.Create("https://support.meldrx.com"));
        var config_workspaceLinkedApps = p.workspaceLinkedApps
            .SelectMany((x, index) =>
                new[]
                {
                    new MeldRx.Infrastructure.AppService.Config(MeldRxSettings, $"WorkspaceLinkedApps__{index}__{nameof(WorkspaceLinkedAppSetting.ChplIds)}", Output.Create(x.ChplIds)),
                    new MeldRx.Infrastructure.AppService.Config(MeldRxSettings, $"WorkspaceLinkedApps__{index}__{nameof(WorkspaceLinkedAppSetting.ClientId)}", Output.Create(x.ClientId)),
                    new MeldRx.Infrastructure.AppService.Config(MeldRxSettings, $"WorkspaceLinkedApps__{index}__{nameof(WorkspaceLinkedAppSetting.Provider)}", Output.Create(x.Provider)),
                }
            )
            .ToList();

        // Cql Compiler App Service Config
        var config_CqlToElmUrl = new MeldRx.Infrastructure.AppService.Config("", "CQL_TO_ELM_URL", app_cql_translation.app.DefaultHostName.Apply(host => $"https://{host}/cql/translator"));
        var config_CqlFormatterUrl = new MeldRx.Infrastructure.AppService.Config("", "CQL_FORMATTER_URL", app_cql_translation.app.DefaultHostName.Apply(host => $"https://{host}/cql/formatter"));

        // Cql Compiler Translation Service Config
        var config_CQL_TRANSLATION_DOCKER_CUSTOM_IMAGE_NAME = new MeldRx.Infrastructure.AppService.Config("", "DOCKER_CUSTOM_IMAGE_NAME", Output.Create($"cqframework/cql-translation-service:{p.cqlTranslationImageVersion}"));

        // Container-specific configurations
        var config_ASPNETCORE_FORWARDEDHEADERS_ENABLED = new MeldRx.Infrastructure.AppService.Config("", "ASPNETCORE_FORWARDEDHEADERS_ENABLED", Output.Create("true"));
        var config_ASPNETCORE_HTTPS_PORT = new MeldRx.Infrastructure.AppService.Config("", "ASPNETCORE_HTTPS_PORT", Output.Create("443"));
        var config_CONTAINER_ENV = new MeldRx.Infrastructure.AppService.Config("", "NUXT_PUBLIC_CONTAINER_ENV", Output.Create(p.env));
        var config_CONTAINER_TAG = new MeldRx.Infrastructure.AppService.Config("", "NUXT_PUBLIC_CONTAINER_TAG", Output.Create("main"));
        var config_STORAGE_URL = new MeldRx.Infrastructure.AppService.Config("", "NUXT_PUBLIC_STORAGE_URL", sapublic.PrimaryBlobEndpoint.Apply(url => $"{url}assets"));
        var config_DOCKER_REGISTRY_SERVER_URL = new MeldRx.Infrastructure.AppService.Config("", "DOCKER_REGISTRY_SERVER_URL", Output.Create("acrmeldrx.azurecr.io"));
        var config_OFFICIAL_DOCKER_REGISTRY_SERVER_URL = new MeldRx.Infrastructure.AppService.Config("", "DOCKER_REGISTRY_SERVER_URL", Output.Create("docker.io"));
        var config_DOCKER_REGISTRY_SERVER_USERNAME = new MeldRx.Infrastructure.AppService.Config("", "DOCKER_REGISTRY_SERVER_USERNAME", Output.Create("acrmeldrx"));
        var config_HTTP_PORTS = new MeldRx.Infrastructure.AppService.Config("", "HTTP_PORTS", Output.Create("80"));
        var config_NODE_ENV = new MeldRx.Infrastructure.AppService.Config("", "NODE_ENV", Output.Create("production"));
        var config_PORT = new MeldRx.Infrastructure.AppService.Config("", "PORT", Output.Create("80"));
        var config_WEBSITES_PORT = new MeldRx.Infrastructure.AppService.Config("", "WEBSITES_PORT", Output.Create("80"));

        var settings_app_meldrx = new MeldRx.Infrastructure.AppService.Settings("app", app_meldrx, rg,
            new List<MeldRx.Infrastructure.AppService.Config> {
                new MeldRx.Infrastructure.AppService.Config("", "DOCKER_REGISTRY_SERVER_PASSWORD", secret_DOCKER_REGISTRY_SERVER_PASSWORD),
                new MeldRx.Infrastructure.AppService.Config(ChplSettings, "ApiKey", secret_ChplSettings_ApiKey),
                new MeldRx.Infrastructure.AppService.Config(ExternalAuthenticationGitHub, "ClientSecret", secret_socialClientSecret_GitHub),
                new MeldRx.Infrastructure.AppService.Config(ExternalAuthenticationGoogle, "ClientSecret", secret_socialClientSecret_Google),
                new MeldRx.Infrastructure.AppService.Config(ExternalAuthenticationMeta, "ClientSecret", secret_socialClientSecret_Meta),
                new MeldRx.Infrastructure.AppService.Config(ExternalAuthenticationTwitter, "ClientSecret", secret_socialClientSecret_Twitter),
                new MeldRx.Infrastructure.AppService.Config(MeldRxSettings, "AdlsConnectionString", secret_AdlsConnectionString),
                new MeldRx.Infrastructure.AppService.Config(MeldRxSettings, "PublicStorageConnectionString", secret_PublicStorageConnectionString),
                new MeldRx.Infrastructure.AppService.Config(MeldRxSettings, "AzureServiceBusConnectionString", secret_AzureServiceBusConnectionString),
                new MeldRx.Infrastructure.AppService.Config(MeldRxSettings, "CmsClientSecret", secret_CmsClientSecret),
                new MeldRx.Infrastructure.AppService.Config(MeldRxSettings, "EncryptionKey", secret_EncryptionKey),
                new MeldRx.Infrastructure.AppService.Config(MeldRxSettings, "HangfireDbConnectionString", secret_RedisConnectionString),
                new MeldRx.Infrastructure.AppService.Config(MeldRxSettings, "MeldRxDbConnectionString", secret_MeldRxDbConnectionString),
                new MeldRx.Infrastructure.AppService.Config(MeldRxSettings, "PostgresDbConnectionString", secret_PostgresDbConnectionString),
                new MeldRx.Infrastructure.AppService.Config(MeldRxSettings, "PostgresDbPassword", secret_PostgresDbPassword),
                new MeldRx.Infrastructure.AppService.Config(MeldRxSettings, "RedisConnectionString", secret_RedisConnectionString),
                new MeldRx.Infrastructure.AppService.Config(MeldRxSettings, "SendGridApiKey", secret_SendGridApiKey),
                new MeldRx.Infrastructure.AppService.Config(MeldRxSettings, "SeqApiKey", secret_SeqApiKey),
                new MeldRx.Infrastructure.AppService.Config(MeldRxSettings, "SuperAdminPassword", secret_SuperAdminPassword),
                new MeldRx.Infrastructure.AppService.Config(MeldRxSettings, "UmlsApiKey", secret_UmlsApiKey),
                new MeldRx.Infrastructure.AppService.Config(StripeBillingSettings, "ApiKey", secret_StripeApiKey),
                new MeldRx.Infrastructure.AppService.Config(StripeBillingSettings, "WebhookSecret", secret_StripeWebhookSecret),
                new MeldRx.Infrastructure.AppService.Config(ApiV2Settings, "CmsDataApiAppToken", secret_CmsDataApiAppToken),
                new MeldRx.Infrastructure.AppService.Config(ApiV2Settings, "HealthItGovApiKey", secret_HealthItGovApiKey),
                new MeldRx.Infrastructure.AppService.Config(ApiV2Settings, "QppSubmissionsApiToken", secret_QppSubmissionsApiToken),
                new MeldRx.Infrastructure.AppService.Config(ApiV2Settings, "StorageConStr", secret_AdlsConnectionString),
                new MeldRx.Infrastructure.AppService.Config(DsMyMipsScoreV2, "PostgresConnectionString", secret_MipsPostgresDbConnectionString),
            },
            new List<MeldRx.Infrastructure.AppService.Config> {
                config_AppInsightsConnectionString,
                config_AppInsightsKey,
                config_AppInsightsVersion,
                config_ASPNETCORE_ENVIRONMENT,
                config_ASPNETCORE_HTTPS_PORT,
                config_DOCKER_REGISTRY_SERVER_URL,
                config_DOCKER_REGISTRY_SERVER_USERNAME,
                config_HTTP_PORTS,
                config_AspnetEnvironment,
                config_AuditSettings_Enabled,
                config_AuthorityUrl,
                config_AutoApproveOrganizationRequest,
                config_CmsAuthorityUrl,
                config_CmsAuthorizationEndpoint,
                config_CmsClientId,
                config_CmsFhirEndpoint,
                config_CmsTokenEndpoint,
                config_CmsUserInfoEndpoint,
                config_CookieDomain_App,
                config_CqlConfigStore_ConnectionStringPath,
                config_CqlConfigStore_Container,
                config_CqlConfigStore_Type,
                config_CqlServiceAddress,
                config_CqlCompilerAddress,
                config_DefaultEmailTemplateId,
                config_DefaultOrganizationUserInviteTemplateId,
                config_DefaultPersonInviteEmailTemplateId,
                config_DistributedCacheKeyPrefix,
                config_FeatureManagementIsCaptchaEnabled,
                config_HangfireIsDashboardOnly,
                config_HangfireJobQueues,
                config_HangfireRetryAttempts,
                config_HangfireServerName,
                config_HangfireWorkerCount,
                config_InternalNotificationsEmailAddress,
                config_MeldRxSmartAppUrl,
                config_PostgresHost,
                config_PostgresDbUsername,
                config_QueueService,
                config_RunHangfireServer,
                config_SendGridApiBaseUrl,
                config_SendGridDisplayName,
                config_SendGridFromAddress,
                config_SeqUrl,
                config_socialClientId_GitHub,
                config_socialClientId_Google,
                config_socialClientId_Meta,
                config_socialClientId_Twitter,
                config_socialEnabled_GitHub,
                config_socialEnabled_Google,
                config_socialEnabled_Meta,
                config_socialEnabled_Twitter,
                config_StripeApiBase,
                config_StripeConnectBase,
                config_StripeFilesBase,
                config_SuperAdminEmail,
                config_SupportUrl,
                config_AuthorityUrlApi,
                config_CmsDataApiUrl,
                config_HealthItGovUrl,
                config_NpiRegistryUrl,
                config_QppSubmissionsApiUrl,
            }
            .Concat(config_workspaceLinkedApps)
            .ToList()
        );

        var settings_app_ccda = new MeldRx.Infrastructure.AppService.Settings("ccda", app_ccda, rg,
            new List<MeldRx.Infrastructure.AppService.Config> {
                new MeldRx.Infrastructure.AppService.Config("", "DOCKER_REGISTRY_SERVER_PASSWORD", secret_DOCKER_REGISTRY_SERVER_PASSWORD),
                new MeldRx.Infrastructure.AppService.Config("ConnectionStrings__", "Default", secret_MeldRxDbConnectionString),
            },
            new List<MeldRx.Infrastructure.AppService.Config> {
                config_AppInsightsConnectionString,
                config_AppInsightsKey,
                config_AppInsightsVersion,
                config_ASPNETCORE_ENVIRONMENT,
                config_ASPNETCORE_HTTPS_PORT,
                config_DOCKER_REGISTRY_SERVER_URL,
                config_DOCKER_REGISTRY_SERVER_USERNAME,
                config_HTTP_PORTS,
            });

        var settings_app_developer = new MeldRx.Infrastructure.AppService.Settings("developer", app_developer, rg,
            new List<MeldRx.Infrastructure.AppService.Config> {
                new MeldRx.Infrastructure.AppService.Config("", "DOCKER_REGISTRY_SERVER_PASSWORD", secret_DOCKER_REGISTRY_SERVER_PASSWORD)
            },
            new List<MeldRx.Infrastructure.AppService.Config> {
                config_AppInsightsConnectionString,
                config_AppInsightsKey,
                config_AppInsightsVersion,
                config_CONTAINER_ENV,
                config_CONTAINER_TAG,
                config_DOCKER_REGISTRY_SERVER_URL,
                config_DOCKER_REGISTRY_SERVER_USERNAME,
                config_HTTP_PORTS,
                config_PORT,
                config_NODE_ENV,
                config_STORAGE_URL
            });

        var settings_app_mymipsscore = new MeldRx.Infrastructure.AppService.Settings("mymipsscore", app_mymipsscore, rg,
            new List<MeldRx.Infrastructure.AppService.Config> {
                new MeldRx.Infrastructure.AppService.Config("", "DOCKER_REGISTRY_SERVER_PASSWORD", secret_DOCKER_REGISTRY_SERVER_PASSWORD)
            },
            new List<MeldRx.Infrastructure.AppService.Config> {
                config_AppInsightsConnectionString,
                config_AppInsightsKey,
                config_AppInsightsVersion,
                config_DOCKER_REGISTRY_SERVER_URL,
                config_DOCKER_REGISTRY_SERVER_USERNAME,
                config_HTTP_PORTS,
                config_REACT_APP_API_URL,
                config_REACT_APP_AUTHORITY,
                config_REACT_APP_CLIENT_ID,
                config_REACT_APP_MMS_ENV,
                config_REACT_APP_POST_LOGOUT_REDIRECT_URL,
                config_REACT_APP_REDIRECT_URL,
                config_REACT_APP_SCORE_PREVIEW_TESTING_ENABLED,
                config_REACT_APP_SILENT_REDIRECT_URL,
            });

        var settings_cql_service = new MeldRx.Infrastructure.AppService.Settings("cql_service", app_cql, rg,
            new List<MeldRx.Infrastructure.AppService.Config> {
                new MeldRx.Infrastructure.AppService.Config("", "DOCKER_REGISTRY_SERVER_PASSWORD", secret_DOCKER_REGISTRY_SERVER_PASSWORD),
                new MeldRx.Infrastructure.AppService.Config("", "UMLS_API_KEY", secret_UmlsApiKey)
            },
            new List<MeldRx.Infrastructure.AppService.Config> {
                config_AppInsightsConnectionString,
                config_AppInsightsKey,
                config_AppInsightsVersion,
                config_DOCKER_REGISTRY_SERVER_URL,
                config_DOCKER_REGISTRY_SERVER_USERNAME,
                config_HTTP_PORTS,
                config_PORT,
                config_WEBSITES_PORT,
                config_NODE_ENV,
            });

        var settings_cql_compiler = new MeldRx.Infrastructure.AppService.Settings("cql_compiler", app_cql_compiler, rg,
            [
                new MeldRx.Infrastructure.AppService.Config("", "DOCKER_REGISTRY_SERVER_PASSWORD", secret_DOCKER_REGISTRY_SERVER_PASSWORD),
            ],
            [
                config_AppInsightsConnectionString,
                config_AppInsightsKey,
                config_AppInsightsVersion,
                config_DOCKER_REGISTRY_SERVER_URL,
                config_DOCKER_REGISTRY_SERVER_USERNAME,
                config_HTTP_PORTS,
                config_PORT,
                config_WEBSITES_PORT,
                config_NODE_ENV,
                config_CqlToElmUrl,
                config_CqlFormatterUrl,
            ]
        );

        var settings_cql_translation = new MeldRx.Infrastructure.AppService.Settings("cql_translation", app_cql_translation, rg,
            [],
            [
                config_AppInsightsConnectionString,
                config_AppInsightsKey,
                config_AppInsightsVersion,
                config_OFFICIAL_DOCKER_REGISTRY_SERVER_URL,
                config_CQL_TRANSLATION_DOCKER_CUSTOM_IMAGE_NAME,
            ]
        );

        // Outputs
        AppCcdaName = app_ccda.Name.Apply(v => v);
        AppDeveloperName = app_developer.Name.Apply(v => v);
        AppMeldRxName = app_meldrx.Name.Apply(v => v);
        AppMyMipsScoreName = app_mymipsscore.Name.Apply(v => v);
        AppCqlServiceName = app_cql.Name.Apply(v => v);
        AppCqlCompilerName = app_cql_compiler.Name.Apply(v => v);
        AppCqlTranslationName = app_cql_translation.Name.Apply(v => v);
        KeyVaultName = kv.Name.Apply(v => v);
        PostgresDbServerName = psqlserver.FullyQualifiedDomainName.Apply(v => v);
        SubscriptionId = Output.Create(p.subscriptionId);
    }

    private string getClientIds(string fileName)
    {
        var clientIds = "";

        foreach (var line in File.ReadAllLines(fileName).Where(t => t.Length > 0).Where(t => !t.Contains("//")))
        {
            if (clientIds == "")
            {
                clientIds += line;
            }
            else
            {
                clientIds += $",{line}";
            }
        }

        return clientIds;
    }

    private string customDomainNameBuilder(string subDomainName, string domainName, bool includeHttps)
    {
        return $"{(includeHttps ? "https://" : "")}{subDomainName}{(p.env_URL_ShowEnvName == "true" ? $".{p.env}" : "")}.{domainName}";
    }
}
