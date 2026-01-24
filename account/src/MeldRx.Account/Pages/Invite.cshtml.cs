using DarenaSolutions.Bbp.Api.Services;
using MeldRx.Account.Controllers;
using MeldRx.Account.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MeldRx.Account.Pages
{
    [ValidateAntiForgeryToken]
    public class InviteModel : PageModel
    {
        private readonly MeldRxSettings _meldRxSettings;
        private readonly NotificationService _notificationService;
        private readonly SettingsService _settingsService;
        private readonly IStringLocalizer<InviteModel> _localizer;
        private readonly IInviteService _inviteService;
        private readonly IPersonService _personService;


        public InviteModel(
            IStringLocalizer<InviteModel> localizer,
            IInviteService inviteService,
            IPersonService personService,
            MeldRxSettings meldRxSettings,
            NotificationService notificationService,
            SettingsService settingsService
        )
        {
            _localizer = localizer;
            _inviteService = inviteService;
            _personService = personService;
            _meldRxSettings = meldRxSettings;
            _notificationService = notificationService;
            _settingsService = settingsService;
        }

        public string DefaultImage { get; set; }
        public PersonDto SelectedPersonDto { get; set; }
        public bool IsCmsSharingInvite { get; set; }
        public bool IsBusy { get; set; }
        public string Logo { get; set; }
        public string Url { get; set; }
        [BindProperty]
        public string Code { get; set; }
        public InviteAnonymousDetailsDto InviteAnonymousDetailsDto { get; set; } = new();
        public InviteAcceptResultDto? InviteAcceptResultDto { get; set; }

        public readonly string[] Relationships = Enum.GetNames(typeof(UserPersonRelationshipType));
        [BindProperty]
        public List<ExistingPersonWithRelationship> ExistingPeople { get; set; } = new();
        [BindProperty]
        public InviteAcceptBySecurityDetailsDto InviteAcceptBySecurityDetailsDto { get; set; } = new();
        [BindProperty]
        public bool HasExistingRelationships { get; set; }
        [BindProperty]
        public PersonTypes PersonType { get; set; } = PersonTypes.NewPerson;
        [BindProperty]
        public string? SelectedExistingPersonId { get; set; }

        public async Task<IActionResult> OnPost(string actionString, string parameter)
        {
            var action = Enum.Parse<InviteAction>(actionString);
            var returnUrl = $"/invite/code/{parameter}";
            switch (action)
            {
                case InviteAction.Login:
                    return Challenge(new AuthenticationProperties { RedirectUri = returnUrl });
                case InviteAction.Register:
                    return RedirectToAction(nameof(AccountController.Register), "Account",
                        new { returnUrl = returnUrl });
                case InviteAction.AcceptInvite:

                    var result = await OnAcceptInvite(parameter);
                    if (result)
                    {
                        return Redirect(_meldRxSettings.AuthorityUrl);
                    }

                    await PopulateExistingRelationships();
                    return Page();
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }
        }

        //The get method is configured to accept both workspace slug and code in the options in AddRazorPages. It can accept https://xx.yy.com/invite/abc or https://xx.yy.com/workspace/WorkspaceSlug
        public async Task<IActionResult> OnGet(string type, string parameter, string returnUrl = null, string completed = null)
        {

            var code = String.Empty;
            var workspaceSlug = string.Empty;

            switch (type)
            {
                case "code" when !string.IsNullOrEmpty(parameter):
                    code = parameter;
                    break;
                case "workspace" when !string.IsNullOrEmpty(parameter):
                    workspaceSlug = parameter;
                    break;
            }

            if (string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(workspaceSlug))
            {
                return Redirect($"{_meldRxSettings.AuthorityUrl}/SynapseSelect?workspaceSlug={workspaceSlug}&completed={completed}");
            }

            var result = await _inviteService.FindActiveInviteIdBySecurityCodeWithSettingsAsync(code);

            if (result.Success)
            {
                //If it is a synapse invite, we will redirect them directly. Currently, we are only doing CMS invite. Once we add others,
                //this will redirect to a page to allow them to select a particular provider
                if (result.Data.IsSynapseOnly)
                {
                    return Redirect($"{_meldRxSettings.AuthorityUrl}/SynapseSelect?code={code}&completed={completed}");
                }

                InviteAnonymousDetailsDto = result.Data;
                if (InviteAnonymousDetailsDto.HasCustomLogo)
                {
                    Url = $"{_meldRxSettings.AuthorityApiUrl}/settings/{InviteAnonymousDetailsDto.FhirServerName}/logo";
                    await LoadImage(InviteAnonymousDetailsDto.HasCustomLogo);
                }
            }
            else
            {
                IsBusy = false;
                //We are assuming here that the user has probably accepted the invite and therefore a bad request is returned.
                // As almost everyone will coming to this page through a link (Not be entering a code on home page), it is a fair assumption
                //We should/may change this in future for the API to return different status for invalid code vs accepted code.

                Response.Redirect(_meldRxSettings.AuthorityUrl);
            }

            if (HttpContext.User.IsAuthenticated())
            {
                await PopulateExistingRelationships();
            }

            return Page();
        }

        private async Task<bool> PopulateExistingRelationships()
        {
            var peopleClientHttpResponseModel = await _personService.FindAllForLoggedInUserAsync();
            if (peopleClientHttpResponseModel.Count > 0)
            {
                ExistingPeople = peopleClientHttpResponseModel.Select(e =>
                    new ExistingPersonWithRelationship
                    {
                        DisplayName = $"{e.Person.FirstName} {e.Person.LastName}",
                        PersonId = e.PersonId,
                        Relationship = e.Relationship.ToString(),
                    }).ToList();
                HasExistingRelationships = true;
            }

            return true;
        }

        protected Task OnRegister()
        {
            return Task.CompletedTask;
            //await LoginService.Register(returnUrl: RedirectUrl);
        }

        protected void GenerateNotifications()
        {
            if (!TempData.ContainsKey(NotificationHelpers.NotificationKey)) return;
            ViewData["Notifications"] = TempData[NotificationHelpers.NotificationKey];
            TempData.Remove(NotificationHelpers.NotificationKey);
        }

        protected async Task<bool> OnAcceptInvite(string code)
        {
            InviteAcceptBySecurityDetailsDto.InvitationCode = code;
            if (!string.IsNullOrEmpty(SelectedExistingPersonId))
            {
                //TODO:Validate that it is not an existing patient in the same FHIR db.
                InviteAcceptBySecurityDetailsDto.ExistingPersonId = Guid.Parse(SelectedExistingPersonId);
            }

            var httpResponseModel = await _inviteService.AcceptAsync(InviteAcceptBySecurityDetailsDto);
            return httpResponseModel.ProcessOperationResult(this.TempData, _notificationService,
                _localizer["SuccessInviteAccepted"],
                title: httpResponseModel.Success ? _localizer["SuccessTitle"] : _localizer["ErrorTitle"]);
        }

        public override void OnPageHandlerExecuted(PageHandlerExecutedContext context)
        {
            GenerateNotifications();
            base.OnPageHandlerExecuted(context);
        }

        protected async Task LoadImage(bool hasCustomLogo)
        {
            try
            {
                if (hasCustomLogo)
                {
                    // the _fhirRecordGrantsProvider required context is being set by the invite service in the calling method, sorry.
                    var imageStream = await _settingsService.GetLogo();
                    if (imageStream != null)
                    {
                        var ms = new MemoryStream();
                        imageStream.CopyTo(ms);
                        Logo = $"data:image/png;base64,{Convert.ToBase64String(ms.ToArray())}";
                    }
                    else
                    {
                        await GetDefaultImage();
                    }
                }
                else
                {
                    await GetDefaultImage();
                }
            }
            catch (Exception e)
            {
            }
        }

        private Task GetDefaultImage()
        {
            Logo = Assets.DefaultImage;
            return Task.CompletedTask;
        }
    }

    public enum InviteAction
    {
        Login,
        Register,
        AcceptInvite
    }
}
