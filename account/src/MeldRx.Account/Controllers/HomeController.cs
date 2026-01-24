using DarenaSolutions.Bbp.Api.Extensions;
using MeldRx.Account.ViewModels.Home;
using Microsoft.AspNetCore.Localization;

namespace MeldRx.Account.Controllers;

[SecurityHeaders]
[ApiExplorerSettings(IgnoreApi = true)]
public class HomeController : Controller
{
    private readonly IIdentityServerInteractionService _interaction;

    public HomeController(IIdentityServerInteractionService interaction)
    {
        _interaction = interaction;
    }

    /// <summary>
    /// Shows the error page
    /// </summary>
    public async Task<IActionResult> Error(string errorId)
    {
        var vm = new ErrorViewModel();

        // retrieve error details from identityserver
        var message = await _interaction.GetErrorContextAsync(errorId);
        if (message != null)
        {
            vm.Error = message;
        }

        return View("Error", vm);
    }
}
