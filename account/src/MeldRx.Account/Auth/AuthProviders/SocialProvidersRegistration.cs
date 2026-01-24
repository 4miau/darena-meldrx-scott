namespace MeldRx.Account.Auth.AuthProviders
{
    public static class SocialProvidersRegistration
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="authenticationBuilder"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static AuthenticationBuilder AddSocialProviders(this AuthenticationBuilder authenticationBuilder,
            IConfiguration configuration)
        {
            var externalProvidersList =
                configuration.GetSettingsForSection<ExternalAuthentication>(nameof(ExternalAuthentication));

            authenticationBuilder.AddAzureAd(configuration);


            if (externalProvidersList != null)
            {
                if (externalProvidersList.Google != null && externalProvidersList.Google.Enabled)
                {
                    authenticationBuilder.AddGoogle(googleOptions =>
                    {
                        googleOptions.ClientId = externalProvidersList.Google.ClientId;
                        googleOptions.ClientSecret = externalProvidersList.Google.ClientSecret;
                    });
                }

                //if (externalProvidersList.Meta != null && externalProvidersList.Meta.Enabled)
                //{
                //    authenticationBuilder.AddGoogle(googleOptions =>
                //    {
                //        googleOptions.ClientId = externalProvidersList.Meta.ClientId;
                //        googleOptions.ClientSecret = externalProvidersList.Meta.ClientSecret;
                //    });
                //}

                if (externalProvidersList.Twitter != null && externalProvidersList.Twitter.Enabled)
                {
                    authenticationBuilder.AddTwitter(options =>
                    {
                        options.ConsumerKey = externalProvidersList.Twitter.ClientId;
                        options.ConsumerSecret = externalProvidersList.Twitter.ClientSecret;
                    });
                }

                //authenticationBuilder.AddOAuth("Twitter", options =>
                //{
                //    options.Scope.Clear();
                //    options.Scope.Add("users.read");
                //    options.AuthorizationEndpoint = "https://twitter.com/i/oauth2/authorize";
                //    options.TokenEndpoint = "https://api.twitter.com/2/oauth2/token";
                //    options.UsePkce = true;
                //    options.ClientId= configuration["ExternalAuthentication:Twitter:ClientId"];
                //    options.ClientSecret = configuration["ExternalAuthentication:Twitter:ClientSecret"];
                //    options.CallbackPath = "/signin-twitter";
                //});

                //authenticationBuilder.AddTwitter(options =>
                //{

                //    options.ConsumerKey = configuration["ExternalAuthentication:Twitter:ClientId"];
                //    options.ConsumerSecret = configuration["ExternalAuthentication:Twitter:ClientSecret"];
                //});

                //authenticationBuilder.AddOpenIdConnect("Twitter", "Twitter",
                //    options =>
                //    {
                //        options.SignInScheme = IdentityConstants.ExternalScheme;
                //        options.SignOutScheme = IdentityConstants.ExternalScheme;
                //        options.SaveTokens = true;

                //        options.Authority = "https://api.twitter.com/oauth/authorize";
                //        options.ClientId = configuration["Authentication:Twitter:ClientId"];
                //        options.ClientSecret = configuration["Authentication:Twitter:ClientSecret"];
                //        options.ResponseType = "code";

                //        //options.TokenValidationParameters = new TokenValidationParameters
                //        //{
                //        //    NameClaimType = "name",
                //        //    RoleClaimType = "role"
                //        //};


                //    });
            }
            return authenticationBuilder;
        }
    }
}
