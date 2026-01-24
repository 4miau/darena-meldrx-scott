using System.Collections.Generic;
using MeldRx.Infrastructure.Models;

class ProjectConfig
{
    // Read config data
    private static Pulumi.Config config = new Pulumi.Config();

    // Global config
    public string ASPNETCORE_ENVIRONMENT { get; set; }
    public string chplSettings_ApiKey { get; set; }
    public string cmsBaseUrl { get; set; }
    public string cmsClientId { get; set; }
    public string cmsClientSecret { get; set; }
    public string csp { get; set; }
    public string defaultEmailTemplateId { get; set; }
    public string defaultOrganizationUserInviteTemplateId { get; set; }
    public string defaultPersonInviteEmailTemplateId { get; set; }
    public string DOCKER_REGISTRY_SERVER_PASSWORD { get; set; }
    public string env { get; set; }
    public string env_AppServicePlan_Apps_SKU_Capacity { get; set; }
    public string env_AppServicePlan_Apps_SKU_Family { get; set; }
    public string env_AppServicePlan_Apps_SKU_Name { get; set; }
    public string env_AppServicePlan_Apps_SKU_Size { get; set; }
    public string env_AppServicePlan_Apps_SKU_Tier { get; set; }
    public string env_App_AppName { get; set; }
    public string env_AutoApproveOrganizationRequest { get; set; }
    public string env_FD_IncludeRules { get; set; }
    public string env_FD_IncludeLegacyRedirects { get; set; }
    public string env_FlexibleServer_SKU_Name { get; set; }
    public string env_FlexibleServer_SKU_Tier { get; set; }
    public string env_FlexibleServer_AvailabilityZone { get; set; }
    public string env_FlexibleServer_Storage { get; set; }
    public string env_FlexibleServer_SetMaxConnections { get; set; }
    public string env_FlexibleServer_MaxConnections { get; set; }
    public string env_FlexibleServer_TrackActivityQuerySize { get; set; }
    public string env_FlexibleServer_FirewallRules { get; set; }
    public string env_General_EnableErrorDetails { get; set; }
    public string env_General_ResourceProtection { get; set; }
    public string env_Jobs_RunHangfireServer { get; set; }
    public string env_PIMKVGroupId { get; set; }
    public string env_Redis_Family { get; set; }
    public string env_Redis_Name { get; set; }
    public string env_Redis_Persistence_Enabled { get; set; }
    public string env_ServiceBus_Zone_Redundant { get; set; }
    public string env_SqlServer_Database_SKU_Tier { get; set; }
    public string env_SqlServer_Database_MaxSizeInBytes { get; set; }
    public string env_SqlServer_ElasticPool_MaxCapacity { get; set; }
    public string env_SqlServer_ElasticPool_SKU_Capacity { get; set; }
    public string env_SqlServer_ElasticPool_SKU_Name { get; set; }
    public string env_SqlServer_ElasticPool_SKU_Tier { get; set; }
    public string env_SqlServer_Login { get; set; }
    public string env_SqlServer_PrincipalType { get; set; }
    public string env_SqlServer_FirewallRules { get; set; }
    public string env_Storage_ContainerName { get; set; }
    public string env_URL_ShowEnvName { get; set; }
    public string feature_IsCaptchaEnabled { get; set; }
    public string meldRxSmartAppUrl { get; set;}
    public string location { get; set; }
    public string logLevel { get; set; }
    public string partnerId { get; set; }
    public string projectName { get; set; }
    public string sendGridApiKey { get; set; }
    public string sendGridDisplayName { get; set; }
    public string sendGridApiBaseUrl { get; set; }
    public string seqApiKey { get; set; }
    public string seqUrl { get; set; }
    public string servicePrincipalId { get; set; }
    public string servicePrincipalObjectId { get; set; }
    public string servicePrincipalSecret { get; set; }
    public string socialClientId_GitHub { get; set; }
    public string socialClientId_Google { get; set; }
    public string socialClientId_Meta { get; set; }
    public string socialClientId_Twitter { get; set; }
    public string socialClientSecret_GitHub { get; set; }
    public string socialClientSecret_Google { get; set; }
    public string socialClientSecret_Meta { get; set; }
    public string socialClientSecret_Twitter { get; set; }
    public string socialEnabled_GitHub { get; set; }
    public string socialEnabled_Google { get; set; }
    public string socialEnabled_Meta { get; set; }
    public string socialEnabled_Twitter { get; set; }
    public string sqlServerLogin { get; set; }
    public string stripeApiBase { get; set; }
    public string stripeApiKey { get; set; }
    public string stripeConnectBase { get; set; }
    public string stripeFilesBase { get; set; }
    public string stripeWebhookSecret { get; set; }
    public string subscriptionId { get; set; }
    public string superAdminPassword { get; set; }
    public string supportUrl { get; set; }
    public string tenantId { get; set; }
    public List<WorkspaceLinkedAppSetting> workspaceLinkedApps { get; set; }

