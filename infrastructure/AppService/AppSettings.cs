using Pulumi;
using System.Collections.Generic;
using System;

namespace MeldRx.Infrastructure.AppService
{
    public class Settings
    {
        public Pulumi.AzureNative.Web.WebAppApplicationSettings settings { get; set; }

        private ProjectConfig p = new ProjectConfig();

        public Settings(string pulumiName, MeldRx.Infrastructure.AppService.App app, MeldRx.Infrastructure.ResourceGroup rg,
            List<MeldRx.Infrastructure.AppService.Config> keyVaultReferences, List<MeldRx.Infrastructure.AppService.Config> configs)
        {
            var properties = buildProperties(keyVaultReferences, configs);

            // Uncomment for steady state
            settings = new Pulumi.AzureNative.Web.WebAppApplicationSettings($"settings-{pulumiName}",
                new()
                {
                    Name = app.Name,
                    ResourceGroupName = rg.Name,
                    Properties = properties
                },
                new CustomResourceOptions { IgnoreChanges = { "properties.NUXT_PUBLIC_CONTAINER_TAG" } }
            );

            // Uncomment for changes
            // settings = new Pulumi.AzureNative.Web.WebAppApplicationSettings($"settings-{pulumiName}", new()
            // {
            //     Name = app.Name,
            //     ResourceGroupName = rg.Name,
            //     Properties = properties
            // });
        }

        private InputMap<string> buildProperties(List<MeldRx.Infrastructure.AppService.Config> keyVaultReferences,
            List<MeldRx.Infrastructure.AppService.Config> configs)
        {
            var properties = new InputMap<string>();

            foreach (var config in keyVaultReferences)
            {
                properties.Add($"{config.SettingsPath}{config.ConfigName}",
                    config.SecretValue.secret.Properties.Apply(v => $"@Microsoft.KeyVault(SecretUri={v.SecretUriWithVersion})"));
            }

            foreach (var config in configs)
            {
                properties.Add($"{config.SettingsPath}{config.ConfigName}",
                    config.ConfigValue);
            }

            return properties;
        }
    }
}
