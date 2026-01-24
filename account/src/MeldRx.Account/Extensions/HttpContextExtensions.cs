#nullable enable
using System.IdentityModel.Tokens.Jwt;
using DarenaSolutions.Core.Extensions;
using MeldRx.Account;

namespace DarenaSolutions.Iam.Server.Extensions
{
    /// <summary>
    /// Class that contains extensions to the <see cref="HttpContext"/> class
    /// </summary>
    public static class HttpContextExtensions
    {
        /// <summary>
        /// Indicates if the logged in user is an organization moderator
        /// </summary>
        /// <param name="self">The source http context</param>
        /// <returns><c>true</c> if the logged in user is an organization moderator</returns>
        public static bool IsOrganizationModerator(this HttpContext self)
        {
            return self.User.IsOrganizationModerator();
        }

        /// <summary>
        /// Indicates if the logged in user is an organization moderator
        /// </summary>
        /// <param name="self">The source claims principal</param>
        /// <returns><c>true</c> if the logged in user is an organization moderator</returns>
        public static bool IsOrganizationModerator(this ClaimsPrincipal self)
        {
            return self.Claims.Any(x => x.Type == JwtClaimTypes.Role && x.Value == UserRoles.OrganizationMod);
        }

        /// <summary>
        /// Indicates if the logged in user is a user moderator
        /// </summary>
        /// <param name="self">The source http context</param>
        /// <returns><c>true</c> if the logged in user is a user moderator</returns>
        public static bool IsUserModerator(this HttpContext self)
        {
            return self.User.IsUserModerator();
        }

        /// <summary>
        /// Indicates if the logged in user is a user moderator
        /// </summary>
        /// <param name="self">The source claims principal</param>
        /// <returns><c>true</c> if the logged in user is a user moderator</returns>
        public static bool IsUserModerator(this ClaimsPrincipal self)
        {
            return self.Claims.Any(x => x.Type == JwtClaimTypes.Role && x.Value == UserRoles.UserMod);
        }

        /// <summary>
        /// Retrieves the base url of this application
        /// </summary>
        /// <param name="self">The source http context</param>
        /// <returns>The base url of this application</returns>
        public static string GetBaseUrl(this HttpContext self)
        {
            var request = self.Request;
            return $"{request.Scheme}://{request.Host}";
        }

        public static async Task RedirectToMeldRxOidcErrorAsync(
            this HttpContext httpContext,
            MeldRxOidcErrorCode errorCode
        )
        {
            if (httpContext.Response.HasStarted)
            {
                throw new InvalidOperationException("the response has already started.");
            }

            var duendeMessageStore = httpContext.RequestServices.GetRequiredService<IMessageStore<ErrorMessage>>();

            var error = new Message<ErrorMessage>(
                errorCode.AsErrorMessage(
                    httpContext.TraceIdentifier
                ),
                DateTime.Now
            );

            Log.ForContext("RequestId", httpContext.TraceIdentifier)
                .Error("{Error}, details: {ErrorDescription}", error.Data.Error, error.Data.ErrorDescription);

            var errorId = await duendeMessageStore.WriteAsync(error);
            httpContext.Response.Redirect($"/Home/Error?errorId={errorId}");
        }
        
        public static string? GetPatientIdIfContextExists(this HttpContext httpContext)
        {
            var bearerToken = httpContext.GetAuthorizationBearerToken();
            if (!string.IsNullOrWhiteSpace(bearerToken))
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(bearerToken);
                var patientClaim = jwtToken.Claims.FirstOrDefault(x =>
                    string.Equals("patient", x.Type, StringComparison.InvariantCultureIgnoreCase)
                );

                if (patientClaim is not null && !string.IsNullOrWhiteSpace(patientClaim.Value))
                {
                    return patientClaim.Value;
                }
            }

            return
                httpContext.Request.Headers.TryGetValue(
                    SharpOnMcpConstants.PatientIdHeaderName,
                    out var headerValue
                ) && !string.IsNullOrWhiteSpace(headerValue)
                    ? headerValue.ToString()
                    : null;
        }
    }
}
