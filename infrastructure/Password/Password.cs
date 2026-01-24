using Pulumi;

namespace MeldRx.Infrastructure.Random
{
    public class Password
    {
        public Pulumi.Random.RandomPassword password { get; set; }
        public Output<string> Result { get; set; }
        public Password(string name)
        {
            password = new Pulumi.Random.RandomPassword(name, new()
            {
                Length = 32,
                Special = true,
                OverrideSpecial = "_%@",
            });

            Result = password.Result;
        }
    }
}
