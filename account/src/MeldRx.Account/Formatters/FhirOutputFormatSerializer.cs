using Hl7.Fhir.Serialization;

namespace DarenaSolutions.Bbp.Api.Formatters
{
    /// <summary>
    /// The class that outputs a serialized FHIR resource in a certain string representation
    /// </summary>
    public class FhirOutputFormatSerializer
    {
        private readonly HttpContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="FhirOutputFormatSerializer"/> class
        /// </summary>
        /// <param name="context">The http context</param>
        public FhirOutputFormatSerializer(HttpContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Serializes a FHIR resource to the response in either the JSON or XML representation. This method
        /// will also set the response content type
        /// </summary>
        /// <param name="isJson">Indicate if the response content should be in the JSON representation or XML representation</param>
        /// <param name="objToSerialize">The FHIR resource to serialize into the response content</param>
        /// <returns>An awaitable task</returns>
        public async Task SerializeToResponseAsync(bool isJson, object objToSerialize)
        {
            string str;
            if (isJson)
            {
                await _context.Response.WriteAsJsonAsync(objToSerialize, options: FhirJsonSerializerOptionsFactory.GetFhirJsonSerializerOptions());
            }
            else
            {
                var xmlSerializer = new FhirXmlSerializer();
                str = xmlSerializer.SerializeToString((Hl7.Fhir.Model.Resource)objToSerialize);

                _context.Response.ContentType = "application/xml";
                await _context.Response.WriteAsync(str);
            }
        }
    }
}
