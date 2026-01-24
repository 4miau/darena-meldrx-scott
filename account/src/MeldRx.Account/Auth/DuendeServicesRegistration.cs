using Duende.IdentityServer.Configuration.EntityFramework;
using Duende.IdentityServer.Configuration.Validation.DynamicClientRegistration;
using Duende.IdentityServer.Validation;
using MeldRx.Account.Services.OAuthCompliantDynamicRegistration;
using MeldRx.Account.SmartOnFhirServices;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MeldRx.Account.Auth;

public static class DuendeServicesRegistration
{
    private const string DuendeIdentityServerLicenseKey =
        "eyJhbGciOiJQUzI1NiIsImtpZCI6IklkZW50aXR5U2VydmVyTGljZW5zZWtleS83Y2VhZGJiNzgxMzA0NjllODgwNjg5MTAyNTQxNGYxNiIsInR5cCI6ImxpY2Vuc2Urand0In0.eyJpc3MiOiJodHRwczovL2R1ZW5kZXNvZnR3YXJlLmNvbSIsImF1ZCI6IklkZW50aXR5U2VydmVyIiwiaWF0IjoxNzMzMDk3NjAwLCJleHAiOjE3OTg0MTYwMDAsImNvbXBhbnlfbmFtZSI6IkRhcmVuYSBTb2x1dGlvbnMiLCJjb250YWN0X2luZm8iOiJwamluZGFsQGRhcmVuYXNvbHV0aW9ucy5jb207IHN1cHBvcnRAZGFyZW5hc29sdXRpb25zLmNvbSIsImVkaXRpb24iOiJFbnRlcnByaXNlIiwiaWQiOiI2NzQ3In0.DeesG-YBUPbeoYjXiuqWOAoI1BRTnQ4jYQnLbSMp_CEwwtoNF-vlMygdCH-OMPl0jDpWfsRCztXQVz1fuR46hdNJ-F2dcortACkd5c6e8sNr_7aDePU7zc5gLQ9tUo9mTHeSBzt29moCFy1TfR2EudQ_yEH8aVll435iWSbCP58rCg6OqYW09f0HPIopGg5TG8AIqWcdNbvnHZR6H7nXUmGicFriJOKVrCV14_9rwJFO6fWVBH-J0XapHGSzcut4JW-WFxnCGJyLx_B13uSgNNTmTSCsCRfeV6sfWCTZQBDbN7omwjJLUhbCh8v4miw1RHwPvYRE8tvZU9u4S7ZnOQ";
    
    public static IServiceCollection AddDuendeServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IConfigurationDbContext>(svcs => svcs.GetRequiredService<ApplicationDbContext>());
        services
            .AddIdentityServer(options =>
            {
                options.IssuerUri = configuration.MeldRxSettings().AuthorityUrl;
                options.InputLengthRestrictions.Scope = 4000;
                options.Discovery.CustomEntries.Add(OidcConstants.Discovery.RegistrationEndpoint, "~/connect/dcr");
                options.KeyManagement.Enabled = true;
                options.KeyManagement.SigningAlgorithms = new List<SigningAlgorithmOptions>()
                {
                    new SigningAlgorithmOptions("RS256"),
                    new SigningAlgorithmOptions("ES384")
                };

                options.Events.RaiseFailureEvents = true;
                options.LicenseKey = DuendeIdentityServerLicenseKey;
            })
            .AddAspNetIdentity<ApplicationUser>()
            .AddConfigurationStore<ApplicationDbContext>()
            .AddOperationalStore<ApplicationDbContext>()
            .AddExtensionGrantValidator<DelegationGrantValidator>()
            .AddAuthorizeInteractionResponseGenerator<SmartOnFhirCompatibleAuthorizeInteractionResponseGenerator>()
            .AddJwtBearerClientAuthentication(); //Client Assertion Parameter validation - https://docs.duendesoftware.com/identityserver/v5/tokens/authentication/overview/

        // This is the second `IAuthorizeRequestValidator` service. First one being registered previously from the `AddIdentityServer`
        // call. Read DsAuthorizeRequestValidator.cs file for more information as to why we need this.
        services.AddTransient<IAuthorizeRequestValidator, DsAuthorizeRequestValidator>();
        
        services
            .AddTransient<IDynamicClientRegistrationValidator, DsDynamicRegistrationValidator>()
            .AddIdentityServerConfiguration(options => options.LicenseKey = DuendeIdentityServerLicenseKey)
            .AddClientConfigurationStore()
            .AddTransient<IClientConfigurationStore, DsDynamicRegistrationStore>();

        services.AddCustomDuendeServices(configuration);
        
        return services;
    }

    public static IServiceCollection AddCustomDuendeServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Do a replace here because when calling .AddIdentityServer(), the client secret validation is added by just
        // adding the service, .AddTransient<IClientSecretValidator, instead of .TryAddTransient<IClientSecretValidator
        services.Replace(ServiceDescriptor.Transient<IClientSecretValidator, SmartOnFhirCompatibleClientSecretValidator>());

        // Add our custom response generators that also support SMART-on-FHIR
        // specifications including default OAuth specifications
        return services
            .AddTransient<ITokenResponseGenerator, SmartOnFhirCompatibleTokenResponseGenerator>()
            .AddTransient<IAuthorizeResponseGenerator, SmartOnFhirCompatibleAuthorizationCodeResponseGenerator>()
            .AddTransient<ITokenService, SmartOnFhirCompatibleTokenService>()
            .AddTransient<IDiscoveryResponseGenerator, MeldRxDiscoveryResponseGenerator>()
            .AddTransient<IProfileService, DsIamProfile>()
            .AddScoped<OverridableIssuerNameService>()
            .AddTransient<IIssuerNameService>(sp => sp.GetRequiredService<OverridableIssuerNameService>());
    }
}
