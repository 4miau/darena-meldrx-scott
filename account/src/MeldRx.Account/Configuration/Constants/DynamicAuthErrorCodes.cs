namespace MeldRx.Server.Constants
{
    /// <summary>
    /// The different error codes supported for dynamic oauth client registration
    /// </summary>
    public static class DynamicAuthErrorCodes
    {
        /// <summary>
        /// The error occurred because of an invalid redirect uri
        /// </summary>
        public const string InvalidRedirectUri = "invalid_redirect_uri";

        /// <summary>
        /// The error occurred because of invalid client metadata
        /// </summary>
        public const string InvalidClientMetadata = "invalid_client_metadata";
    }
}
