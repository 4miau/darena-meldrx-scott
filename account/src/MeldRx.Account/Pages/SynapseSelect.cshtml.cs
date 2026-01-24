using DarenaSolutions.Bbp.Api.Services;
using MeldRx.Account.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MeldRx.Account.Pages;

public class SynapseSelectModel : PageModel
{
    private readonly MeldRxSettings _meldRxSettings;
    private readonly ApplicationDbContext _dbContext;
    private readonly IInviteService _inviteService;

    public SynapseSelectModel(
        MeldRxSettings meldRxSettings,
        ApplicationDbContext dbContext,
        IInviteService inviteService
    )
    {
        _meldRxSettings = meldRxSettings;
        _dbContext = dbContext;
        _inviteService = inviteService;
    }

    public List<SynapseOrganizationViewModel> Organizations { get; set; }
    public string Code { get; set; }
    public string WorkspaceSlug { get; set; }
    public SynapseOrganizationViewModel CompletedOrganization { get; set; }
    public string FhirServerName { get; set; }

    public async Task OnGetAsync(string code, string organizationName, string completed, string workspaceSlug)
    {
        if (!string.IsNullOrEmpty(code))
        {
            var codeResult = await _inviteService.FindActiveInviteIdBySecurityCodeAsync(code);
            FhirServerName = codeResult.Data.FhirServerName;
            Code = code;
        }
        else
        {
            if (!string.IsNullOrEmpty(workspaceSlug))
            {
                var fhirServer = await _dbContext.FhirServers.AsNoTracking().FirstOrDefaultAsync(f => f.FhirDatabaseDisplayName == workspaceSlug);
                FhirServerName = fhirServer?.Name;
            }
            else
            {
                Organizations = [];
                return;
            }
        }


        var query = _dbContext.SynapseOrganizations
            .AsNoTracking()
            .Where(x => x.WorkspaceId == null);

        if (Guid.TryParse(completed, out var parsedId))
        {
            CompletedOrganization = await query
                .Where(x => x.Id == parsedId)
                .Select(x => new SynapseOrganizationViewModel()
                {
                    Id = x.Id,
                    OrganizationName = x.OrganizationName
                })
                .FirstOrDefaultAsync();
        }

        if (!string.IsNullOrEmpty(organizationName))
        {
            query = query
                .Where(x => x.OrganizationName.Contains(organizationName));
        }
        else
        {
            query = query.Where(x => x.IsOnCommonList == true);
        }

        Organizations = await query
            .OrderBy(x => x.OrganizationName)
            .Take(50)
            .Select(x => new SynapseOrganizationViewModel()
            {
                Id = x.Id,
                OrganizationName = x.OrganizationName
            })
            .ToListAsync();
    }

    public IActionResult OnPost(string code, string workspaceSlug, string synapseOrganizationId)
    {
        return string.IsNullOrEmpty(workspaceSlug)
            ? Redirect(
                $"{_meldRxSettings.AuthorityApiUrl}/synapse/code/{code}/synapseOrganization/{synapseOrganizationId}")
            : Redirect($"{_meldRxSettings.AuthorityApiUrl}/synapse/workspace/{workspaceSlug}/synapseOrganization/{synapseOrganizationId}");
    }
}