using Pulumi;

namespace MeldRx.Infrastructure.ServiceBus
{
    public class Queue
    {
        public Pulumi.AzureNative.ServiceBus.Queue queue { get; set; }
        private ProjectConfig p = new ProjectConfig();
        public Queue(string pulumiName, string resourceName, MeldRx.Infrastructure.ServiceBus.Namespace sb, MeldRx.Infrastructure.ResourceGroup rg)
        {
            queue = new Pulumi.AzureNative.ServiceBus.Queue(pulumiName, new()
            {
                ResourceGroupName = rg.Name,
                NamespaceName = sb.Name,
                QueueName = resourceName,
                EnablePartitioning = false,
                RequiresSession = true,
                MaxDeliveryCount = 32,
                MaxSizeInMegabytes = 1024,
                MaxMessageSizeInKilobytes = 256
            });
        }
    }
}
