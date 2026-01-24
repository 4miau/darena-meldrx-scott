using DarenaSolutions.Bbp.Api.Extensions;
using MeldRx.Account.Areas.Api;
using MeldRx.Account.Configuration;
using MeldRx.Account.SmartOnFhirServices;
using MeldRx.Account.ViewModels.Account;
using MeldRx.Sdk.Dtos.Organization;
using MeldRx.Sdk.Internal.Pocos;
using MeldRx.Services.Shared.Repositories;
using MeldRx.Services.Shared.Services.Users;

namespace MeldRx.Account.Controllers;

/// <summary>
///
/// </summary>
[SecurityHeaders]
[Authorize]
public class AccountController : BaseController
{
    private readonly IDistributedCacheManager _distributedCacheManager;
    private readonly IFeatureManager _featureManager;
    private readonly UserResolver<ApplicationUser> _userResolver;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IIdentityServerInteractionService _interaction;
    private readonly IClientStore _clientStore;
    private readonly IAuthenticationSchemeProvider _schemeProvider;
    private readonly IEventService _events;
    private readonly IMeldRxEmailSender _emailSender;
    private readonly IStringLocalizer<AccountController> _localizer;
    private readonly RegisterConfiguration _registerConfiguration;
    private readonly MeldRxSettings _meldRxSettings;
    private readonly IMeldRxEmailSender _meldRxEmailSender;
    private readonly LinkedFhirApiService _linkedFhirApiService;
    private readonly IdentityOptions _identityOptions;
    private readonly ILogger<AccountController> _logger;
    private readonly RegisterUser _registerUser;
    private readonly IAuthenticationHandlerProvider _authenticationHandlerProvider;
    private readonly ApplicationDbContext _dbContext;
    private readonly IFhirServerRepository _fhirServerRepository;
    private readonly GetUserPermissions _getUserPermissions;

    public AccountController(
        IDistributedCacheManager distributedCacheManager,
        IFeatureManager featureManager,
        UserResolver<ApplicationUser> userResolver,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IIdentityServerInteractionService interaction,
        IClientStore clientStore,
        IAuthenticationSchemeProvider schemeProvider,
        IEventService events,
        IMeldRxEmailSender emailSender,
        IStringLocalizer<AccountController> localizer,
        RegisterConfiguration registerConfiguration,
        IOptions<IdentityOptions> identityOptions,
        MeldRxSettings meldRxSettings,
        IMeldRxEmailSender meldRxEmailSender,
        LinkedFhirApiService linkedFhirApiService,
        ILogger<AccountController> logger,
        RegisterUser registerUser,
        IAuthenticationHandlerProvider authenticationHandlerProvider,
        ApplicationDbContext dbContext,
        IFhirServerRepository fhirServerRepository,
        GetUserPermissions getUserPermissions
    ) : base(logger)

    {
        _distributedCacheManager = distributedCacheManager;
        _featureManager = featureManager;
        _userResolver = userResolver;
        _userManager = userManager;
        _signInManager = signInManager;
        _interaction = interaction;
        _clientStore = clientStore;
        _schemeProvider = schemeProvider;
        _events = events;
        _emailSender = emailSender;
        _localizer = localizer;
        _registerConfiguration = registerConfiguration;
        _meldRxSettings = meldRxSettings;
        _meldRxEmailSender = meldRxEmailSender;
        _linkedFhirApiService = linkedFhirApiService;
        _identityOptions = identityOptions.Value;
        _logger = logger;
        _registerUser = registerUser;
        _authenticationHandlerProvider = authenticationHandlerProvider;
        _dbContext = dbContext;
        _fhirServerRepository = fhirServerRepository;
        _getUserPermissions = getUserPermissions;
    }

    public IActionResult AccessDenied()
    {
        return View();
    }

    private static readonly IReadOnlyList<string> MeClaimsAllowList =
    [
        JwtClaimTypes.Subject,
        JwtClaimTypes.ClientId,
        JwtClaimTypes.PreferredUserName,
        JwtClaimTypes.Name,
        JwtClaimTypes.FamilyName,
        JwtClaimTypes.GivenName,
        JwtClaimTypes.Email,
        JwtClaimTypes.Role,
        AuthConstants.ClaimTypes.Launch,
        AuthConstants.ClaimTypes.LinkedWorkspaceId,
    ];

