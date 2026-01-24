using System.IdentityModel.Tokens.Jwt;
using System.Web;
using MeldRx.Account.Extensions;
using Microsoft.IdentityModel.Tokens;

namespace MeldRx.Account.Auth.AuthProviders
{
    public static class AzureActiveDirectoryRegistration
    {
        public static AuthenticationBuilder AddAzureAd(this AuthenticationBuilder authenticationBuilder, IConfiguration configuration)
        {
            var apiSettings = configuration.MeldRxSettings();
            authenticationBuilder.AddOpenIdConnect(ExternalProviders.AzureActiveDirectory, "Azure Active Directory",
                options =>
                {
                    //options.SignInScheme = IdentityConstants.ExternalScheme;
                    // options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    // options.SignOutScheme = IdentityConstants.ExternalScheme;

                    options.CallbackPath = "/oidc-aad-callback";
                    options.ClientId = apiSettings.AadAudience;
                    // options.ClientSecret = apiSettings.AadClientSecret;
                    options.Authority = apiSettings.AadAuthority;
                    options.ResponseType = OidcConstants.ResponseTypes.Code;
                    options.ResponseMode = OidcConstants.ResponseModes.FormPost;
                    options.Scope.Add(IdentityServerConstants.StandardScopes.OpenId);
                    options.Scope.Add(IdentityServerConstants.StandardScopes.Profile);
                    options.Scope.Add(AuthConstants.AadGraphUserReadScope);
                    options.Scope.Add(AuthConstants.AadGraphDirectoryReadScope);
                    options.SaveTokens = true;
                    options.ClearInboundClaimTypeMapping();
                    options.Events.OnRemoteFailure = RemoteFailureHandler;

                    // options.Events.OnTicketReceived =async ctx =>
                    // {
                    //     var tenantId =
                    //         ctx.Principal.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid");
                    //     var dbService = ctx.HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>();
                    //     var doesExist = new Organization();// await dbService.Organizations.FirstOrDefaultAsync(o => o.AadTenantId == tenantId);
                    //     if (doesExist !=null)
                    //     {
                    //          await ctx.HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, ctx.Principal, ctx.Properties);
                    //          ctx.HandleResponse();
                    //          // Default redirect path is the base path
                    //          if (string.IsNullOrEmpty(ctx.Properties.RedirectUri))
                    //          {
                    //              ctx.Properties.RedirectUri = "/";
                    //          }
                    //
                    //          ctx.Response.Redirect(ctx.Properties.RedirectUri);
                    //     }
                    //
                    // };

                    // ONLY FOR LOCALHOST
                    if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "local" ||
                        Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "localb2c")
                    {
                        options.TokenValidationParameters.ValidateIssuer = false;
                        options.TokenValidationParameters.ValidateIssuerSigningKey = false;
                        options.RequireHttpsMetadata = false;
                    }

                    options.TokenValidationParameters.IssuerValidator = (issuer, token, parameters) =>
                    {
                        if (token is JwtSecurityToken jwt)
                        {
                            if (jwt.Payload.TryGetValue(AuthConstants.TenantIdClaimType, out var tid))
                            {
                                var tenantId = (string)tid;
                                var validIssuer = apiSettings.AadAuthority.Replace("common", tenantId);
                                if (validIssuer == issuer)
                                {
                                    return issuer;
                                }
                            }
                            else
                            {
                                throw new SecurityTokenInvalidIssuerException(
                                    $"IDX10205: Issuer validation failed. Could not read tenant id");
                            }
                        }

                        throw new SecurityTokenInvalidIssuerException(
                            $"IDX10205: Issuer validation failed. Issuer: '{issuer}' is not a valid issuer");
                    };

                    //if (Environment.IsDevelopment())
                    //{
                    //    options.RequireHttpsMetadata = false;
                    //}
                });

            return authenticationBuilder;
        }

        private static Task RemoteFailureHandler(RemoteFailureContext context)
        {
            var uri = new Uri(context.Properties.RedirectUri);
            string returnUrl = null;

            if (!string.IsNullOrWhiteSpace(uri.Query))
            {
                var queryString = HttpUtility.ParseQueryString(uri.Query);
                returnUrl = queryString["returnUrl"];
            }

            var parameters = new List<string>();
            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                parameters.Add($"returnUrl={returnUrl}");
            }

            string errorDescription;
            if (!string.IsNullOrWhiteSpace(context.Failure.Message))
            {
                errorDescription = context.Failure.Message;
            }
            else if (context.Failure.Data.Contains("error_description"))
            {
                var str = context.Failure.Data["error_description"].ToString();
                var startIndex = str.IndexOf(":", StringComparison.InvariantCulture);
                startIndex = startIndex < 0 ? 0 : startIndex + 2;

                var endIndex = str.IndexOf("Trace ID:", StringComparison.InvariantCulture);
                endIndex = endIndex - 2 < startIndex ? str.Length : endIndex - 2;

                errorDescription = str.Substring(startIndex, endIndex - startIndex);
            }
            else
            {
                errorDescription = "An unexpected error has occurred while logging in through the external identity provider";
            }

            parameters.Add($"popupNotificationMessage={HttpUtility.UrlEncode(errorDescription)}");
            var redirectStr = $"/Identity/Account/Login?{string.Join("&", parameters)}";

            context.Response.Redirect(redirectStr);
            context.HandleResponse();

            return Task.CompletedTask;
        }
    }
}
