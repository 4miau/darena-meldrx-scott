// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

// Original file: https://github.com/DuendeSoftware/IdentityServer.Quickstart.UI
// Modified by Jan Škoruba

using Duende.IdentityServer.Validation;
using Meldh.Fhir.Core.Interfaces;
using MeldRx.Account.Configuration;
using MeldRx.Account.Helpers;
using MeldRx.Account.ViewModels.Consent;
using MeldRx.Account.ViewModels.Home;
using MeldRx.Sdk.ApiClient.MeldRxApi;
using MeldRx.Services.Shared.Repositories;
using MeldRx.Services.Shared.Services;
using ApiScope = Duende.IdentityServer.Models.ApiScope;
using IdentityResource = Duende.IdentityServer.Models.IdentityResource;

namespace MeldRx.Account.Controllers
{
    /// <summary>
    /// This controller processes the consent UI
    /// </summary>
    [SecurityHeaders]
    [Authorize]
    public class ConsentController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IEventService _events;
        private readonly SettingsService _settingsService;
        private readonly IFhirRecordGrantsProvider _fhirRecordGrantsProvider;
        private readonly IFhirServerRepository _fhirServerRepository;
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<ConsentController> _logger;

        public ConsentController(
            IIdentityServerInteractionService interaction,
            IEventService events,
            SettingsService settingsService,
            IFhirRecordGrantsProvider fhirRecordGrantsProvider,
            IFhirServerRepository fhirServerRepository,
            ApplicationDbContext dbContext,
            ILogger<ConsentController> logger)
        {
            _interaction = interaction;
            _events = events;
            _settingsService = settingsService;
            _fhirRecordGrantsProvider = fhirRecordGrantsProvider;
            _fhirServerRepository = fhirServerRepository;
            _dbContext = dbContext;
            _logger = logger;
        }

        /// <summary>
        /// Shows the consent screen
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index(string returnUrl)
        {
            var buildResult = await BuildViewModelAsync(returnUrl);
            if (buildResult.Success)
            {
                return View("Index", buildResult.Data);
            }

            return View(
                "Error",
                new ErrorViewModel()
                {
                    Error = new ErrorMessage()
                    {
                        Error = buildResult.ErrorMessage
                    }
                }
            );
        }

