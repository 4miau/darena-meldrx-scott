using System.Text.RegularExpressions;
using Microsoft.Extensions.Primitives;

namespace MeldRx.Account.Extensions
{
    /// <summary>
    /// Class that contains extensions to the <see cref="string"/> primitive type
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Parses out the fhir server name from a url. We know that the fhir server name is always at the end of a
        /// url which comes after the last "/" character. Example: https://www.example.com/test. For this url, the
        /// fhir server name is "test".
        ///
        /// This method does not check to see if the source string is a valid url. This method should only be called
        /// when the source string is known to be a fhir server url.
        /// </summary>
        /// <param name="self">The source string values. The source string is assumed to be the first item in this list of values</param>
        /// <returns>The fhir server name from a url</returns>
        public static string GetFhirServerFromUrl(this StringValues self)
        {
            if (self.Count != 1)
            {
                throw new ArgumentException("The list of audience values must contain one value");
            }

            var str = self.First();
            return GetFhirServerFromUrl(str);
        }

        private static readonly Regex FhirServerRegex = new(@"fhir\/(?<FhirServerSlug>[^\/]+)");

        /// <summary>
        /// Parses out the fhir server name from a url. We know that the fhir server name is always at the end of a
        /// url which comes after the last "/" character. Example: https://www.example.com/test. For this url, the
        /// fhir server name is "test".
        ///
        /// This method does not check to see if the source string is a valid url. This method should only be called
        /// when the source string is known to be a fhir server url.
        /// </summary>
        /// <param name="str">The source string.</param>
        /// <returns>The fhir server name from a url</returns>
        public static string GetFhirServerFromUrl(this string str)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(str);

            var match = FhirServerRegex.Match(str);
            if (match.Success)
            {
                return match.Groups["FhirServerSlug"].Value;
            }

            throw new ArgumentException("The string could not be parsed");
        }
    }
}
