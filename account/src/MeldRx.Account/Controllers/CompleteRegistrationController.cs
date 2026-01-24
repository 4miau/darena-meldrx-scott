using MeldRx.Account.ViewModels.CompleteRegistration;
using MeldRx.Sdk;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace MeldRx.Account.Controllers
{
    /// <summary>
    /// Controller to manage registration from invites
    /// </summary>
    [AllowAnonymous]
    public class CompleteRegistrationController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _dbContext;

        public CompleteRegistrationController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext dbContext
        )
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Index(
            string code = null,
            [FromQuery(Name = HttpConstants.ClientIdOAuthReturnUrlQueryParameterName)] string clientId = null,
            string returnUrl = null
        )
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return BadRequest("A code must be supplied for password reset.");
            }

            var model = new CompleteRegistrationViewModel()
            {
                Code = code.GetDecodedCode(),
                ClientId = clientId,
                ReturnUrl = returnUrl,
            };

            return View(model);
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

            return View(new CompleteRegistrationStatusViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Index(
            CompleteRegistrationViewModel model,
            string code = null
        )
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(nameof(model.Email), "This email does not match the invitation.");
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!user.Active)
            {
                ModelState.AddModelError(nameof(model.Email), "The user is inactive");
            }

            if (user.ExternalIdp != null)
            {
                ModelState.AddModelError(nameof(model.Email), "Users that are associated with an external identity provider cannot reset their passwords");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);

            if (result.Succeeded)
            {
                user.IsPasswordTemporary = false;
                user.EmailConfirmed = true;

                await _userManager.UpdateAsync(user);
                await transaction.CommitAsync();

                return Redirect(string.IsNullOrEmpty(model.ReturnUrl) ? "/CompleteRegistration/Status" : model.ReturnUrl);
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(nameof(model.Email), (error.Description == "Invalid token." ? "Email does not match the invitation." : "There was an error with this invitation."));
            }

            return View(model);
        }
    }
}
