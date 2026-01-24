using System.Text;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace DarenaSolutions.Bbp.Api.Formatters
{
    /// <summary>
    /// The output formatter for a JSON representation of a resource
    /// </summary>
    public class FhirJsonOutputFormatter : TextOutputFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FhirJsonOutputFormatter"/> class
        /// </summary>
        public FhirJsonOutputFormatter()
        {
            SupportedMediaTypes.Add("json;fhirVersion=4.0;charset=utf-8");
            SupportedMediaTypes.Add("text/json;fhirVersion=4.0;charset=utf-8");
            SupportedMediaTypes.Add("application/json;fhirVersion=4.0;charset=utf-8");
            SupportedMediaTypes.Add("application/fhir+json;fhirVersion=4.0;charset=utf-8");

            SupportedEncodings.Add(Encoding.UTF8);
        }

        /// <inheritdoc />
        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            var overrider = new FhirFormatQueryParameterOverrider(context.HttpContext);
            var overridden = await overrider.FormatQueryParameterOverrideAsync(context.Object);

            if (!overridden)
            {
                var serializer = new FhirOutputFormatSerializer(context.HttpContext);
                await serializer.SerializeToResponseAsync(true, context.Object);
            }
        }

        /// <inheritdoc />
        protected override bool CanWriteType(Type type)
        {
            if (type == null)
            {
                return false;
            }

            return type == typeof(Hl7.Fhir.Model.Resource)
                   || type.IsSubclassOf(typeof(Hl7.Fhir.Model.Resource))
                   || type == typeof(MeldRxCdsEventResponse)
                   || type == typeof(CdsServiceResponse);
        }
    }
}
