namespace MeldRx.Account.Pages
{
    public class PatientSearchModel
    {
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fhirServer"></param>
        /// <param name="returnUrl"></param>
        /// <param name="scopesList"></param>
        /// <param name="clientId"></param>
        public PatientSearchModel(string fhirServer, List<string> scopesList, string clientId,string returnUrl) 
        {
            FhirServer = fhirServer;
            ScopesList = scopesList;
            ClientId = clientId;
            ReturnUrl=returnUrl;
        }

        public string FhirServer { get; set; }
        public List<string> ScopesList { get; set; }
        public string ClientId { get; set; }
        public string ReturnUrl { get; set; }
        public string AccessToken { get; set; }

    }
}
