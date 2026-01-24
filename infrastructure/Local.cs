using Pulumi;
using System.Collections.Generic;
using System;
using System.Linq;
using System.IO;

class LocalStack : Stack
{
    public ProjectConfig p = new ProjectConfig();

    public LocalStack()
    {
        // Registered Providers
        var provider = new MeldRx.Infrastructure.Provider.AzureNative("provider", p.subscriptionId);

        // Azure Resource Group
        var rg = new MeldRx.Infrastructure.ResourceGroup("rg", p.projectName, provider.provider);

        // Storage Account
        var sa = new MeldRx.Infrastructure.Storage.StorageAccount("sa", p.projectName, false, true, false, rg);

        // For database exports
        var container = new MeldRx.Infrastructure.Storage.BlobContainer("container", "dbexports", false, sa, rg);
    }
}
