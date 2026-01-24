using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace MeldRx.Account.Extensions;

/// <summary>
/// An extensions class that contains extensions to the <see cref="OpenIdConnectOptions"/> class
/// </summary>
public static class OpenIdConnectOptionsExtensions
{
    /// <summary>
    /// Clears the default inbound claim type mapping by setting a new JWT security token handler. The default mapping
    /// does not set claims to the expected claim names such as those in <see cref="JwtClaimTypes"/>. This extension
    /// method sets it to the expected names
    /// </summary>
    /// <param name="self">The source open id connect options</param>
    public static void ClearInboundClaimTypeMapping(this OpenIdConnectOptions self)
    {
        if (self == null)
        {
            throw new ArgumentNullException(nameof(self));
        }

        var handler = new JwtSecurityTokenHandler { InboundClaimTypeMap = new Dictionary<string, string>() };
        self.SecurityTokenValidator = handler;
    }
}