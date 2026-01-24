using DarenaSolutions.Bbp.Api.Extensions;
using MeldRx.Account.Helpers;
using MeldRx.Account.Services;
using MeldRx.Account.Services.AadGraphServices;
using MeldRx.Account.ViewModels.Account;
using MeldRx.Sdk.Internal.Extensions;
using MeldRx.Sdk.Internal.Services;

namespace MeldRx.Account.Controllers;

/// <summary>
/// Controller to manage all Azure Active Directory Registration
/// </summary>
[SecurityHeaders]
[Authorize]
public class AadController : BaseController
{
    private readonly OrganizationService _organizationService;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IEmailSender _emailSender;
    private readonly IAadGraphApiClient _graphClient;
    private readonly IAccountManager _accountManager;
    private readonly NotificationService _notificationService;
    private readonly IEmailContentCreator _emailContentCreator;
    private readonly ApplicationDbContext _dbContext;
    private readonly MeldRxSettings _meldRxSettings;

    private const string NotInAadAutoActivationGroupMessage =
        "You do not belong to any of the groups assigned to this organization that allows you to login with " +
        "Darena Solutions. Please contact your company administrator to add you to one of those groups.";

    public AadController(
           SignInManager<ApplicationUser> signInManager,
           IEmailSender emailSender,
           IAadGraphApiClient graphClient,
           OrganizationService organizationService,
           IAccountManager accountManager,
           NotificationService notificationService,
           IEmailContentCreator emailContentCreator,
           ApplicationDbContext dbContext,
           MeldRxSettings meldRxSettings,
           ILogger<AadController> logger
           ) : base(logger)
    {
        _organizationService = organizationService;
        _signInManager = signInManager;
        _emailSender = emailSender;
        _graphClient = graphClient;
        _accountManager = accountManager;
        _notificationService = notificationService;
        _emailContentCreator = emailContentCreator;
        _dbContext = dbContext;
        _meldRxSettings = meldRxSettings;
    }

    [HttpPost]
    [HttpGet]
    [AllowAnonymous]
    public IActionResult AadLogin( string returnUrl = null)
    {

        // Request a redirect to the external login provider.
        var redirectUrl = Url.Action(nameof(AadCallback), new { ReturnUrl = returnUrl });
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(ExternalProviders.AzureActiveDirectory, redirectUrl);

        return Challenge(properties, ExternalProviders.AzureActiveDirectory);
    }

