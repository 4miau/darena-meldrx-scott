using Pulumi;
using System.Collections.Generic;
using System;

namespace MeldRx.Infrastructure.AppService
{
  public class Config
  {
    public string SettingsPath { get; set; }
    public string ConfigName { get; set; }
    public Output<string> ConfigValue { get; set; }
    public MeldRx.Infrastructure.KeyVault.Secret SecretValue { get; set; }

    public Config(string settingsPath, string configName, Output<string> configValue)
    {
      SettingsPath = settingsPath;
      ConfigName = configName;
      ConfigValue = configValue;
    }

    public Config(string settingsPath, string configName, MeldRx.Infrastructure.KeyVault.Secret secretValue)
    {
      SettingsPath = settingsPath;
      ConfigName = configName;
      SecretValue = secretValue;
    }
  }
}