    [HttpGet]
    public async Task<IActionResult> Me([FromServices] GetUserPermissions getUserPermissions)
    {
        var permissions = await getUserPermissions.ByUserId(User.GetUserId());

        if (permissions == null)
        {
            await _signInManager.SignOutAsync();
            return StatusCode(401);
        }

        return Ok(
            new ApplicationUserInfo
            {
                Claims = User.Claims
                    .Where(x => MeClaimsAllowList.Contains(x.Type))
                    .ToDictionary(x => x.Type, x => x.Value),
                Permissions = permissions,
            }
        );
    }

    /// <summary>
    /// Entry point into the login workflow
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Login(string returnUrl)
    {
        if (_meldRxSettings.DefaultLoginScheme == DefaultLoginSchemes.Aad)
        {
            return RedirectToAction(nameof(AadController.AadLogin), "Aad", new { returnUrl = returnUrl });
        }

        //Check if MeldRxLaunchIdParameter is passed. If yes, check for the EHR context in the database
        var meldrxLaunchIdOr = SofUtilities.GetParameterValueFromPartialUrl(returnUrl, SmartOnFhirConstants.MeldRxLaunchIdParameter);
        if (!meldrxLaunchIdOr.NotFound)
        {
            var sofContext = await _distributedCacheManager.FindObjectAsync<ExternalSofLaunchContext>(meldrxLaunchIdOr.Data);
            var linkedApi = await _linkedFhirApiService.FindByWorkspaceUrlSlug(sofContext.WorkspaceSlug);
            if (linkedApi.Success)
            {
                return Challenge(
                    new AuthenticationProperties()
                    {
                        Items =
                        {
                            { JwtClaimTypes.ClientId, SofUtilities.GetParameterValueFromPartialUrl(returnUrl, JwtClaimTypes.ClientId).Data },
                            { JwtClaimTypes.Scope, SofUtilities.GetParameterValueFromPartialUrl(returnUrl, JwtClaimTypes.Scope).Data },
                            { JwtClaimTypes.Audience, SofUtilities.GetParameterValueFromPartialUrl(returnUrl, JwtClaimTypes.Audience).Data },
                            { SmartOnFhirConstants.LaunchParameterName, sofContext.Launch },
                            { SmartOnFhirConstants.IssuerParameter, sofContext.IssuerUrl },
                        },
                        RedirectUri = returnUrl
                    },
                    ExternalProviders.SoF
                );
            }
        }

        //Check if the audience or issuer URL was passed. If yes, kick of the SMART on FHIR flow
        //We need to check for 2 scenarios here. The audience parameter could be for a linked or a regular workspace.
        //For linked workspace, we need to kick off the SOF Authentication Provider. For Regular workspace, we just need to let
        //it follow our regular steps as it will handle it in the connect flow later.
        var linkedApiResult = await LoadLinkedApiFromReturnUrl(returnUrl);
        if (!linkedApiResult.Success)
        {
            return await RedirectToMeldRxOidcError(MeldRxOidcErrorCode.InvalidWorkspace);
        }

        var isLinked = linkedApiResult.Data != null;
        if (isLinked)
        {
            var app = await LoadAppFromReturnUrl(returnUrl);
            if (!app.Success || app.Data == null)
            {
                return await RedirectToMeldRxOidcError(MeldRxOidcErrorCode.InvalidAppClientId);
            }

            if (linkedApiResult.Data.PatientStrategy == LinkedWorkspacePatientStrategy.Default
                || app.Data.SoFAppUserType == SoFAppUserType.Provider)
            {
                return Challenge(
                    new AuthenticationProperties()
                    {
                        Items =
                        {
                            { JwtClaimTypes.ClientId, SofUtilities.GetParameterValueFromPartialUrl(returnUrl, JwtClaimTypes.ClientId).Data },
                            { JwtClaimTypes.Scope, SofUtilities.GetParameterValueFromPartialUrl(returnUrl, JwtClaimTypes.Scope).Data },
                            { JwtClaimTypes.Audience, SofUtilities.GetParameterValueFromPartialUrl(returnUrl, JwtClaimTypes.Audience).Data },
                            { SmartOnFhirConstants.LaunchParameterName, SofUtilities.GetParameterValueFromPartialUrl(returnUrl, SmartOnFhirConstants.LaunchParameterName).Data },
                            { SmartOnFhirConstants.IssuerParameter, SofUtilities.GetParameterValueFromPartialUrl(returnUrl, JwtClaimTypes.Issuer).Data },
                        },
                        RedirectUri = returnUrl,
                    },
                    ExternalProviders.SoF
                );
            }
        }

        //If override scheme is null, we can safely assume this is not a SOF redirect and just log the user in
        if (User.IsAuthenticated())
        {
            var permissions = await _getUserPermissions.ByUserId(User.GetUserId());
            if (permissions == null)
            {
                await _signInManager.SignOutAsync();
                return Redirect(returnUrl);
            }

            if (permissions.HasMipsReports && !permissions.HasWorkspaces && !permissions.HasPeople && !permissions.IsDeveloper)
            {
                return Redirect("/mymipsscore");
            }
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return Redirect("/"); // Adjust controller and action names as necessary
            }
        }

