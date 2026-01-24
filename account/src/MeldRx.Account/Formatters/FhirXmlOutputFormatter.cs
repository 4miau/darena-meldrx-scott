using System.Text;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace DarenaSolutions.Bbp.Api.Formatters
{
    /// <summary>
    /// The output formatter for an XML representation of a FHIR resource
    /// </summary>
    public class FhirXmlOutputFormatter : TextOutputFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FhirXmlOutputFormatter"/> class
        /// </summary>
        public FhirXmlOutputFormatter()
        {
            SupportedMediaTypes.Add("xml");
            SupportedMediaTypes.Add("text/xml");
            SupportedMediaTypes.Add("application/xml");
            SupportedMediaTypes.Add("application/fhir+xml");
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
                await serializer.SerializeToResponseAsync(false, context.Object);
            }
        }

        /// <inheritdoc />
        protected override bool CanWriteType(Type type)
        {
            if (type == null)
            {
                return false;
            }
            if (type != typeof(Hl7.Fhir.Model.Resource) && !type.IsSubclassOf(typeof(Hl7.Fhir.Model.Resource)))
            {
                return false;
            }

            return base.CanWriteType(type);
        }
    }
}
