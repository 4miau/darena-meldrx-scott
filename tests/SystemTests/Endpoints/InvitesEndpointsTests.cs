using Hl7.Fhir.Model;
using MeldRx.Sdk.Dtos;
using MeldRx.Sdk.Enums;
using Shouldly;
using SystemTests._Fixtures;
using SystemTests.BlueButtonPro.SystemAppTests;
using Task = System.Threading.Tasks.Task;

namespace SystemTests.Endpoints;

public class InvitesEndpointsTests
{
    private readonly SystemAppFixture _systemAppFixture = new(GlobalState.Settings.FhirServerId, GlobalState.Settings.FhirServerName);
    private readonly AnonymousUserFixture _anonymous = new(GlobalState.Settings.FhirServerId, GlobalState.Settings.FhirServerName);

    [Fact]
    public async Task invite_patient_and_inspect_status()
    {
        var patient = new Patient()
        {
            Name = new List<HumanName>()
            {
                new HumanName().WithGiven("Tony").AndFamily("Baloney")
            },
            BirthDate = new DateTime(2000, 01, 01).ToString("yyyy-MM-dd"),
            Gender = AdministrativeGender.Male
        };

        // create patient
        var createPatientResult = await _systemAppFixture.Fhir.Create(patient);
        createPatientResult.Success.ShouldBeTrue(createPatientResult.ErrorMessage);
        var patientId = createPatientResult.Data.Id;

        // check invitation status by patient id - unlinked
        var inviteStatus = await _systemAppFixture.Invites.GetPatientLinkDetailsByPatientIdAsync(_systemAppFixture.WorkspaceId, patientId);
        inviteStatus.Success.ShouldBeTrue(inviteStatus.RawResponseContent);
        inviteStatus.DeserializedContent.Status.ShouldBe(PatientLinkStatus.Unlinked);

        // create user invitation
        var createInviteResult = await _systemAppFixture.Invites.CreateAsync(_systemAppFixture.WorkspaceId, new CreateInviteDto(patientId));
        createInviteResult.Success.ShouldBe(true);
        createInviteResult.DeserializedContent.Code.Length.ShouldBeEquivalentTo(8);

        // get invite by id
        var getInviteResult = await _systemAppFixture.Invites.FindByIdAsync(createInviteResult.DeserializedContent.Id);
        getInviteResult.Success.ShouldBeTrue(getInviteResult.RawResponseContent);

        // check invitation status by patient id - sent
        inviteStatus = await _systemAppFixture.Invites.GetPatientLinkDetailsByPatientIdAsync(_systemAppFixture.WorkspaceId, patientId);
        inviteStatus.DeserializedContent.Status.ShouldBe(PatientLinkStatus.InvitationSent);

        // list sent invites
        var sentInvites = await _systemAppFixture.Invites.FindSentAsync(_systemAppFixture.WorkspaceId);
        sentInvites.Success.ShouldBeTrue(sentInvites.RawResponseContent);
        sentInvites.DeserializedContent.Resources.ShouldContain(x => x.Id == createInviteResult.DeserializedContent.Id);

        // should be able to find the invite as anonymous
        var findAnonCode = await _anonymous.Invites.FindByCodeAnonymousAsync(createInviteResult.DeserializedContent.Code);
        findAnonCode.Success.ShouldBeTrue(findAnonCode.RawResponseContent);

        // delete invite
        var deleteResult = await _systemAppFixture.Invites.DeleteAsync(_systemAppFixture.WorkspaceId, createInviteResult.DeserializedContent.Id);
        deleteResult.Success.ShouldBeTrue(deleteResult.RawResponseContent);

        // check invitation status by patient id - unlinked
        inviteStatus = await _systemAppFixture.Invites.GetPatientLinkDetailsByPatientIdAsync(_systemAppFixture.WorkspaceId, patientId);
        inviteStatus.DeserializedContent.Status.ShouldBe(PatientLinkStatus.Unlinked);

        // shouldn't be able to find the invite as anonymous
        findAnonCode = await _anonymous.Invites.FindByCodeAnonymousAsync(createInviteResult.DeserializedContent.Code);
        findAnonCode.Success.ShouldBeFalse(findAnonCode.RawResponseContent);
    }
}