        /// <summary>
        /// Handles the consent screen postback
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ConsentInputModel model)
        {
            var result = await ProcessConsent(model);

            if (result.IsRedirect)
            {
                var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);
                if (context?.IsNativeClient() == true)
                {
                    // The client is native, so this change in how to
                    // return the response is for better UX for the end user.
                    return this.LoadingPage("Redirect", result.RedirectUri);
                }

                return Redirect(result.RedirectUri);
            }

            if (result.HasValidationError)
            {
                ModelState.AddModelError(string.Empty, result.ValidationError);
            }

            if (result.ShowView)
            {
                return View("Index", result.ViewModel);
            }

            return View("Error");
        }

        /*****************************************/
        /* helper APIs for the ConsentController */
        /*****************************************/
        private async Task<ProcessConsentResult> ProcessConsent(ConsentInputModel model)
        {
            var result = new ProcessConsentResult();

            // validate return url is still valid
            var request = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);
            if (request == null) return result;

            ConsentResponse grantedConsent = null;

            // user clicked 'no' - send back the standard 'access_denied' response
            if (model?.Button == "no")
            {
                grantedConsent = new ConsentResponse { Error = AuthorizationError.AccessDenied };

                // emit event
                await _events.RaiseAsync(new ConsentDeniedEvent(User.GetSubjectId(), request.Client.ClientId, request.ValidatedResources.RawScopeValues));
            }
            // user clicked 'yes' - validate the data
            else if (model?.Button == "yes")
            {
                // if the user consented to some scope, build the response model
                if (model.ScopesConsented != null && model.ScopesConsented.Any())
                {
                    var scopes = model.ScopesConsented;
                    if (ConsentOptions.EnableOfflineAccess == false)
                    {
                        scopes = scopes.Where(x => x != global::Duende.IdentityServer.IdentityServerConstants.StandardScopes.OfflineAccess);
                    }

                    grantedConsent = new ConsentResponse
                    {
                        RememberConsent = model.RememberConsent,
                        ScopesValuesConsented = scopes.ToArray(),
                        Description = model.Description
                    };

                    // emit event
                    await _events.RaiseAsync(new ConsentGrantedEvent(User.GetSubjectId(), request.Client.ClientId, request.ValidatedResources.RawScopeValues, grantedConsent.ScopesValuesConsented, grantedConsent.RememberConsent));
                }
                else
                {
                    result.ValidationError = ConsentOptions.MustChooseOneErrorMessage;
                }
            }
            else
            {
                result.ValidationError = ConsentOptions.InvalidSelectionErrorMessage;
            }

            if (grantedConsent != null)
            {
                // communicate outcome of consent back to identityserver
                await _interaction.GrantConsentAsync(request, grantedConsent);

                // indicate that's it ok to redirect back to authorization endpoint
                result.RedirectUri = model.ReturnUrl;
                result.Client = request.Client;
            }
            else
            {
                // we need to redisplay the consent UI
                var consentResult = await BuildViewModelAsync(model.ReturnUrl, model);
                result.ViewModel = consentResult.Data;
            }

            return result;
        }

        private async Task<OperationResult<ConsentViewModel>> BuildViewModelAsync(string returnUrl, ConsentInputModel model = null)
        {
            var request = await _interaction.GetAuthorizationContextAsync(returnUrl);
            var fhirUrl = returnUrl.GetQueryValueFromUrl(JwtClaimTypes.Audience);
            var fhirServerName = string.IsNullOrEmpty(fhirUrl) ? null : fhirUrl.GetFhirServerNameFromUrl();
            //var settings = await _settingsClient.Get(fhirServerName);

            if (request != null)
            {
                return await CreateConsentViewModel(model, returnUrl, request, fhirServerName);
            }
            else
            {
                _logger.LogError("No consent request matching request: {0}", returnUrl);
            }

            return OperationResult<ConsentViewModel>.Succeed();
        }

        private async Task<OperationResult<ConsentViewModel>> CreateConsentViewModel(
            ConsentInputModel model,
            string returnUrl,
            AuthorizationRequest request,
            string fhirServerName
            )
        {
            var drr = await _dbContext.DynamicRegistrationRequests
                .AsNoTracking().Include(dynamicRegistrationRequest => dynamicRegistrationRequest.Organization)
                .FirstOrDefaultAsync(d=>d.Client.ClientId==request.Client.ClientId);
            var vm = new ConsentViewModel
                {
                    RememberConsent = model?.RememberConsent ?? true,
                    ScopesConsented = model?.ScopesConsented ?? Enumerable.Empty<string>(),
                    Description = model?.Description,
                    ReturnUrl = returnUrl,
                    AppName = request.Client.ClientName ?? request.Client.ClientId,
                   
                    AppLogoUrl = request.Client.LogoUri,
                    AllowRememberConsent = request.Client.AllowRememberConsent
                };
                if (drr?.Organization != null)
                {
                    vm.AppPublisherName = drr.Organization.Name;
                    vm.AppPublisherTosUrl = drr.TermsOfServiceUrl;
                    vm.AppPublisherPrivacyPolicyUrl = drr.PrivacyPolicyUrl;
                    vm.AppPublisherUrl = request.Client.ClientUri;
                }
                else
                {
                    vm.AppPublisherName = "Darena Solutions";
                    vm.AppPublisherTosUrl = "https://darenasolutions.com/terms-of-service";
                    vm.AppPublisherPrivacyPolicyUrl = "https://darenasolutions.com/privacy-policy";
                    vm.AppPublisherUrl = request.Client.ClientUri;
                }
                if (!string.IsNullOrEmpty(fhirServerName))
                {
                    // Since this query runs in anonymous context, we have to setup the FHIR server manually for this request
                    var fhirServer = await _fhirServerRepository.FindByDisplayNameAsync(fhirServerName);
                    if (fhirServer == null)
                    {
                        return OperationResult<ConsentViewModel>.Error($"{fhirServerName} fhir server doesn't exist.");
                    }

                    _fhirRecordGrantsProvider.WorkspaceDto = new WorkspaceDto()
                    {
                        Id = fhirServer.Id,
                        FhirDatabaseDisplayName = fhirServer.FhirDatabaseDisplayName,
                        Type = FhirServerType.Organizational
                    };

                    var settings = await _settingsService.GetSettings(true);
                    if (settings != null)
                    {
                        var result = settings;
                        vm.WorkspaceName = fhirServer.Name;
                        if (result.HasLogo)
                        {
                            var imageStream = await _settingsService.GetLogo();
                            if (imageStream != null)
                            {
                                var ms = new MemoryStream();
                                imageStream.CopyTo(ms);
                                vm.WorkspaceLogoUrl = $"data:image/png;base64,{Convert.ToBase64String(ms.ToArray())}";
                            }
                        }
                    }
                }

                vm.IdentityScopes = request.ValidatedResources.Resources.IdentityResources.Select(x => CreateScopeViewModel(x, vm.ScopesConsented.Contains(x.Name) || model == null)).ToArray();

                var apiScopes = new List<ScopeViewModel>();
                foreach (var parsedScope in request.ValidatedResources.ParsedScopes)
                {
                    var apiScope = request.ValidatedResources.Resources.FindApiScope(parsedScope.ParsedName);
                    if (apiScope != null)
                    {
                        var scopeVm = CreateScopeViewModel(parsedScope, apiScope, vm.ScopesConsented.Contains(parsedScope.RawValue) || model == null);
                        apiScopes.Add(scopeVm);
                    }
                }
                if (ConsentOptions.EnableOfflineAccess && request.ValidatedResources.Resources.OfflineAccess)
                {
                    apiScopes.Add(GetOfflineAccessScope(vm.ScopesConsented.Contains(global::Duende.IdentityServer.IdentityServerConstants.StandardScopes.OfflineAccess) || model == null));
                }
                vm.ApiScopes = apiScopes;
                vm.Scopes = apiScopes.Concat(vm.IdentityScopes).OrderByDescending(t => t.Required);
                return OperationResult<ConsentViewModel>.Succeed(vm);
            
        }

        private ScopeViewModel CreateScopeViewModel(IdentityResource identity, bool check)
        {
            return new ScopeViewModel
            {
                Value = identity.Name,
                DisplayName = identity.DisplayName ?? identity.Name,
                Description = identity.Description,
                Emphasize = identity.Emphasize,
                Required = identity.Required,
                Checked = check || identity.Required
            };
        }

        public ScopeViewModel CreateScopeViewModel(ParsedScopeValue parsedScopeValue, ApiScope apiScope, bool check)
        {
            var displayName = apiScope.DisplayName ?? apiScope.Name;
            if (!String.IsNullOrWhiteSpace(parsedScopeValue.ParsedParameter))
            {
                displayName += ":" + parsedScopeValue.ParsedParameter;
            }

            if (apiScope.Name.Contains("patient/") && apiScope.Name.Contains(".read"))
            {
                //patient/*.read or patient/Condition.read -> find * or Condition
                var resource = apiScope.Name.Split('.')[0].Replace("patient/", string.Empty);
                displayName = resource == "*" ? "Download All" : $"Download {resource}s";
            }

            return new ScopeViewModel
            {
                Value = parsedScopeValue.RawValue,
                DisplayName = displayName,
                Description = apiScope.Description,
                Emphasize = apiScope.Emphasize,
                Required = apiScope.Required,
                Checked = check || apiScope.Required
            };
        }

        private ScopeViewModel GetOfflineAccessScope(bool check)
        {
            return new ScopeViewModel
            {
                Value = global::Duende.IdentityServer.IdentityServerConstants.StandardScopes.OfflineAccess,
                DisplayName = ConsentOptions.OfflineAccessDisplayName,
                Description = ConsentOptions.OfflineAccessDescription,
                Emphasize = true,
                Checked = check
            };
        }
    }
}
