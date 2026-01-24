using Hl7.Fhir.Model;
using Shouldly;
using SystemTests._Fixtures;
using SystemTests.BlueButtonPro.SystemAppTests;
using Task = System.Threading.Tasks.Task;

namespace SystemTests.Endpoints;

public class DocumentsEndpointsTest
{
    private readonly SystemAppFixture _systemAppFixture = new(GlobalState.Settings.FhirServerId, GlobalState.Settings.FhirServerName);

    [Fact]
    public async Task attach_document_to_patient()
    {
        var patient = new Patient()
        {
            Name = new List<HumanName> { new HumanName() { Given = new string[] { Guid.NewGuid().ToString() }, Family = Guid.NewGuid().ToString() } },
            BirthDate = new DateTime(2000, 01, 01).ToString("yyyy-MM-dd"),
            Gender = AdministrativeGender.Male
        };

        // create patient
        var createPatientResult = await _systemAppFixture.Fhir.Create(patient);
        createPatientResult.Success.ShouldBe(true);
        var patientId = createPatientResult.Data.Id;

        var form = new MultipartFormDataContent();
        form.Add(
            new StringContent(patientId),
            "PatientId"
        );

        form.Add(
            new StringContent("file content"),
            "files",
            $"{Guid.NewGuid()}.txt"
        );

        var documentUploadResult = await _systemAppFixture.Documents.UploadDocumentAsReference(
            form,
            _systemAppFixture.WorkspaceName
        );
        documentUploadResult.Success.ShouldBeTrue(documentUploadResult.RawResponseContent);

        var getDocumentResult = await _systemAppFixture.Fhir.GetById<DocumentReference>(documentUploadResult.DeserializedContent.Id);
        getDocumentResult.Success.ShouldBeTrue();
    }
}
