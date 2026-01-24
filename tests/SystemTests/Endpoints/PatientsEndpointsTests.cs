using Hl7.Fhir.Model;
using Shouldly;
using SystemTests._Fixtures;
using SystemTests.BlueButtonPro.SystemAppTests;
using Task = System.Threading.Tasks.Task;

namespace SystemTests.Endpoints;

public class PatientsEndpointsTests
{
    private readonly SystemAppFixture _systemAppFixture = new(GlobalState.Settings.FhirServerId, GlobalState.Settings.FhirServerName);

    [Fact]
    public async Task invite_patient_and_inspect_status()
    {
        var patient = new Patient()
        {
            Name = new List<HumanName>()
            {
                new HumanName().WithGiven(Guid.NewGuid().ToString()).AndFamily(Guid.NewGuid().ToString())
            },
            BirthDate = new DateTime(2000, 01, 01).ToString("yyyy-MM-dd"),
            Gender = AdministrativeGender.Male
        };

        // create patient
        var createPatientResult = await _systemAppFixture.Fhir.Create(patient);
        createPatientResult.Success.ShouldBeTrue(createPatientResult.ErrorMessage);
        var patientId = createPatientResult.Data.Id;

        // no profile image to begin with
        await Assert.ThrowsAsync<HttpRequestException>(async () =>
        {
            await _systemAppFixture.Patients.GetProfilePicture(
                _systemAppFixture.WorkspaceName,
                patientId
            );
        });

        // add profile image
        var fileName = "test_image.png";
        var uploadProfilePicture = await _systemAppFixture.Patients.UploadProfilePicture(
            _systemAppFixture.WorkspaceName,
            patientId,
            new StreamContent(SampleFixture.GetSampleAsStream(fileName)),
            "image/png",
            fileName
        );
        uploadProfilePicture.Success.ShouldBeTrue(uploadProfilePicture.RawResponseContent);

        // has profile image
        var profilePicture = await _systemAppFixture.Patients.GetProfilePicture(
            _systemAppFixture.WorkspaceName,
            patientId
        );
        var ms = new MemoryStream();
        await profilePicture.CopyToAsync(ms);
        ms.Length.ShouldBeGreaterThan(0);

        // delete profile picture
        var deleteProfilePicture = await _systemAppFixture.Patients.DeleteProfilePicture(
            _systemAppFixture.WorkspaceName,
            patientId
        );
        deleteProfilePicture.Success.ShouldBeTrue(uploadProfilePicture.RawResponseContent);

        // picture no longer accessible
        await Assert.ThrowsAsync<HttpRequestException>(async () =>
        {
            await _systemAppFixture.Patients.GetProfilePicture(
                _systemAppFixture.WorkspaceName,
                patientId
            );
        });
    }
}
