using System.Net;
using System.Net.Http.Headers;
using Azure.Core;
using Hl7.Fhir.Rest;
using Meldh.Fhir.Core.Interfaces;
using MeldRx.Account.Services;
using MeldRx.Account.Views.Shared;
using MeldRx.Sdk.Extensions.Fhir;
using MeldRx.Sdk.Fhir.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FhirClient = Hl7.Fhir.Rest.FhirClient;

namespace MeldRx.Account.Pages
{
    [ValidateAntiForgeryToken]
    public class PatientSelectionModel : PageModel
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public PatientSelectionModel(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        [BindProperty]
        public string FhirUrl { get; set; }
        [BindProperty]
        public string ReturnUrl { get; set; }
        [BindProperty]
        public string PatientId { get; set; }
        [BindProperty]
        public string SearchText { get; set; }
        [BindProperty]
        public int PageNumber { get; set; }
        [BindProperty]
        public PagedResult<PatientDto> PatientDtos { get; set; } = new PagedResult<PatientDto> { };
        public string Message { get; set; }

        public Task<IActionResult> OnPostAsync(string actionString)
        {
            var action = Enum.Parse<PatientSelectionAction>(actionString);
            switch (action)
            {
                case PatientSelectionAction.SelectPatient:
                    return Task.FromResult(SelectPatient());
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }
        }

        public Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            ReturnUrl=returnUrl;
            var isValidCall = true;
            if (returnUrl == null)
            {
                isValidCall = false;
            }
            else
            {
                FhirUrl = returnUrl.GetQueryValueFromUrl(JwtClaimTypes.Audience);
                if (string.IsNullOrEmpty(FhirUrl))
                {
                    isValidCall = false;
                }
            }

            if (!isValidCall)
            {
                //TODO return page with error message
            }
            return Task.FromResult<IActionResult>(Page());
        }

        protected IActionResult SelectPatient()
        {
            var encodedPatientLaunchQueryParameter = WebUtility.UrlEncode(SmartOnFhirConstants.LaunchPatientScopeName);
            var encodedPatientId = WebUtility.UrlEncode(PatientId);
            var redirectUrl = $"{ReturnUrl}&{encodedPatientLaunchQueryParameter}={encodedPatientId}";
            return LocalRedirect(redirectUrl);
        }

        public async Task<IActionResult> OnGetSearchPatients(string searchText, string fhirUrl,int pageNo=1)
        {
            searchText = string.IsNullOrEmpty(searchText) ? string.Empty: searchText;
            var result =  await SearchPatient(searchText, fhirUrl, pageNo);
            if (!result.Success)
            {
                Message = $"An unknown error occured";
                return Content(Message);
            }
            else
            {
                PatientDtos = result.Data;
                if(PatientDtos.Resources == null || PatientDtos.Resources.Count == 0)
                {
                    Message = $"No Record Found";
                    return Content(Message);
                }
                else
                {
                    return Partial("_patientList", new _patientListModel { PatientDtos = PatientDtos.Resources, PatientId = PatientId, PageNumber = pageNo, PageCount = PatientDtos.TotalPages, Total = PatientDtos.Total });
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchText"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public async Task<OperationResult<PagedResult<PatientDto>>> SearchPatient(string searchText,string fhirUrl,int page =1)
        {

          var handler = new FhirClientAuthorizationMessageHandler();
           handler.RequestCookies = _contextAccessor?.HttpContext?.Request.Cookies;
           var settings = new FhirClientSettings
            {
                //Timeout = 0,
                PreferredFormat = ResourceFormat.Json,
                //VerifyFhirVersion = true,
                //PreferredReturn = Prefer.ReturnMinimal
            };
            var client = new FhirClient(fhirUrl, settings, handler);
            try
            {
                var pageSize = 10;
                var resultFhir =await client.SearchPatients(searchText, page, pageSize);
                return resultFhir;
            }
            catch (Exception e)
            {
                Log.Error(e, "Smart Patient Search");
                return OperationResult<PagedResult<PatientDto>>.Error(e.Message);

            }
        }

        public enum PatientSelectionAction
        {
            SearchPatient,
            SelectPatient,
            PageChange
        }
    }
}