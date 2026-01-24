namespace MeldRx.Account.Helpers
{
    public static class UrlHelpers
    {
        public static string QueryStringSafeHash(string hash)
        {
            hash = hash.Replace('+', '-');
            return hash.Replace('/', '_');
        }

        public static string QueryStringUnSafeHash(string hash)
        {
            hash = hash.Replace('-', '+');
            return hash.Replace('_', '/');
        }

        /// <summary>
        /// Returns true if the id is not a valid number
        /// </summary>
        /// <param name="id">String to check if it's not a valid number</param>
        /// <returns>True if the id is not a valid number</returns>
        public static bool IsNotPresentedValidNumber(this string id)
        {
            int.TryParse(id, out var parsedId);

            return !string.IsNullOrEmpty(id) && parsedId == default;
        }
    }
}
