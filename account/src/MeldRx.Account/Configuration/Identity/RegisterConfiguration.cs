


namespace DarenaSolutions.Iam.Server.Configuration.Identity
{
    public class RegisterConfiguration
    {
        //public RegisterConfiguration(IamProfile profile)
        //{
        //    Enabled = profile == IamProfile.B2b;
        //}
        //public bool Enabled { get; set; } 
        public LoginResolutionPolicy ResolutionPolicy { get; set; } = LoginResolutionPolicy.Email;
    }
}
