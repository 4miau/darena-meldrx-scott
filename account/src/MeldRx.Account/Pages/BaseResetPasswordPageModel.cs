using System.Diagnostics.CodeAnalysis;
using MeldRx.Account.Services;
using MeldRx.Sdk;
using Microsoft.AspNetCore.Mvc.RazorPages;

#pragma warning disable 1591
#pragma warning disable SA1649

namespace MeldRx.Account.Pages
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Documentation not required on views")]
    public class BaseResetPasswordPageModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IClientContextHelper _clientContextHelper;
        private readonly ApplicationDbContext _dbContext;

        protected BaseResetPasswordPageModel(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext dbContext,
            IClientContextHelper clientContextHelper)
        {
            _userManager = userManager;
            _clientContextHelper = clientContextHelper;
            _dbContext = dbContext;
        }

        public string ClientId { get; set; }

        public string ReturnUrl { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        protected IActionResult OnGetBase(string code, string clientId, string returnUrl)
        {
            ClientId = clientId;
            ReturnUrl = returnUrl;
            if (string.IsNullOrWhiteSpace(code))
            {
                return BadRequest("A code must be supplied for password reset.");
            }

            Input = new InputModel
            {
                Code = code
            };

            return Page();
        }

        protected async Task<IActionResult> OnPostBaseAsync(string clientId, string returnUrl)
        {
            ClientId = clientId;
            ReturnUrl = returnUrl;
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Dictionary<string, string> routeValues = null;
            if (!string.IsNullOrWhiteSpace(clientId))
            {
                await _clientContextHelper.CheckAndSetClientContextAsync();
                routeValues = new Dictionary<string, string>
                {
                    { HttpConstants.ClientIdOAuthReturnUrlQueryParameterName, clientId }
                };
            }

            routeValues ??= new Dictionary<string, string>();
            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                returnUrl = _clientContextHelper.GetReturnUrl();
            }
            routeValues.Add("returnUrl", returnUrl);
            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToPage("./ResetPasswordConfirmation", routeValues);
            }

            if (!user.Active)
            {
                ModelState.AddModelError(string.Empty, "The user is inactive");
            }

            if (user.ExternalIdp != null)
            {
                ModelState.AddModelError(string.Empty, "Users that are associated with an external identity provider cannot reset their passwords");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            var result = await _userManager.ResetPasswordAsync(user, Input.Code, Input.Password);
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
                await transaction.CommitAsync();

                // Log email confirmation
                if (emailWasNotConfirmed)
                {
                    //_logger.Information(new EmailConfirmationEvent(user.Email, EmailConfirmationStrategy.PasswordReset));
                }

                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToPage("./ResetPasswordConfirmation", routeValues);
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage =  ModelValidationMessages.PasswordLength, MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = ModelValidationMessages.PasswordMatch)]
            public string ConfirmPassword { get; set; }

            public string Code { get; set; }
        }
    }
}