    [HttpGet]
    [Authorize(AuthenticationSchemes = ExternalProviders.AzureActiveDirectory)]
    public async Task<IActionResult> AadCallback(string returnUrl = null, string remoteError = null)
    {
        var tenantId = HttpContext.GetClaimValue(AuthConstants.TenantIdClaimType);
        if (string.IsNullOrEmpty(tenantId))
        {
            PopupNotificationMessage = $"Unable to obtain Tenant Id";
            _notificationService.CreateNotification(TempData,NotificationHelpers.AlertType.Error,PopupNotificationMessage);
            return RedirectToAction(nameof(AccountController.Login),"Account");
        }

        var emailAddress = await _graphClient.GetEmailAddressAsync();
        var routeValues = string.IsNullOrWhiteSpace(returnUrl) ? null : new { returnUrl };

        var orgFindResult = await _organizationService.AdminSearchScopeFindByAadTenantIdAsync(tenantId);
        if (!orgFindResult.Success && !orgFindResult.NotFound)
        {
            PopupNotificationMessage = $"An unexpected error occurred: {orgFindResult.ErrorMessage}";
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        // Determine if their organization has already been approved and they are just a new user.
        if (orgFindResult.NotFound)
        {
            // If the organization could not be found (not approved yet). Determine if this user is a global administrator.
            // If so, redirect them to the organization request screen. Otherwise, return an error
            //if (!await _graphClient.IsGlobalAdministratorAsync())
            //{
            //    PopupNotificationMessage =
            //        "You cannot register your organization with Darena Solutions because you are not a global administrator. " +
            //        "Contact your administrator to register your organization with Darena Solutions.";
            //    return RedirectToAction(nameof(AccountController.Login), "Account");
            //}

            return RedirectToAction(nameof(AzureActiveDirectoryRegistration), routeValues);
        }
        //else
        //{
        //    return RedirectToAction(nameof(AccountController.Login), "Account", routeValues);
        //}
        //If the organization was found, determine if the user belongs in one of the auto activation groups. If
        //they do, register the user and allow them to login.
        if (!await _graphClient.IsInGroupAnyAsync(orgFindResult.Data.AadAutoActiveUserGroups))
        {
            PopupNotificationMessage = NotInAadAutoActivationGroupMessage;
            return RedirectToAction(nameof(AccountController.Login), "Account", routeValues);
        }



        return await SignInAndRedirectAsync(emailAddress, returnUrl);

    }

    [Authorize(AuthenticationSchemes = ExternalProviders.AzureActiveDirectory)]
    [HttpPost]
    public async Task<IActionResult> AzureActiveDirectoryRegistration([FromForm] AzureActiveDirectoryRegistrationViewModel vm)
    {
        var applicationUser = await _graphClient.GetCreateApplicationUserModelAsync();
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        var tuple = await _accountManager.RegisterAsync(
            applicationUser,
            hasNoPassword: true,
            emailConfirmed: true,
            externalIdp: ExternalIdp.AzureActiveDirectory);

        if (!tuple.result.Succeeded)
        {
            foreach (var error in tuple.result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(vm);
        }

        await transaction.CommitAsync();
        // _logger.Information(new EmailConfirmationEvent(ApplicationUser.Email, EmailConfirmationStrategy.ExternalIdpLogin));

        ///*************ONLY FOR STAGING***********Disable in Production
        var message = await _emailContentCreator.CreatePlainTextContentAsync(
            $"A new organization request has arrived. The details are listed below:<br /><br />" +
            $"Organization name: {vm.AzureAdBasedOrganization.Name}<br />" +
            $"User first name: {applicationUser.FirstName}<br />" +
            $"User last name: {applicationUser.LastName}<br />" +
            $"User email address: {applicationUser.Email}<br />" +
            $"Tenant id: {vm.TenantId}");

        // Email in the background
        var unused = _emailSender.SendEmailAsync(
            _meldRxSettings.InternalNotificationsEmailAddress,
            $"New Organization Request",
            message);

        PopupNotificationMessage =
            "A request has been created for your organization. A member of Darena Solutions will review " +
            "the request and notify you shortly. Once the request is approved, you can begin logging in " +
            "with your Azure Active Directory account. Any members in your organization that are in your " +
            "specified list of groups will be approved automatically and can also login.";

        //return await ExternalLoginConfirmation(new ExternalLoginConfirmationViewModel
        //{
        //    Email = vm.ApplicationUser.Email,

        //}, vm.ReturnUrl);
        return await SignInAndRedirectAsync( applicationUser.Email,vm.ReturnUrl);
        return Redirect(vm.ReturnUrl);
        return RedirectToAction(nameof(AccountController.Login),"Account");
    }

    [Authorize(AuthenticationSchemes = ExternalProviders.AzureActiveDirectory)]
    [HttpGet]
    public async Task<IActionResult> AzureActiveDirectoryRegistration(string returnUrl = null)
    {

        var user = await _graphClient.GetCreateApplicationUserModelAsync();
        if (string.IsNullOrWhiteSpace(user.FirstName) ||
            string.IsNullOrWhiteSpace(user.LastName))
        {
            PopupNotificationMessage =
                "We could not find a first name or last name. Ensure this information is applied for your " +
                "Azure Active Directory account";

            var routeValues = string.IsNullOrWhiteSpace(returnUrl) ? null : new { returnUrl };
            return RedirectToAction(nameof(AccountController.Login), "Account", routeValues);
        }
        var vm = new AzureActiveDirectoryRegistrationViewModel
        {
            ReturnUrl = string.IsNullOrEmpty(returnUrl) ? "https://www.darenasolutions.com" : returnUrl,
            ApplicationUser = user,
            AzureAdBasedOrganization = await _graphClient.GetCreateOrganizationRequestModelAsync(),
            TenantId = HttpContext.GetClaimValue(AuthConstants.TenantIdClaimType),
            ObjectId = HttpContext.GetClaimValue(AuthConstants.ObjectIdClaimType)
        };
        vm.AzureAdBasedOrganization.Email = user.Email;
        vm.AzureAdBasedOrganization.PrimaryContactFirstName = user.FirstName;
        vm.AzureAdBasedOrganization.PrimaryContactLastName=user.LastName;


        return View(vm);
    }

    private async Task<IActionResult> SignInAndRedirectAsync( string emailAddress, string returnUrl)
    {
        var tenantId = HttpContext.GetClaimValue(AuthConstants.TenantIdClaimType);
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
        var user = new ApplicationUser
        {
            Email = emailAddress,
            Active = true,
            UserName = emailAddress,
        };

        await _signInManager.SignInWithClaimsAsync(user, new AuthenticationProperties
        {

        }, new Claim[]
        {
                new Claim(AuthConstants.TenantIdClaimType,tenantId),
                new Claim(ClaimTypes.Email,emailAddress),
                new Claim(ClaimTypes.NameIdentifier,emailAddress),
            new Claim(ClaimTypes.AuthenticationMethod, ExternalProviders.AzureActiveDirectory)
        });

        if (!string.IsNullOrWhiteSpace(returnUrl) && returnUrl != "/")
        {
            return LocalRedirect(returnUrl);
        }

        var identityManagerUrl = "/";// _settings.GetIdentityManagerUserEditorUrl(user.Id);
                                     //return $"{IdentityManagerReactPostLogoutRedirectUrl}?action=modifyUser&id={userId}";
        return Redirect(identityManagerUrl);

    }




}
