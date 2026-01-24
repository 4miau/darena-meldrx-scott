using DarenaSolutions.Bbp.Api.Constants;
using DarenaSolutions.MyMipsScore.Api.V2.Policies;
using MeldRx.Account.Auth.Policies;
using MeldRx.Account.ServicesRegistration.Policies;
using MeldRx.Services.Api.Auth.Policies;

namespace MeldRx.Account.ServicesRegistration
{
    public static class AuthorizationRegistration
    {
        public static void AddAuthorizationServices(this IServiceCollection services, IWebHostEnvironment environment)
        {
            services.AddSingleton<IAuthorizationServiceResolver, HttpContextAuthorizationServiceResolver>();

            services
                .AddTransient<IAuthorizationHandler, OneOfPoliciesHandler>()
                .AddScoped<IAuthorizationHandler, IsSuperAdminRequirementHandler>()
                .AddScoped<IAuthorizationHandler, IsMmsOrSuperAdminRequirementHandler>()
                .AddScoped<IAuthorizationHandler, SubjectOrClientScopeRequirementHandler>()
                .AddScoped<IAuthorizationHandler, AnonymousRequirementHandler>()
                .AddScoped<IAuthorizationHandler, AccessiblePersonIdRequirementHandler>()
                .AddScoped<IAuthorizationHandler, FhirRecordRequirementHandler>()
                .AddScoped<IAuthorizationHandler, AccessibleFhirRecordsEndpointRequirementHandler>()
                .AddScoped<IAuthorizationHandler, FhirEndpointsRequirementHandler>();

            services.AddTransient<IClaimsTransformation, ClaimsTransformation>();

            services
                .AddAuthorization(options =>
                {
                    options.AddPolicy(AuthConstants.SuperAdminPolicy, policy =>
                    {
                        policy.Requirements.Add(new IsSuperAdminRequirement());
                        policy.AddAuthenticationSchemes(
                            IdentityServerConstants.LocalApi.AuthenticationScheme,
                            IdentityConstants.ApplicationScheme
                        );
                    });

                    options.AddPolicy(AuthConstants.MmsOrSuperAdminPolicy, policy =>
                    {
                        policy.Requirements.Add(new IsMmsOrSuperAdminRequirement());
                        policy.AddAuthenticationSchemes(
                            IdentityServerConstants.LocalApi.AuthenticationScheme,
                            IdentityConstants.ApplicationScheme
                        );
                    });

                    options.AddPolicy(AuthConstants.SubjectRequirementPolicy, policy =>
                    {
                        policy.RequireClaim(JwtClaimTypes.Subject);
                        policy.AddAuthenticationSchemes(
                            IdentityServerConstants.LocalApi.AuthenticationScheme,
                            IdentityConstants.ApplicationScheme
                        );
                    });

                    options.AddPolicy(PolicyNames.AccessTokenOnly, policy => policy
                        .AddAuthenticationSchemes(IdentityServerConstants.LocalApi.AuthenticationScheme)
                        .RequireAuthenticatedUser()
                    );

                    options.AddPolicy(AuthConstants.IsAuthorizedPolicy, policy =>
                    {
                        policy.RequireAuthenticatedUser();
                        policy.AddAuthenticationSchemes(
                            IdentityServerConstants.LocalApi.AuthenticationScheme,
                            IdentityConstants.ApplicationScheme
                        );
                    });

                    options.AddPolicy(AuthConstants.SubjectOrAdminSearch, policy =>
                        policy.Requirements.Add(new OneOfPoliciesRequirement(AuthConstants.SubjectRequirementPolicy, AuthConstants.AdminSearchScopeRequirementPolicy)));


                    options.AddPolicy(AuthConstants.SuperAdminProdEnvironmentRequirementPolicy, policy =>
                        policy
                            .RequireRole(UserRoles.SuperAdmin)
                            .RequireAssertion(context =>
                                environment.IsEnvironment(EnvironmentNames.StagingMeldRx) ||
                                environment.IsEnvironment(EnvironmentNames.ProductionMeldRx))
                    );


                    options.AddPolicy(PolicyNames.CdsSandboxAllowedRequirement, policy =>
                    {
                        policy.Requirements.Add(new AnonymousRequirement());
                        policy.AddAuthenticationSchemes(ExternalProviders.CdsSandbox);
                    });


                    options.AddPolicy(PolicyNames.FhirRecordRequirementPolicy, policy =>
                    {
                        policy.RequireClaim(JwtClaimTypes.Subject);
                        policy.AddAuthenticationSchemes(
                            IdentityServerConstants.LocalApi.AuthenticationScheme,
                            IdentityConstants.ApplicationScheme
                        );
                        policy.Requirements.Add(new FhirRecordRequirement());
                    });


                    options.AddPolicy(PolicyNames.AccessiblePersonIdRequirement, policy =>
                    {
                        policy.Requirements.Add(new AccessiblePersonIdRequirement());
                        policy.AddAuthenticationSchemes(
                            IdentityServerConstants.LocalApi.AuthenticationScheme,
                            IdentityConstants.ApplicationScheme
                        );
                    });

                    options.AddPolicy(PolicyNames.FhirRecordRequirementPolicyUserContextOptional, policy =>
                    {
                        policy.AddAuthenticationSchemes(
                            IdentityServerConstants.LocalApi.AuthenticationScheme,
                            IdentityConstants.ApplicationScheme
                        );
                        policy.Requirements.Add(new FhirRecordRequirement());
                    });

                    options.AddPolicy(PolicyNames.FhirEndpointsPolicy, policy =>
                    {
                        policy.AddAuthenticationSchemes(
                            IdentityServerConstants.LocalApi.AuthenticationScheme,
                            IdentityConstants.ApplicationScheme
                        );
                        policy.Requirements.Add(new FhirEndpointsRequirement());
                    });

                    options.AddPolicy(PolicyNames.AccessibleFhirRecordsEndpointPolicy, policy =>
                    {
                        policy.AddAuthenticationSchemes(
                            IdentityServerConstants.LocalApi.AuthenticationScheme,
                            IdentityConstants.ApplicationScheme
                        );
                        policy.Requirements.Add(new AccessibleFhirRecordsEndpointRequirement());
                    });

                    //These Admin policies are required by MIPS API. They can be removed if/when MIPS api is merged with main app
                    options.AddPolicy(AuthConstants.AdminSearchScopeRequirementPolicy, policy =>
                    {
                        policy.AddAuthenticationSchemes(
                            IdentityServerConstants.LocalApi.AuthenticationScheme,
                            IdentityConstants.ApplicationScheme
                        );
                        policy.RequireClaim(JwtClaimTypes.Scope, AuthConstants.AdminSearchScope);
                    });

                    options.AddPolicy(AuthConstants.AdminWriteScopeRequirementPolicy, policy =>
                    {
                        policy.AddAuthenticationSchemes(
                            IdentityServerConstants.LocalApi.AuthenticationScheme,
                            IdentityConstants.ApplicationScheme
                        );
                        policy.RequireClaim(JwtClaimTypes.Scope, AuthConstants.AdminWriteScope);
                    });
                    
                    //MIPS controller Policies
                      options.AddPolicy(DarenaSolutions.MyMipsScore.Api.V2.Constants.AuthConstants.MipsReportIdContextPolicy, policy =>
                      {
                          policy.AddAuthenticationSchemes(
                              IdentityServerConstants.LocalApi.AuthenticationScheme,
                              IdentityConstants.ApplicationScheme
                          );
                          policy.Requirements.Add(new MipsReportRequirement());
                      });

                    options.AddPolicy(DarenaSolutions.MyMipsScore.Api.V2.Constants.AuthConstants.ProviderIdContextPolicy, policy =>
                    {
                        policy.AddAuthenticationSchemes(
                            IdentityServerConstants.LocalApi.AuthenticationScheme,
                            IdentityConstants.ApplicationScheme
                        );
                        policy.RequireClaim(JwtClaimTypes.Subject);
                        policy.Requirements.Add(new ProviderRequirement());
                    });

                    options.AddPolicy(DarenaSolutions.MyMipsScore.Api.V2.Constants.AuthConstants.FacilityIdContextPolicy, policy =>
                    {
                        policy.AddAuthenticationSchemes(
                            IdentityServerConstants.LocalApi.AuthenticationScheme,
                            IdentityConstants.ApplicationScheme
                        );
                        policy.RequireClaim(JwtClaimTypes.Subject);
                        policy.Requirements.Add(new FacilityRequirement());
                    });

                    options.AddPolicy(DarenaSolutions.MyMipsScore.Api.V2.Constants.AuthConstants.ProviderIdAndFacilityIdContextPolicy, policy =>
                    {
                        policy.AddAuthenticationSchemes(
                            IdentityServerConstants.LocalApi.AuthenticationScheme,
                            IdentityConstants.ApplicationScheme
                        );
                        policy.RequireClaim(JwtClaimTypes.Subject);
                        policy.Requirements.Add(new ProviderRequirement());
                        policy.Requirements.Add(new FacilityRequirement());
                    });

                    options.AddPolicy(DarenaSolutions.MyMipsScore.Api.V2.Constants.AuthConstants.MipsReportIdAndProviderIdAndFacilityIdContextPolicy, policy =>
                    {
                        policy.AddAuthenticationSchemes(
                            IdentityServerConstants.LocalApi.AuthenticationScheme,
                            IdentityConstants.ApplicationScheme
                        );
                        policy.RequireClaim(JwtClaimTypes.Subject);
                        policy.Requirements.Add(new MipsReportRequirement());
                        policy.Requirements.Add(new ProviderRequirement());
                        policy.Requirements.Add(new FacilityRequirement());
                    });

                    options.AddPolicy(DarenaSolutions.MyMipsScore.Api.V2.Constants.AuthConstants.SubjectRequirementPolicy, policy =>
                    {
                        policy.AddAuthenticationSchemes(
                            IdentityServerConstants.LocalApi.AuthenticationScheme,
                            IdentityConstants.ApplicationScheme
                        );
                        policy.RequireClaim(JwtClaimTypes.Subject);
                    });

                    options.AddPolicy(DarenaSolutions.MyMipsScore.Api.V2.Constants.AuthConstants.MmsAdminPolicy, policy =>
                    {
                        policy.AddAuthenticationSchemes(
                            IdentityServerConstants.LocalApi.AuthenticationScheme,
                            IdentityConstants.ApplicationScheme
                        );
                        policy.Requirements.Add(new IsMmsAdminRequirement());
                    });

                    options.AddPolicy(DarenaSolutions.MyMipsScore.Api.V2.Constants.AuthConstants.CanAddOrModifyProvidersPolicy, policy =>
                    {
                        policy.AddAuthenticationSchemes(
                            IdentityServerConstants.LocalApi.AuthenticationScheme,
                            IdentityConstants.ApplicationScheme
                        );
                        policy.RequireClaim(JwtClaimTypes.Subject);
                        policy.Requirements.Add(new MipsReportRequirement());
                        policy.Requirements.Add(new CanAddOrModifyProvidersRequirement());
                    });
                });
        }
    }
}
