//using System;
//using Hl7.Fhir.Model;
//using Hl7.Fhir.Serialization;

//namespace DarenaSolutions.BlueButtonPro.Api.Formatters
//{
//    /// <summary>
//    /// The input formatter for an XML representation of a FHIR resource
//    /// </summary>
//    public class FhirXmlInputFormatter : BaseInputFormatter
//    {
//        /// <summary>
//        /// Initializes a new instance of the <see cref="FhirXmlInputFormatter"/> class
//        /// </summary>
//        public FhirXmlInputFormatter()
//            : base("xml", "text/xml", "application/xml", "application/fhir+xml")
//        {
//        }

//        /// <inheritdoc />
//        public override Base ParseStr(string str, Type type)
//        {
//            var parser = new FhirXmlParser();
//            return parser.Parse(str, type);
//        }
//    }
//}