        // build a model so we know what to show on the login page
        var vm = await BuildLoginViewModelAsync(returnUrl);

        if (vm.EnableLocalLogin == false && vm.ExternalProviders.Count() == 1)
        {
            if (vm.ExternalProviders.First().AuthenticationScheme == ExternalProviders.AzureActiveDirectory)
            {
                return RedirectToAction(nameof(AadController.AadLogin), "Aad", new { returnUrl });
            }

            // only one option for logging in
            return ExternalLogin(vm.ExternalProviders.First().AuthenticationScheme, returnUrl);
        }

        return View(vm);
    }

    private async Task<LoginViewModel> BuildLoginViewModelAsync(string returnUrl, string overrideScheme = null)
    {
        var context = await _interaction.GetAuthorizationContextAsync(returnUrl);

        //If the request is through a client with linked App, we need to check the auth server url
        if (!string.IsNullOrEmpty(overrideScheme))
        {
            context.IdP = overrideScheme;
        }

        if (context?.IdP != null && await _schemeProvider.GetSchemeAsync(context.IdP) != null)
        {
            var local = context.IdP == IdentityServerConstants.LocalIdentityProvider;

            // this is meant to short circuit the UI and only trigger the one external IdP
            var vm = new LoginViewModel
            {
                EnableLocalLogin = local,
                ReturnUrl = returnUrl,
                Username = context?.LoginHint,
                LoginResolutionPolicy = _registerConfiguration.ResolutionPolicy
            };

            if (!local)
            {
                vm.ExternalProviders = new[] { new ExternalProvider { AuthenticationScheme = context.IdP } };
            }

            return vm;
        }

        var schemes = await _schemeProvider.GetAllSchemesAsync();

        var providers = schemes
            .Where(x => x.DisplayName != null && x.Name != ExternalProviders.SoF && x.Name != ExternalProviders.Cms)
            .Select(x => new ExternalProvider
            {
                DisplayName = x.DisplayName ?? x.Name,
                AuthenticationScheme = x.Name
            }).ToList();

        var allowLocal = true;
        if (context?.Client.ClientId != null)
        {
            var client = await _clientStore.FindEnabledClientByIdAsync(context.Client.ClientId);
            if (client != null)
            {
                allowLocal = client.EnableLocalLogin;

                if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
                {
                    providers = providers.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
                }
            }
        }

        return new LoginViewModel
        {
            AllowRememberLogin = AccountOptions.AllowRememberLogin,
            EnableLocalLogin = allowLocal && AccountOptions.AllowLocalLogin,
            ReturnUrl = returnUrl,
            Username = context?.LoginHint,
            LoginResolutionPolicy = _registerConfiguration.ResolutionPolicy,
            ExternalProviders = providers.ToArray()
        };
    }

    /// <summary>
    /// Handle postback from username/password login
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginInputModel model, string button)
    {
        // check if we are in the context of an authorization request
        var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);

        // the user clicked the "cancel" button
        if (button != "login")
        {
            if (context != null)
            {
                // if the user cancels, send a result back into IdentityServer as if they
                // denied the consent (even if this client does not require consent).
                // this will send back an access denied OIDC error response to the client.
                await _interaction.DenyAuthorizationAsync(context, AuthorizationError.AccessDenied);

                // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                if (context.IsNativeClient())
                {
                    // The client is native, so this change in how to
                    // return the response is for better UX for the end user.
                    return this.LoadingPage("Redirect", model.ReturnUrl);
                }

                return Redirect(model.ReturnUrl);
            }

            // since we don't have a valid context, then we just go back to the home page
            return Redirect("~/");
        }

        if (!ModelState.IsValid)
        {
            return await InvalidCredentials();
        }

        if (string.IsNullOrEmpty(model.ReturnUrl)) model.ReturnUrl = "~/";

        var user = await _userResolver.GetUserAsync(model.Username);
        if (user == null)
        {
            return await InvalidCredentials();
        }

        if (!user.Active)
        {
            return await InvalidCredentials();
        }

        if (!user.EmailConfirmed)
        {
            return RedirectToAction(nameof(OrganizationRequestController.Status), "OrganizationRequest");
        }

        var checkPasswordResult = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberLogin, true);
        if (!checkPasswordResult.Succeeded)
        {
            if (checkPasswordResult.RequiresTwoFactor)
            {
                return RedirectToAction(nameof(LoginWith2fa), new { model.ReturnUrl, RememberMe = model.RememberLogin });
            }

            if (checkPasswordResult.IsLockedOut)
            {
                return View("Lockout");
            }

            return await InvalidCredentials();
        }

        await _events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.UserName));

        if (context != null)
        {
            if (context.IsNativeClient())
            {
                // The client is native, so this change in how to
                // return the response is for better UX for the end user.
                return this.LoadingPage("Redirect", model.ReturnUrl);
            }

            // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
            return Redirect(model.ReturnUrl);
        }

        if (string.IsNullOrEmpty(model.ReturnUrl))
        {
            return Redirect(_meldRxSettings.AuthorityUrl);
        }

        if (model.ReturnUrl.ToLower().Contains("mymipsscore"))
        {
            return Redirect(model.ReturnUrl);
        }

        var permissions = await _getUserPermissions.ByUserId(user.Id);
        if (permissions.HasMipsReports && !permissions.HasWorkspaces && !permissions.HasPeople && !permissions.IsDeveloper)
        {
            return Redirect("/mymipsscore");
        }


        if (model.ReturnUrl.StartsWith(_meldRxSettings.AuthorityUrl, StringComparison.CurrentCultureIgnoreCase))
        {
            return Redirect(model.ReturnUrl);
        }

        // request for a local page
        if (Url.IsLocalUrl(model.ReturnUrl))
        {
            return Redirect(model.ReturnUrl);
        }

        // user might have clicked on a malicious link - should be logged
        throw new Exception("invalid return URL");

        async Task<IActionResult> InvalidCredentials()
        {
            await _events.RaiseAsync(new UserLoginFailureEvent(model.Username, "invalid credentials", clientId: context?.Client.ClientId));
            ModelState.AddModelError(string.Empty, AccountOptions.InvalidCredentialsErrorMessage);
            ViewBag.Errormessage = AccountOptions.InvalidCredentialsErrorMessage;
            var vm = await BuildLoginViewModelAsync(model);
            return View(vm);
        }
    }


    /// <summary>
    /// Show logout page
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Logout(string logoutId)
    {
        // build a model so the logout page knows what to display
        var vm = await BuildLogoutViewModelAsync(logoutId);

        if (vm.ShowLogoutPrompt == false)
        {
            // if the request for logout was properly authenticated from IdentityServer, then
            // we don't need to show the prompt and can just log the user out directly.
            return await Logout(vm);
        }

        return View(vm);
    }

    /// <summary>
    /// Handle logout page postback
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout(LogoutInputModel model)
    {
        var context = await _interaction.GetLogoutContextAsync(model.LogoutId);
        // build a model so the logged out page knows what to display
        var vm = await BuildLoggedOutViewModelAsync(model.LogoutId);

        if (User?.Identity.IsAuthenticated == true)
        {
            // delete local authentication cookie
            await _signInManager.SignOutAsync();

            // raise the logout event
            await _events.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));
        }

        // check if we need to trigger sign-out at an upstream identity provider
        if (vm.TriggerExternalSignout)
        {
            // build a return URL so the upstream provider will redirect back
            // to us after the user has logged out. this allows us to then
            // complete our single sign-out processing.
            string url = Url.Action("Logout", new { logoutId = vm.LogoutId });

            // this triggers a redirect to the external provider for sign-out
            return SignOut(new AuthenticationProperties { RedirectUri = url }, vm.ExternalAuthenticationScheme);
        }

        if (!string.IsNullOrEmpty(context.PostLogoutRedirectUri))
        {
            return Redirect(context.PostLogoutRedirectUri);
        }

        return Redirect("/");
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail(string userId, string code)
    {
        if (userId == null || code == null)
        {
            return View("Error");
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return View("Error");
        }

        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

        var result = await _userManager.ConfirmEmailAsync(user, code);
        return View(result.Succeeded ? "ConfirmEmail" : "Error");
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ForgotPassword()
    {
        return View(new ForgotPasswordViewModel());
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    [ValidateReCaptcha]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!await _featureManager.IsEnabledAsync(Features.IsCaptchaEnabled))
        {
            ModelState.Remove("Recaptcha"); //we will remove recaptcha validation if IsCaptchaEnabled key is false in app setting
        }

        if (ModelState.IsValid)
        {
            ApplicationUser user = null;
            switch (model.Policy)
            {
                case LoginResolutionPolicy.Email:
                    try
                    {
                        user = await _userManager.FindByEmailAsync(model.Email);
                    }
                    catch (Exception ex)
                    {
                        // in case of multiple users with the same email this method would throw and reveal that the email is registered
                        _logger.LogError("Error retrieving user by email ({0}) for forgot password functionality: {1}", model.Email, ex.Message);
                        user = null;
                    }

                    break;
                case LoginResolutionPolicy.Username:
                    try
                    {
                        user = await _userManager.FindByNameAsync(model.Username);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("Error retrieving user by userName ({0}) for forgot password functionality: {1}", model.Username, ex.Message);
                        user = null;
                    }

                    break;
            }

            // Allowing to use password resets for confirmed email messages
            if (user == null) // || !await _userManager.IsEmailConfirmedAsync(user))
            {
                // Don't reveal that the user does not exist
                return View("ForgotPasswordConfirmation");
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code.GetEncodedCode() }, HttpContext.Request.Scheme);

            await _meldRxEmailSender.SendEmail(user.Email, new GenericEmail
            {
                Name = user.FirstName,
                Subject = "Reset MeldRx Password",
                ActionLink = callbackUrl,
                ActionName = "Click Here",
                Title = "It happens!",
                Body1 = "We all forget!",
                Body2 = "Let's get you back on MeldRx."
            });
            // await _emailSender.SendEmailAsync(user.Email, _localizer["ResetPasswordTitle"], _localizer["ResetPasswordBody", HtmlEncoder.Default.Encode(callbackUrl)]);

            return View("ForgotPasswordConfirmation");
        }

        return View(model);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ResetPassword(string code = null, string userId = null)
    {
        return code == null || userId == null ? View("Error") : View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.FindByIdAsync(model.UserId);
        if (user == null)
        {
            // Don't reveal that the user does not exist
            return RedirectToAction(nameof(ResetPasswordConfirmation), "Account");
        }

        var result = await _userManager.ResetPasswordAsync(user, model.Code.GetDecodedCode(), model.Password);

        if (result.Succeeded)
        {
            if (result.Succeeded)
            {
                var emailWasNotConfirmed = !user.EmailConfirmed;
                user.IsPasswordTemporary = false;

                // Also confirm there email. They could not have come to this page with a valid token if they didn't receive
                // it in their inbox in the first place. Another situation where this is possible is when the user has successfully
                // signed into the system in the /Login page, but their password is set as temporary. If the email was not
                // confirmed, an invalid attempt would be displayed. If it was confirmed, then we redirect them to this
                // page, which again shows that we know the email should be confirmed.
                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);
                if (emailWasNotConfirmed)
                {
                    //_logger.Information(new EmailConfirmationEvent(user.Email, EmailConfirmationStrategy.PasswordReset));
                }

                //if (!string.IsNullOrEmpty(returnUrl))
                //{
                //    return Redirect(returnUrl);
                //}

                //return RedirectToPage("./ResetPasswordConfirmation", routeValues);
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return RedirectToAction(nameof(ResetPasswordConfirmation), "Account");
        }

        AddErrors(result);

        return View();
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ResetPasswordConfirmation()
    {
        return View();
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ForgotPasswordConfirmation()
    {
        return View();
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
    {
        if (remoteError != null)
        {
            ModelState.AddModelError(string.Empty, _localizer["ErrorExternalProvider", remoteError]);

            return View(nameof(Login));
        }

        var info = await _signInManager.GetExternalLoginInfoAsync();


        if (info == null)
        {
            return RedirectToAction(nameof(Login));
        }

        // Sign in the user with this external login provider if the user already has a login.
        //var result = await _signInManager.ExternalLoginSignInAsync(provider, providerUserId, isPersistent: false);
        var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
        if (result.Succeeded)
        {
            //This saves the credentials from the external provider
            await _signInManager.UpdateExternalAuthenticationTokensAsync(info);
            return RedirectToLocal(returnUrl);
        }

        if (result.RequiresTwoFactor)
        {
            return RedirectToAction(nameof(LoginWith2fa), new { ReturnUrl = returnUrl });
        }

        if (result.IsLockedOut)
        {
            return View("Lockout");
        }

        // If the user does not have an account, then ask the user to create an account.

        //Option to log them in with temporary cookie and save the token


        ViewData["ReturnUrl"] = returnUrl;
        ViewData["LoginProvider"] = info.ProviderDisplayName;
        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        var userName = info.Principal.Identity.Name;

        return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = email });
    }

    [HttpPost]
    [HttpGet]
    [AllowAnonymous]
    public IActionResult ExternalLogin(string provider, string returnUrl = null)
    {
        // Request a redirect to the external login provider.
        if (provider == ExternalProviders.AzureActiveDirectory)
        {
            return RedirectToAction(nameof(AadController.AadLogin), "Aad", new { returnUrl = returnUrl });
        }

        var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

        return Challenge(properties, provider);
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl = null)
    {
        returnUrl = returnUrl ?? Url.Content("~/");

        // Get the information about the user from the external login provider
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            return View("ExternalLoginFailure");
        }

        if (ModelState.IsValid)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                result = await _userManager.AddLoginAsync(user, info);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);
                    await _signInManager.UpdateExternalAuthenticationTokensAsync(info);

                    return RedirectToLocal(returnUrl);
                }
            }

            AddErrors(result);
        }

        ViewData["LoginProvider"] = info.LoginProvider;
        ViewData["ReturnUrl"] = returnUrl;

        return View(model);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> LoginWithRecoveryCode(string returnUrl = null)
    {
        // Ensure the user has gone through the username & password screen first
        var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
        if (user == null)
        {
            throw new InvalidOperationException(_localizer["Unable2FA"]);
        }

        var model = new LoginWithRecoveryCodeViewModel()
        {
            ReturnUrl = returnUrl
        };

        return View(model);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> LoginWithRecoveryCode(LoginWithRecoveryCodeViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
        if (user == null)
        {
            throw new InvalidOperationException(_localizer["Unable2FA"]);
        }

        var recoveryCode = model.RecoveryCode.Replace(" ", string.Empty);

        var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

        if (result.Succeeded)
        {
            return LocalRedirect(string.IsNullOrEmpty(model.ReturnUrl) ? "~/" : model.ReturnUrl);
        }

        if (result.IsLockedOut)
        {
            return View("Lockout");
        }

        ModelState.AddModelError(string.Empty, _localizer["InvalidRecoveryCode"]);

        return View(model);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> LoginWith2fa(bool rememberMe, string returnUrl = null)
    {
        // Ensure the user has gone through the username & password screen first
        var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

        if (user == null)
        {
            throw new InvalidOperationException(_localizer["Unable2FA"]);
        }

        var model = new LoginWith2faViewModel()
        {
            ReturnUrl = returnUrl,
            RememberMe = rememberMe
        };

        return View(model);
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LoginWith2fa(LoginWith2faViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
        if (user == null)
        {
            throw new InvalidOperationException(_localizer["Unable2FA"]);
        }

        var authenticatorCode = model.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

        var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, model.RememberMe, model.RememberMachine);

        if (result.Succeeded)
        {
            return LocalRedirect(string.IsNullOrEmpty(model.ReturnUrl) ? "~/" : model.ReturnUrl);
        }

        if (result.IsLockedOut)
        {
            return View("Lockout");
        }

        ModelState.AddModelError(string.Empty, _localizer["InvalidAuthenticatorCode"]);

        return View(model);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Register(string returnUrl = null)
    {
        // allow patient invites to visit this registration page.
        if (returnUrl?.StartsWith("/invite/") ?? false)
        {
            return await GetRegisterMethod(returnUrl, false);
        }

        return Redirect("/");
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterDeveloper(string returnUrl = null)
    {
        return await GetRegisterMethod(returnUrl, true);
    }

    private async Task<IActionResult> GetRegisterMethod(string returnUrl = null, bool isDeveloperAccount = false)
    {
        if (User.IsAuthenticated())
        {
            return Redirect("~/");
        }

        ViewData["ReturnUrl"] = returnUrl;

        var schemes = await _schemeProvider.GetAllSchemesAsync();

        var providers = schemes
            .Where(x => x.DisplayName != null)
            .Select(x => new ExternalProvider
            {
                DisplayName = x.DisplayName ?? x.Name,
                AuthenticationScheme = x.Name
            }).ToList();

        return _registerConfiguration.ResolutionPolicy switch
        {
            LoginResolutionPolicy.Username => View(),
            LoginResolutionPolicy.Email => View("Register", new RegisterWithoutUsernameViewModel
            {
                ExternalProviders = providers,
            }),
            _ => View("RegisterFailure")
        };
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    [ValidateReCaptcha]
    public async Task<IActionResult> Register(RegisterWithoutUsernameViewModel model, string returnUrl = null, bool isCalledFromRegisterWithoutUsername = false)
    {
        //if (!_registerConfiguration.Enabled) return View("RegisterFailure");

        if (!await _featureManager.IsEnabledAsync(Features.IsCaptchaEnabled))
        {
            ModelState.Remove("Recaptcha"); //we will remove recaptcha validation if IsCaptchaEnabled key is false in app setting
        }

        returnUrl ??= Url.Content("~/");

        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid) return View(model);

        var user = new ApplicationUser()
        {
            UserName = model.Email,
            Email = model.Email,
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code }, HttpContext.Request.Scheme);

            await _emailSender.SendEmail(model.Email, new GenericEmail
            {
                ActionLink = string.IsNullOrEmpty(returnUrl) ? callbackUrl : callbackUrl + "&returnUrl=" + returnUrl,
                ActionName = "Confirm Account",
                Body1 =
                    $"An account was recently registered with the email address {model.Email} on MeldRx. To confirm your account, please click on the link below.",
                Body2 = "If you did not create this account, please ignore this email.",
                Subject = "Confirm Your Account Registration",
                Name = model.Email,
                Title = "Confirm Your Account Registration"
            });

            return View("RegisterConfirmation");

        }
        else
        {
            AddErrors(result);
        }

        return View(model);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> LinkedWorkspaceRegisterForm(string returnUrl)
    {
        var signInResult = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
        if (!signInResult.Succeeded)
        {
            return Unauthorized();
        }

        return View(new LinkedWorkspaceRegisterForm() { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> LinkedWorkspaceRegisterForm(
        LinkedWorkspaceRegisterForm form,
        [FromServices] IAccountManager accountManager
        )
    {
        var signInResult = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
        if (!signInResult.Succeeded)
        {
            return Unauthorized();
        }

        try
        {
            var user = signInResult.Principal;

            var organizationIdClaim = user.FindFirstValue(AuthConstants.ClaimTypes.OrganizationId);
            ArgumentException.ThrowIfNullOrEmpty(organizationIdClaim);

            var externalIdClaim = user.FindFirstValue(AuthConstants.ClaimTypes.ExternalFhirApiSub);
            ArgumentException.ThrowIfNullOrEmpty(externalIdClaim);

            var (result, appUser) = await accountManager.RegisterAsync(
                new CreateApplicationUserDto
                {
                    FirstName = "SOF",
                    LastName = "User",
                    ExternalUserId = externalIdClaim,
                    Active = true,
                    OrganizationUserRelations =
                    [
                        new CreateModifyOrganizationUserRelationDto
                        {
                            OrganizationId = new Guid(organizationIdClaim),
                            OrganizationRole = OrganizationRoles.User
                        }
                    ],
                    SendRegistrationEmail = true,
                    Email = form.Email,
                },
                setPasswordAsTemporary: true,
                externalIdp: ExternalIdp.Sof
            );

            if (result.Succeeded)
            {
                await _signInManager.SignInWithClaimsAsync(appUser, new AuthenticationProperties(), user.Claims);
                return Redirect(form.ReturnUrl);
            }

            // email and username set to same value, so filter out username because we capture email.
            foreach (var error in result.Errors.Where(x => x.Code != "DuplicateUserName"))
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(form);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to handle linked workspace user registration.");
            return View("Error");
        }
    }

    /*****************************************/
    /* helper APIs for the AccountController */
    /*****************************************/

    private async Task<OperationResult<LinkedFhirApiDto?>> LoadLinkedApiFromReturnUrl(string returnUrl)
    {
        if (string.IsNullOrWhiteSpace(returnUrl) || returnUrl == "~/")
        {
            return OperationResult<LinkedFhirApiDto>.Succeed();
        }

        var workspaceUrl = SofUtilities.GetParameterValueFromPartialUrl(returnUrl, SmartOnFhirConstants.AudienceParameter);
        if (!workspaceUrl.Success)
        {
            workspaceUrl = SofUtilities.GetParameterValueFromPartialUrl(returnUrl, SmartOnFhirConstants.IssuerParameter);
        }

        var nonWorkspaceFlow = !workspaceUrl.Success;
        if (nonWorkspaceFlow)
        {
            return OperationResult<LinkedFhirApiDto>.Succeed();
        }

        var workspace = await _fhirServerRepository.FindByDisplayNameAsync(workspaceUrl.Data.GetFhirServerNameFromUrl());
        if (workspace == null)
        {
            return OperationResult<LinkedFhirApiDto>.Error();
        }

        var standAloneWorkspace = workspace.LinkedFhirApiId == null;
        if (standAloneWorkspace)
        {
            return OperationResult<LinkedFhirApiDto>.Succeed();
        }

        return await _linkedFhirApiService.FindByMeldRxFhirApiUrl(workspaceUrl.Data);
    }

    private async Task<OperationResult<DynamicRegistrationRequest>> LoadAppFromReturnUrl(string returnUrl)
    {
        var clientId = SofUtilities.GetParameterValueFromPartialUrl(returnUrl, JwtClaimTypes.ClientId);
        if (!clientId.Success)
        {
            return OperationResult<DynamicRegistrationRequest>.Error();
        }

        var appRegistration = await _dbContext.DynamicRegistrationRequests
            .AsNoTracking()
            .Include(x => x.Client)
            .FirstOrDefaultAsync(a => a.Client.ClientId == clientId.Data);

        return OperationResult<DynamicRegistrationRequest>.Succeed(appRegistration);
    }

    private IActionResult RedirectToLocal(string returnUrl)
    {
        if (Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return Redirect("/");
    }

    private void AddErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }

    private async Task<LoginViewModel> BuildLoginViewModelAsync(LoginInputModel model)
    {
        var vm = await BuildLoginViewModelAsync(model.ReturnUrl);
        vm.Username = model.Username;
        vm.RememberLogin = model.RememberLogin;
        return vm;
    }

    private async Task<LogoutViewModel> BuildLogoutViewModelAsync(string logoutId)
    {
        var vm = new LogoutViewModel { LogoutId = logoutId, ShowLogoutPrompt = AccountOptions.ShowLogoutPrompt };

        if (User?.Identity.IsAuthenticated != true)
        {
            // if the user is not authenticated, then just show logged out page
            vm.ShowLogoutPrompt = false;
            return vm;
        }

        var context = await _interaction.GetLogoutContextAsync(logoutId);
        if (context?.ShowSignoutPrompt == false)
        {
            // it's safe to automatically sign-out
            vm.ShowLogoutPrompt = false;
            return vm;
        }

        // show the logout prompt. this prevents attacks where the user
        // is automatically signed out by another malicious web page.
        return vm;
    }

    private async Task<LoggedOutViewModel> BuildLoggedOutViewModelAsync(string logoutId)
    {
        // get context information (client name, post logout redirect URI and iframe for federated signout)
        var logout = await _interaction.GetLogoutContextAsync(logoutId);

        var vm = new LoggedOutViewModel
        {
            AutomaticRedirectAfterSignOut = AccountOptions.AutomaticRedirectAfterSignOut,
            PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
            ClientName = string.IsNullOrEmpty(logout?.ClientName) ? logout?.ClientId : logout?.ClientName,
            SignOutIframeUrl = logout?.SignOutIFrameUrl,
            LogoutId = logoutId
        };

        if (User?.Identity.IsAuthenticated == true)
        {
            var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
            if (idp != null && idp != IdentityServerConstants.LocalIdentityProvider)
            {
                var handler = await _authenticationHandlerProvider.GetHandlerAsync(HttpContext, idp);
                if (handler is IAuthenticationSignOutHandler)
                {
                    if (vm.LogoutId == null)
                    {
                        // if there's no current logout context, we need to create one
                        // this captures necessary info from the current logged in user
                        // before we signout and redirect away to the external IdP for signout
                        vm.LogoutId = await _interaction.CreateLogoutContextAsync();
                    }

                    vm.ExternalAuthenticationScheme = idp;
                }
            }
        }

        return vm;
    }
}
