using Microsoft.IdentityModel.Tokens;

namespace MeldRx.Account.Auth.AuthProviders;

public static class DuendeTestOAuthRegistration
{
    public static AuthenticationBuilder AddDuendeTestOAuth(this AuthenticationBuilder authenticationBuilder,
        IConfiguration configuration)
    {
        authenticationBuilder.AddOpenIdConnect(ExternalProviders.Duende, "Demo IdentityServer",
            options =>
            {
                options.SignInScheme = IdentityConstants.ExternalScheme;
                options.SignOutScheme = IdentityConstants.ExternalScheme;
                options.SaveTokens = true;

                options.Authority = "https://demo.duendesoftware.com";
                options.ClientId = "interactive.confidential";
                options.ClientSecret = "secret";
                options.ResponseType = "code";

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
                    RoleClaimType = "role"
                };


            });
        return authenticationBuilder;
    }
}