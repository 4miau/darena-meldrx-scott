using System.Net;
using Meldh.Fhir.Core.Constants;
using Microsoft.SqlServer.Server;

namespace DarenaSolutions.Bbp.Api.Formatters
{
    /// <summary>
    /// The class that will override the response content type if a <see cref="Microsoft.SqlServer.Server.Format"/>
    /// query parameter exists in an incoming request.
    /// </summary>
    public class FhirFormatQueryParameterOverrider
    {
        private readonly HttpContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="FhirFormatQueryParameterOverrider"/> class
        /// </summary>
        /// <param name="context">The http context</param>
        public FhirFormatQueryParameterOverrider(HttpContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Determines if the <see cref="Microsoft.SqlServer.Server.Format"/> query parameter exists in the current
        /// request. If it does, it will override the response content to what is specified in the <see cref="Microsoft.SqlServer.Server.Format"/>
        /// query parameter. If it does not, nothing happens in this method
        /// </summary>
        /// <param name="objToSerialize">The object to serialize to the format specified in the <see cref="Microsoft.SqlServer.Server.Format"/>
        /// query parameter</param>
        /// <returns><c>true</c> if the <see cref="Microsoft.SqlServer.Server.Format"/> query parameter was found and the response content
        /// was successfully serialized to the specified content. <c>false</c> otherwise</returns>
        public async Task<bool> FormatQueryParameterOverrideAsync(object objToSerialize)
        {
            if (!_context.Request.Query.ContainsKey(GlobalQueryParameterNames.Format))
            {
                return false;
            }

            var value = _context.Request.Query[GlobalQueryParameterNames.Format].FirstOrDefault();

            var serializer = new FhirOutputFormatSerializer(_context);
            switch (value)
            {
                case "xml":
                case "text/xml":
                case "application/xml":
                case "application/fhir+xml":
                    await serializer.SerializeToResponseAsync(false, objToSerialize);
                    return true;
                case "json":
                case "text/json":
                case "application/json":
                case "application/fhir+json":
                    await serializer.SerializeToResponseAsync(true, objToSerialize);
                    return true;
                default:
                    _context.Response.StatusCode = (int)HttpStatusCode.NotAcceptable;
                    return true;
            }
        }
    }
}