    // MMS
    public string cmsDataApiAppToken { get; set; }
    public string qppSubmissionsApiToken { get; set; }
    public string healthItGovApiKey { get; set; }
    public string qppSubmissionsApiUrl { get; set; }

    // CQL
    public string umlsApiKey { get; set; }
    public string cqlTranslationImageVersion { get; set; }

    public ProjectConfig()
    {
        projectName = config.Get("projectName") ?? "meldrx";
        ASPNETCORE_ENVIRONMENT = config.Get("ASPNETCORE_ENVIRONMENT") ?? "";
        chplSettings_ApiKey = config.Get("chplSettings_ApiKey") ?? "";
        cmsBaseUrl = config.Get("cmsBaseUrl") ?? "";
        cmsClientId = config.Get("cmsClientId") ?? "";
        cmsClientSecret = config.Get("cmsClientSecret") ?? "";
        csp = config.Get("csp") ?? "";
        defaultEmailTemplateId = config.Get("defaultEmailTemplateId") ?? "";
        defaultOrganizationUserInviteTemplateId = config.Get("defaultOrganizationUserInviteTemplateId") ?? "";
        defaultPersonInviteEmailTemplateId = config.Get("defaultPersonInviteEmailTemplateId") ?? "";
        DOCKER_REGISTRY_SERVER_PASSWORD = config.Get("DOCKER_REGISTRY_SERVER_PASSWORD") ?? "";
        env = config.Get("env") ?? "";
        meldRxSmartAppUrl = config.Get("meldRxSmartAppUrl") ?? "";
        env_AppServicePlan_Apps_SKU_Capacity = config.Get("env_AppServicePlan_Apps_SKU_Capacity") ?? "";
        env_AppServicePlan_Apps_SKU_Family = config.Get("env_AppServicePlan_Apps_SKU_Family") ?? "";
        env_AppServicePlan_Apps_SKU_Name = config.Get("env_AppServicePlan_Apps_SKU_Name") ?? "";
        env_AppServicePlan_Apps_SKU_Size = config.Get("env_AppServicePlan_Apps_SKU_Size") ?? "";
        env_AppServicePlan_Apps_SKU_Tier = config.Get("env_AppServicePlan_Apps_SKU_Tier") ?? "";
        env_App_AppName = config.Get("env_App_AppName") ?? "app";
        env_AutoApproveOrganizationRequest = config.Get("env_AutoApproveOrganizationRequest") ?? "false";
        env_FD_IncludeRules = config.Get("env_FD_IncludeRules") ?? "";
        env_FD_IncludeLegacyRedirects = config.Get("env_FD_IncludeLegacyRedirects") ?? "";
        env_FlexibleServer_SKU_Name = config.Get("env_FlexibleServer_SKU_Name") ?? "";
        env_FlexibleServer_SKU_Tier = config.Get("env_FlexibleServer_SKU_Tier") ?? "";
        env_FlexibleServer_AvailabilityZone = config.Get("env_FlexibleServer_AvailabilityZone") ?? "1";
        env_FlexibleServer_Storage = config.Get("env_FlexibleServer_Storage") ?? "";
        env_FlexibleServer_SetMaxConnections = config.Get("env_FlexibleServer_SetMaxConnections") ?? "false";
        env_FlexibleServer_MaxConnections = config.Get("env_FlexibleServer_MaxConnections") ?? "50";
        env_FlexibleServer_FirewallRules = config.Get("env_FlexibleServer_FirewallRules") ?? "false";
        env_General_EnableErrorDetails = config.Get("env_General_EnableErrorDetails") ?? "";
        env_FlexibleServer_TrackActivityQuerySize = config.Get("env_FlexibleServer_TrackActivityQuerySize") ?? "false";
        env_General_ResourceProtection = config.Get("env_General_ResourceProtection") ?? "";
        env_Jobs_RunHangfireServer = config.Get("env_Jobs_RunHangfireServer") ?? "";
        env_PIMKVGroupId = config.Get("env_PIMKVGroupId") ?? "";
        env_Redis_Family = config.Get("env_Redis_Family") ?? "";
        env_Redis_Name = config.Get("env_Redis_Name") ?? "";
        env_Redis_Persistence_Enabled = config.Get("env_Redis_Persistence_Enabled") ?? "false";
        env_ServiceBus_Zone_Redundant = config.Get("env_ServiceBus_Zone_Redundant") ?? "false";
        env_SqlServer_Database_SKU_Tier = config.Get("env_SqlServer_Database_SKU_Tier") ?? "";
        env_SqlServer_Database_MaxSizeInBytes = config.Get("env_SqlServer_Database_MaxSizeInBytes") ?? "2147483648";
        env_SqlServer_ElasticPool_MaxCapacity = config.Get("env_SqlServer_ElasticPool_MaxCapacity") ?? "";
        env_SqlServer_ElasticPool_SKU_Capacity = config.Get("env_SqlServer_ElasticPool_SKU_Capacity") ?? "";
        env_SqlServer_ElasticPool_SKU_Name = config.Get("env_SqlServer_ElasticPool_SKU_Name") ?? "";
        env_SqlServer_ElasticPool_SKU_Tier = config.Get("env_SqlServer_ElasticPool_SKU_Tier") ?? "";
        env_SqlServer_Login = config.Get("env_SqlServer_Login") ?? "";
        env_SqlServer_PrincipalType = config.Get("env_SqlServer_PrincipalType") ?? "";
        env_SqlServer_FirewallRules = config.Get("env_SqlServer_FirewallRules") ?? "";
        env_Storage_ContainerName = config.Get("env_Storage_ContainerName") ?? "";
        env_URL_ShowEnvName = config.Get("env_URL_ShowEnvName") ?? "";
        feature_IsCaptchaEnabled = config.Get("feature_IsCaptchaEnabled") ?? "";
        location = config.Get("location") ?? "";
        logLevel = config.Get("logLevel") ?? "";
        partnerId = config.Get("partnerId") ?? "";
        sendGridApiKey = config.Get("sendGridApiKey") ?? "";
        sendGridDisplayName = config.Get("sendGridDisplayName") ?? "";
        sendGridApiBaseUrl = config.Get("sendGridApiBaseUrl") ?? "";
        seqApiKey = config.Get("seqApiKey") ?? "";
        seqUrl = config.Get("seqUrl") ?? "";
        servicePrincipalId = config.Get("clientId") ?? "";
        servicePrincipalObjectId = config.Get("clientObjectId") ?? "";
        servicePrincipalSecret = config.Get("clientSecret") ?? "";
        socialClientId_GitHub = config.Get("socialClientId_GitHub") ?? "";
        socialClientId_Google = config.Get("socialClientId_Google") ?? "";
        socialClientId_Meta = config.Get("socialClientId_Meta") ?? "";
        socialClientId_Twitter = config.Get("socialClientId_Twitter") ?? "";
        socialClientSecret_GitHub = config.Get("socialClientSecret_GitHub") ?? "";
        socialClientSecret_Google = config.Get("socialClientSecret_Google") ?? "";
        socialClientSecret_Meta = config.Get("socialClientSecret_Meta") ?? "";
        socialClientSecret_Twitter = config.Get("socialClientSecret_Twitter") ?? "";
        socialEnabled_GitHub = config.Get("socialEnabled_GitHub") ?? "";
        socialEnabled_Google = config.Get("socialEnabled_Google") ?? "";
        socialEnabled_Meta = config.Get("socialEnabled_Meta") ?? "";
        socialEnabled_Twitter = config.Get("socialEnabled_Twitter") ?? "";
        sqlServerLogin = config.Get("sqlServerLogin") ?? "";
        stripeApiBase = config.Get("stripeApiBase") ?? "";
        stripeApiKey = config.Get("stripeApiKey") ?? "";
        stripeConnectBase = config.Get("stripeConnectBase") ?? "";
        stripeFilesBase = config.Get("stripeFilesBase") ?? "";
        stripeWebhookSecret = config.Get("stripeWebhookSecret") ?? "";
        subscriptionId = config.Get("subscriptionId") ?? "";
        superAdminPassword = config.Get("superAdminPassword") ?? "";
        supportUrl = config.Get("supportUrl") ?? "";
        tenantId = config.Get("tenantId") ?? "";
        umlsApiKey = config.Get("umlsApiKey") ?? "";
        workspaceLinkedApps = config.GetObject<List<WorkspaceLinkedAppSetting>>("workspaceLinkedApps") ?? [];
        // MMS
        cmsDataApiAppToken = config.Get("cmsDataApiAppToken") ?? "";
        cqlTranslationImageVersion = config.Get("cqlTranslationImageVersion") ?? "";
        qppSubmissionsApiToken = config.Get("qppSubmissionsApiToken") ?? "";
        healthItGovApiKey = config.Get("healthItGovApiKey") ?? "";
        qppSubmissionsApiUrl = config.Get("qppSubmissionsApiUrl") ?? "";
    }
}
