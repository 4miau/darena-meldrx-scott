using MeldRx.Account.Auth.AuthProviders;
using MeldRx.Account.SmartOnFhirServices;
using MeldRx.Sdk.Internal.Cms.Oidc;
using MeldRx.Services.Shared;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Graph;

namespace MeldRx.Account.Auth
{
    /// <summary>
    ///
    /// </summary>
    public static class AuthenticationRegistration
    {
        public static void AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ISofAuthSessionService, SofAuthSessionService>();

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.None;
                options.Secure = CookieSecurePolicy.Always;
            });


            services
                .AddIdentityServices(configuration)
                .AddDuendeServices(configuration);
            //******IMPORTANT - this line should always come AFTER AddIdentityServices***********
            //********AddIdentityServices calls AddIdentity which overrides the cookie events below if it comes after. ********
            services.ConfigureApplicationCookie(c =>
            {
                var cookieLifeTime = TimeSpan.FromDays(10);
                c.Cookie.Domain = configuration.MeldRxSettings().CookieDomain;
                c.Cookie.SameSite = SameSiteMode.Lax;
                c.Cookie.MaxAge = cookieLifeTime;
                c.ExpireTimeSpan = cookieLifeTime;
                c.SlidingExpiration = true;

                var baseOnRedirectToLogin = c.Events.OnRedirectToLogin;
                c.Events.OnRedirectToLogin = context =>
                {
                    var redirect = context.HttpContext.GetEndpoint()?.Metadata.GetMetadata<RedirectUnauthorizedAttribute>() != null;
                    if (redirect)
                    {
                        return baseOnRedirectToLogin(context);
                    }
                    
                    if (context.Request.Path.Value !=null && (context.Request.Path.Value.Contains("/api") || context.Request.Path.Value.Contains("/mms-api"))  )
                    {
                        context.Response.StatusCode = 401;
                        return Task.CompletedTask;
                    }

                    return baseOnRedirectToLogin(context);
                };

                var baseOnRedirectToAccessDenied = c.Events.OnRedirectToLogin;
                c.Events.OnRedirectToAccessDenied = context =>
                {
                    if (context.Request.Path.Value !=null && (context.Request.Path.Value.Contains("/api") || context.Request.Path.Value.Contains("/mms-api")))
                    {
                        context.Response.StatusCode = 403;
                        return Task.CompletedTask;
                    }

                    return baseOnRedirectToAccessDenied(context);
                };


                var baseOnSigningOut = c.Events.OnSigningOut;
                c.Events.OnSigningOut = async context =>
                {
                    await PersistCookieHash(context, cookieLifeTime);

                    await baseOnSigningOut(context);
                    return;

                    static async Task PersistCookieHash(CookieSigningOutContext ctx, TimeSpan timeSpan)
                    {
                        var cookieName = ctx.Options.Cookie.Name!;
                        var cache = ctx.HttpContext.RequestServices.GetRequiredService<IDistributedCacheManager>();

                        var cookie = ctx.Request.Cookies[cookieName]!;
                        var hash = cookie.Md5();

                        await cache.SetStringAsync(hash, "", absoluteExpiration: timeSpan);
                    }
                };

                var baseOnValidatePrincipal = c.Events.OnValidatePrincipal;
                c.Events.OnValidatePrincipal = async context =>
                {
                    if (await InvalidatedCookieAsync(context, cookieLifeTime))
                    {
                        context.Principal = null; // setting principal to null fails Authentication
                        return;
                    }

                    await baseOnValidatePrincipal(context);
                    return;

                    static async ValueTask<bool> InvalidatedCookieAsync(CookieValidatePrincipalContext ctx, TimeSpan cookieLifetime)
                    {
                        var cookieName = ctx.Options.Cookie.Name!;
                        var memoryCache = ctx.HttpContext.RequestServices.GetRequiredService<IMemoryCache>();
                        var cache = ctx.HttpContext.RequestServices.GetRequiredService<IDistributedCacheManager>();

                        var cookie = ctx.Request.Cookies[cookieName]!;
                        var hash = cookie.Md5();
                        if (memoryCache.TryGetValue(hash, out bool invalidated))
                        {
                            return invalidated;
                        }

                        invalidated = await cache.FindStringAsync(hash) != null;

                        // mark cookie as valid for 30 seconds in memory before checking with cache again.
                        memoryCache.Set(
                            hash,
                            invalidated,
                            absoluteExpiration: invalidated
                                ? DateTimeOffset.Now.Add(cookieLifetime)
                                : DateTimeOffset.Now.AddSeconds(30)
                        );

                        return invalidated;
                    }
                };
            });

            services.AddAuthentication()
                .AddLocalApi()
                //.AddSocialProviders(configuration) Disabled for now. Target January-2024 release
                .AddExternalSof(configuration)
                .AddExternalAppSof(configuration)
                .AddCmsAuthentication(configuration)
                ;

            services.AddScoped<ISmartOnFhirPatientSearchService, SmartOnFhirPatientSearchService>();

            //Azure Ad
            services
                .AddSingleton<IAuthenticationProvider, AadGraphAuthenticationProvider>()
                .AddSingleton<IAadGraphHttpClientProvider, AadGraphHttpClientProvider>()
                .AddSingleton<GraphServiceClient>(provider =>
                    new GraphServiceClient(provider.GetService<IAadGraphHttpClientProvider>().Get()))
                .AddSingleton<IAadGraphApiClient, AadGraphApiClient>();
        }

        // TODO: https://dev.azure.com/darenasolutions/MeldRx/_workitems/edit/253
        private static void AddCdsHooks(this AuthenticationBuilder authenticationBuilder, IConfiguration configuration)
        {
            // services.AddJwtBearer(ExternalProviders.CdsSandbox, options =>
            // {
            //     options.TokenValidationParameters = new TokenValidationParameters
            //     {
            //         ValidateIssuer = false,
            //         ValidateAudience = false,
            //         ValidateLifetime = false,
            //         ValidateIssuerSigningKey = false,
            //         //ValidIssuer = "https://sandbox.cds-hooks.org",
            //         SignatureValidator = delegate(string token, TokenValidationParameters parameters)
            //         {
            //             var jwt = new JwtSecurityToken(token);
            //             return jwt;
            //         },
            //     };
            //     options.Events = new JwtBearerEvents()
            //     {
            //         OnTokenValidated = ctx =>
            //         {
            //
            //             return Task.CompletedTask;
            //         },
            //         OnMessageReceived = context =>
            //         {
            //             return Task.CompletedTask;
            //         },
            //         OnForbidden = context =>
            //         {
            //             return Task.CompletedTask;
            //         },
            //         OnAuthenticationFailed = context =>
            //         {
            //             return Task.CompletedTask;
            //         }
            //     };
            // })
        }

        private static void AddCmsAuthentication(this AuthenticationBuilder authenticationBuilder, IConfiguration configuration)
        {
            authenticationBuilder
                .AddCookie(ExternalProviders.ExternalAppCookie, options => { options.Cookie.Name = ExternalProviders.ExternalAppCookie; })
                .AddRemoteScheme<CmsAuthenticationOptions, CmsAuthenticationHandler>(
                    ExternalProviders.Cms,
                    "CMS Blue Button 2.0",
                    options =>
                    {
                        options.SignInScheme = ExternalProviders.ExternalAppCookie;
                        options.Authority = configuration.MeldRxSettings().CmsAuthorityUrl;
                        options.ClientId = configuration.MeldRxSettings().CmsClientId;
                        options.ClientSecret = configuration.MeldRxSettings().CmsClientSecret;
                        options.CallbackPath = "/oidc-cms-callback";
                    }
                );
        }
    }

    public class ExternalAuthentication
    {
        public ExternalAuthenticationProviderDetails Google { get; set; }
        public ExternalAuthenticationProviderDetails Twitter { get; set; }
        public ExternalAuthenticationProviderDetails Meta { get; set; }
        public ExternalAuthenticationProviderDetails Github { get; set; }
    }

    public class ExternalAuthenticationProviderDetails
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string DisplayName { get; set; }
        public string Icon { get; set; }
        public bool Enabled { get; set; }
    }
}
