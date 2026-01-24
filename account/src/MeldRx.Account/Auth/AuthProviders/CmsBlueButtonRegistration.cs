using MeldRx.Sdk.Internal.Cms;

namespace MeldRx.Account.Auth.AuthProviders;

public static class CmsBlueButtonRegistration
{
    public static AuthenticationBuilder AddCmsBlueButton(this AuthenticationBuilder authenticationBuilder,
        IConfiguration configuration)
    {
        authenticationBuilder.AddCmsAuthentication(options =>
        {
            var meldrxSettings = configuration.GetSettingsForSection<MeldRxSettings>(nameof(MeldRxSettings));
            options.SignInScheme = IdentityConstants.ExternalScheme;
            options.SignOutScheme = IdentityConstants.ExternalScheme;
            options.Authority = meldrxSettings.CmsAuthorityUrl;
            options.ClientId = meldrxSettings.CmsClientId;
            options.ClientSecret = meldrxSettings.CmsClientSecret;

            //configuration.GetSection(nameof(CmsAuthSettings)).Bind(options);
            options.CallbackPath = "/oidc-cms-callback";
            options.SaveTokens = true;
        });
        return authenticationBuilder;
    }
}