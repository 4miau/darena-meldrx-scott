using Hl7.Fhir.Model;
using Shouldly;
using SystemTests._Fixtures;
using SystemTests.BlueButtonPro.SystemAppTests;
using Task = System.Threading.Tasks.Task;

namespace SystemTests.Endpoints;

public class BlueButtonProFhirEndpointsTests
{
    private readonly SystemAppFixture _systemAppFixture = new(GlobalState.Settings.FhirServerId, GlobalState.Settings.FhirServerName);

    [Fact]
    public async Task cruds_operations()
    {
        var patient = new Patient()
        {
            Name = new List<HumanName> { new HumanName() { Given = new string[] { "One" }, Family = "First" } },
            BirthDate = new DateTime(2000, 01, 01).ToString("yyyy-MM-dd"),
            Gender = AdministrativeGender.Male
        };

        // create patient
        var createPatientResult = await _systemAppFixture.Fhir.Create(patient);
        createPatientResult.Success.ShouldBe(true);

        var patientId = createPatientResult.Data.Id;

        // update patient
        patient.Id = patientId;
        patient.Name = new List<HumanName> { new HumanName() { Given = new string[] { "Two" }, Family = "Second" } };
        createPatientResult = await _systemAppFixture.Fhir.Update(patient);
        createPatientResult.Success.ShouldBe(true);

        // update again
        patient.Name = new List<HumanName> { new HumanName() { Given = new string[] { "Three" }, Family = "Third" } };
        createPatientResult = await _systemAppFixture.Fhir.Update(patient);
        createPatientResult.Success.ShouldBe(true);

        // get patient
        var savedPatient = await _systemAppFixture.Fhir.GetById<Patient>(patientId);
        savedPatient.Success.ShouldBeTrue();
        savedPatient.Data.Meta.VersionId.ShouldBe("3");
        savedPatient.Data.Name.ShouldContain(name => name.Given.Single() == "Three" && name.Family == "Third");

        // get initial patient version
        savedPatient = await _systemAppFixture.Fhir.GetVersionById<Patient>(patientId, 1);
        savedPatient.Success.ShouldBeTrue();
        savedPatient.Data.Meta.VersionId.ShouldBe("1");
        savedPatient.Data.Name.ShouldContain(name => name.Given.Single() == "One" && name.Family == "First");

        // get patient version after first update
        savedPatient = await _systemAppFixture.Fhir.GetVersionById<Patient>(patientId, 2);
        savedPatient.Success.ShouldBeTrue();
        savedPatient.Data.Meta.VersionId.ShouldBe("2");
        savedPatient.Data.Name.ShouldContain(name => name.Given.Single() == "Two" && name.Family == "Second");

        // latest update not in version history
        var missingPatient = await _systemAppFixture.Fhir.GetVersionById<Patient>(patientId, 3);
        missingPatient.Success.ShouldBeFalse();

        // search patients
        var patientSearchResult = await _systemAppFixture.Fhir.Search<Patient>(criteria: new []{"given=three"});
        patientSearchResult.Success.ShouldBeTrue(patientSearchResult.ErrorMessage);
        patientSearchResult.Data.Entry.Select(x => x.Resource).Cast<Patient>().ShouldContain(p => p.Id == patientId, 1);

        // delete patient
        var deletePatientResult = await _systemAppFixture.Fhir.DeleteByIdAsync<Patient>(patientId);
        deletePatientResult.Success.ShouldBeTrue(deletePatientResult.ErrorMessage);

        // patient no longer there
        var getPatientResult = await _systemAppFixture.Fhir.GetById<Patient>(patientId);
        getPatientResult.Success.ShouldBeFalse(getPatientResult.ErrorMessage);

        // history still there.
        savedPatient = await _systemAppFixture.Fhir.GetVersionById<Patient>(patientId, 2);
        savedPatient.Success.ShouldBeTrue();
        savedPatient.Data.Meta.VersionId.ShouldBe("2");
        savedPatient.Data.Name.ShouldContain(name => name.Given.Single() == "Two" && name.Family == "Second");

        // latest update now in version history
        savedPatient = await _systemAppFixture.Fhir.GetVersionById<Patient>(patientId, 3);
        savedPatient.Success.ShouldBeTrue();
        savedPatient.Data.Meta.VersionId.ShouldBe("3");
        savedPatient.Data.Name.ShouldContain(name => name.Given.Single() == "Three" && name.Family == "Third");
    }
}
