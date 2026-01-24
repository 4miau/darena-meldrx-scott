using Pulumi;

namespace MeldRx.Infrastructure.Cache
{
    public class Redis
    {
        public Pulumi.AzureNative.Cache.Redis redis { get; set; }
        public Output<string> Name { get; set; }
        public Output<string> PrimaryKey { get; set; }
        private ProjectConfig p = new ProjectConfig();

        public Redis(string pulumiName, string resourceName, MeldRx.Infrastructure.ResourceGroup rg)
        {
            var redisConfig = getRedisConfig(rg);

            redis = new Pulumi.AzureNative.Cache.Redis(pulumiName, new()
            {
                Name = $"redis-{resourceName}-{p.env}",
                ResourceGroupName = rg.Name,
                EnableNonSslPort = false,
                MinimumTlsVersion = "1.2",
                RedisVersion = "6.0",
                RedisConfiguration = redisConfig,
                Sku = new Pulumi.AzureNative.Cache.Inputs.SkuArgs
                {
                    Name = p.env_Redis_Name,
                    Family = p.env_Redis_Family,
                    Capacity = 1,
                },
                Tags = { { "env", p.env } },
            });

            Name = redis.Name;
            PrimaryKey = getRedisKeys(rg);
        }

        private Output<string> getRedisKeys(MeldRx.Infrastructure.ResourceGroup rg)
        {
            //Redis access keys
            var redisKeys = Pulumi.AzureNative.Cache.ListRedisKeys.Invoke(new Pulumi.AzureNative.Cache.ListRedisKeysInvokeArgs
            {
                ResourceGroupName = rg.Name,
                Name = redis.Name
            });

            return redisKeys.Apply(v =>
            {
                var firstKey = v.PrimaryKey;
                return Output.CreateSecret(firstKey);
            });
        }

        private Pulumi.AzureNative.Cache.Inputs.RedisCommonPropertiesRedisConfigurationArgs getRedisConfig(MeldRx.Infrastructure.ResourceGroup rg)
        {
            if (p.env_Redis_Persistence_Enabled == "true")
            {
                // Redis Premium Storage Account (non-HNS)
                var sa_redis = new MeldRx.Infrastructure.Storage.StorageAccount("saredis", p.projectName, false, false, false, rg);

                return new Pulumi.AzureNative.Cache.Inputs.RedisCommonPropertiesRedisConfigurationArgs
                {
                    MaxmemoryPolicy = "allkeys-lru",
                    AofStorageConnectionString0 = sa_redis.PrimaryConnectionString,
                    // AofStorageConnectionString1 = sa.SecondaryConnectionString,
                    // RdbBackupEnabled = "false",
                    // RdbBackupFrequency = "15",
                    // RdbBackupMaxSnapshotCount = "1",
                    // RdbStorageConnectionString = sa.PrimaryConnectionString,
                };
            }
            else
            {
                return new Pulumi.AzureNative.Cache.Inputs.RedisCommonPropertiesRedisConfigurationArgs
                {
                    MaxmemoryPolicy = "allkeys-lru",
                };
            }
        }
    }
}
