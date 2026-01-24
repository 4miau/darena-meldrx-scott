using System.Threading.Tasks;
using System;
using Pulumi;

class Program
{
    public async static Task<int> Main()
    {
        var stack = Environment.GetEnvironmentVariable("PULUMI_STACK");

        if (stack == "local")
        {
            return await Deployment.RunAsync<LocalStack>();
        }
        if (stack == "cdn")
        {
            return await Deployment.RunAsync<CdnStack>();
        }
        else
        {
            return await Deployment.RunAsync<MeldRxStack>();
        }
    }
}
