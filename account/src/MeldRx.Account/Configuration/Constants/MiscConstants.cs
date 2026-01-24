using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarenaSolutions.Iam.Server.Constants
{
    public static class MiscConstants
    {
        public static string FhirApiProviderDtosKey= "FhirApiProvidersList";
        /// <summary>
        /// The key identifier that is used in <see cref="HttpContext.Items"/> to store the FHIR server that is being used
        /// in a SMART-on-FHIR authentication process. The FHIR server is needed in various areas in the same request, so
        /// it is easier to store it in the current context items for easy access rather than retrieving from the database.
        /// Since the context items are only valid for the current request and are cleared out after the request is completed,
        /// it is also safe
        /// </summary>
        public const string SofFhirServerCacheKey = "SOF_FHIR_SERVER_CACHE";

        public const string FolderForPublicSampleData = "sample_data";
    }
}
