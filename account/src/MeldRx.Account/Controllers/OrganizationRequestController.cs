using MeldRx.Account.Extensions;
using MeldRx.Account.ViewModels.OrganizationRequest;
using MeldRx.Sdk.Dtos.Organization;
using MeldRx.Sdk.Internal.Constants;
using MeldRx.Services.Shared.Resources;
using MeldRx.Services.Shared.Services.Organization;
using MeldRx.Services.Shared.Services.WorkspaceUsage;
using Organization = MeldRx.Sdk.Internal.Database.Resources.Organization;

namespace MeldRx.Account.Controllers;

/// <summary>
/// Controller to manage organization registration
/// </summary>
[Authorize]
[SecurityHeaders]
public class OrganizationRequestController : BaseController
{
    private readonly IAccountManager _accountManager;
    private readonly ApplicationDbContext _dbContext;
    private readonly MeldRxSettings _meldRxSettings;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public OrganizationRequestController(
        IAccountManager accountManager,
        ApplicationDbContext dbContext,
        MeldRxSettings meldRxSettings,
        UserManager<ApplicationUser> userManager,
        IWebHostEnvironment webHostEnvironment,
        ILogger<OrganizationRequestController> logger
    ) : base(logger)
    {
        _accountManager = accountManager;
        _dbContext = dbContext;
        _meldRxSettings = meldRxSettings;
        _userManager = userManager;
        _webHostEnvironment = webHostEnvironment;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Index(string returnUrl = null)
    {
        if (_webHostEnvironment.IsDevelopment())
        {
            var model = new OrganizationRequestViewModel
            {
                Email = $"{Guid.NewGuid().ToString()}@meldrx.com",
                OrganizationName = "autoorg",
                OrganizationIdentifier = Guid.NewGuid().ToString(),
                Password = "P@ssw0rd123",
                ConfirmPassword = "P@ssw0rd123"
            };
            return View(model);
        }

        return View(new OrganizationRequestViewModel());
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Status(string returnUrl = null)
    {
        if (User.IsAuthenticated())
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return Redirect("/");
            }
        }

        return View(new OrganizationRequestStatusViewModel());
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    [ValidateReCaptcha]
    public async Task<IActionResult> Index(
        OrganizationRequestViewModel model,
        [FromServices] CreateOrganization createOrganization,
        [FromServices] OrganizationService organizationService,
        string returnUrl = null
    )
    {
        //This will only be set to true on staging for quick testing
        var autoApprove = _meldRxSettings.AutoApproveOrganizationRequest;
        if (autoApprove)
        {
            ModelState.Remove("Recaptcha");
        }

        returnUrl ??= Url.Content("~/");
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }


        if (string.IsNullOrEmpty(model.OrganizationIdentifier))
        {
            model.OrganizationIdentifier = Guid.NewGuid().ToString();
        }


        //Check for Org Identifier
        var existing = await organizationService.FindByTinAsync(model.OrganizationIdentifier);
        if (!existing.NotFound)
        {
            ModelState.AddModelError(nameof(model.OrganizationIdentifier), "There is already an account with this organization identifier.");
            return View(model);
        }

        //Check for User
        var existingUser = await _userManager.FindByEmailAsync(model.Email);
        if (existingUser != null)
        {
            if (existingUser.Active)
            {
                ModelState.AddModelError(nameof(model.Email), "There was a problem with this email. Please try again.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "You have a pending organization request.");
            }

            return View(model);
        }

        var applicationUser = new CreateApplicationUserDto
        {
            Email = model.Email,
            Password = model.Password,
            ConfirmPassword = model.ConfirmPassword,
            Active = false,
            FirstName = model.FirstName,
            LastName = model.LastName,
        };

        await using var transaction = await _dbContext.Database.BeginTransactionAsync();

        //if auto approve is on, we set the email to confirmed
        var tuple = await _accountManager.RegisterAsync(
            applicationUser,
            emailConfirmed: autoApprove, useCase: EmailUseCases.DeveloperAccountRegistration);

        if (!tuple.result.Succeeded)
        {
            foreach (var error in tuple.result.Errors)
            {
                ModelState.AddModelError(nameof(model.Password), error.Description);
            }

            return View(model);
        }

        var organization = new Organization
        {
            Name = model.OrganizationName,
            Tin = model.OrganizationIdentifier,
            Type = OrganizationType.Developer,
            Subscriptions =
            [
                new Subscription
                {
                    Active = true,
                    BillingFrequency = BillingFrequency.Monthly,
                    AllowOverage = false,
                    IncludedWorkspaces = _meldRxSettings.DeveloperSubscriptionSettings.IncludedWorkspaces,
                    IncludedVirtualWorkspaces = _meldRxSettings.DeveloperSubscriptionSettings.IncludedVirtualWorkspaces,
                    IncludedApiCalls = _meldRxSettings.DeveloperSubscriptionSettings.IncludedApiCalls,
                    IncludedDataStorage = _meldRxSettings.DeveloperSubscriptionSettings.IncludedDataStorageInMiB.GetBytesFromMiB()
                },
            ],
        };
        var createOrgResult = await createOrganization.CreateAsync(organization);
        if (!createOrgResult.Success)
        {
            ModelState.AddModelError(string.Empty, createOrgResult.ErrorMessage);
            return View();
        }

        var user = tuple.user;
        user.Active = true;
        user.OrganizationUserRelations =
        [
            new OrganizationUserRelation
            {
                OrganizationId = createOrgResult.Data.Id,
                OrganizationRole = OrganizationRoles.Admin
            },
        ];

        await _userManager.UpdateAsync(user);
        await transaction.CommitAsync();
        return RedirectToAction("Status");
    }
}
