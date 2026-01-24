// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

// Original file: https://github.com/DuendeSoftware/IdentityServer.Quickstart.UI
// Modified by Jan Škoruba

using Microsoft.AspNetCore.Mvc.Filters;

namespace MeldRx.Account.Helpers
{
    public class SecurityHeadersAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            var result = context.Result;
            if (result is ViewResult)
            {
                // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Content-Type-Options
                if (!context.HttpContext.Response.Headers.ContainsKey("X-Content-Type-Options"))
                {
                    context.HttpContext.Response.Headers["X-Content-Type-Options"] = "nosniff";
                }

                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "local" &&
                    Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "localb2c")
                {
                    // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Frame-Options
                    if (!context.HttpContext.Response.Headers.ContainsKey("X-Frame-Options"))
                    {
                        context.HttpContext.Response.Headers["X-Frame-Options"] = "SAMEORIGIN";
                    }
                }

                // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Referrer-Policy
                var referrer_policy = "no-referrer";
                if (!context.HttpContext.Response.Headers.ContainsKey("Referrer-Policy"))
                {
                    context.HttpContext.Response.Headers["Referrer-Policy"] = referrer_policy;
                }
            }
        }
    }
}








