using MeldRx.Account.SmartOnFhirServices;

namespace MeldRx.Account.Auth.AuthProviders;

public static class GenericSmartOnFhirProviderRegistration
{
    public static AuthenticationBuilder AddExternalSof(
        this AuthenticationBuilder authenticationBuilder,
        IConfiguration configuration
    )
    {
        authenticationBuilder.AddRemoteScheme<SofAuthenticationOptions, SoFAuthenticationHandler>(ExternalProviders.SoF, "SMART on FHIR",
            options =>
            {
                options.SignInScheme = IdentityConstants.ExternalScheme;
                options.SignOutScheme = IdentityConstants.ExternalScheme;
                options.CallbackPath = "/sof-callback";
                options.SaveTokens = true;
            });

        return authenticationBuilder;
    }
    public static AuthenticationBuilder AddExternalAppSof(
        this AuthenticationBuilder authenticationBuilder,
        IConfiguration configuration
    )
    {
        authenticationBuilder.AddRemoteScheme<SofAuthenticationOptions, ExternalAppSoFAuthenticationHandler>(ExternalProviders.ExternalAppSof, "SMART on FHIR",
            options =>
            {
                options.SignInScheme = ExternalProviders.ExternalAppCookie;
                options.SignOutScheme = ExternalProviders.ExternalAppCookie;
                options.CallbackPath = "/sof-meldrx-callback";
                options.SaveTokens = true;
            });

        return authenticationBuilder;
    }
}
