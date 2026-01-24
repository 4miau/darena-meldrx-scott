using Hl7.Fhir.Model;
using MeldRx.Sdk.Enums;
using Polly;
using Shouldly;
using SystemTests._Fixtures;
using SystemTests.BlueButtonPro.SystemAppTests;
using Task = System.Threading.Tasks.Task;

namespace SystemTests.Endpoints;

public class ImportCcdaEndpointsTests
{
    private readonly SystemAppFixture _systemAppFixture = new(GlobalState.Settings.FhirServerId, GlobalState.Settings.FhirServerName);

    [Fact]
    public async Task import_ccda_as_string()
    {
        // load ccda
        var ccda = await SampleFixture.GetSampleAsString("Ccdas", "AliceNewman_07809acb-9149-4b3a-9836-e5e45ae02d4d.xml");
        ccda.ShouldNotBeNullOrWhiteSpace();

        // upload ccda
        var uploadCcdaResult = await _systemAppFixture.Imports.UploadCcdaAsString(ccda, _systemAppFixture.WorkspaceId);
        uploadCcdaResult.Success.ShouldBeTrue(uploadCcdaResult.RawResponseContent);
        var documentId = uploadCcdaResult.DeserializedContent.DownloadUrl.Split('/').Last();

        // can download ccda
        var downloadDocumentStream = await _systemAppFixture.Downloads.GetCcdaDocument(_systemAppFixture.WorkspaceName, documentId);
        var document = await new StreamReader(downloadDocumentStream).ReadToEndAsync();
        document.ShouldNotBeNull();
        document.Length.ShouldBeGreaterThan(0);

        var jobId = uploadCcdaResult.DeserializedContent.Id;

        // wait for job to be running
        await Policy.Handle<ShouldAssertException>()
                    .RetryAsync(30, (e, i) => Task.Delay(TimeSpan.FromSeconds(2))) // allow 1 minute to start the job because of cold start
                    .ExecuteAsync(async () =>
                    {
                        var getBackgroundJobResult = await _systemAppFixture.BackgroundJobs.GetById(_systemAppFixture.WorkspaceName, jobId);
                        getBackgroundJobResult.Success.ShouldBeTrue(getBackgroundJobResult.RawResponseContent);
                        getBackgroundJobResult.DeserializedContent.Status.ShouldBe(BackgroundJobStatus.Running, jobId);
                    });

        // wait for job to be complete
        await Policy.Handle<ShouldAssertException>()
                    .RetryAsync(30, (e, i) => Task.Delay(TimeSpan.FromSeconds(2))) // allow 1 minute to process the job because they can run long...
                    .ExecuteAsync(async () =>
                    {
                        var getBackgroundJobResult = await _systemAppFixture.BackgroundJobs.GetById(_systemAppFixture.WorkspaceName, jobId);
                        getBackgroundJobResult.Success.ShouldBeTrue(getBackgroundJobResult.RawResponseContent);
                        getBackgroundJobResult.DeserializedContent.Status.ShouldBe(BackgroundJobStatus.Completed, jobId);
                    });

        // find patient
        var searchPatientResult = await _systemAppFixture.Fhir.Search<Patient>();
        var uploadedPatient = searchPatientResult.Data.Entry.Select(x => x.Resource as Patient).FirstOrDefault(p => p.Identifier.Single().Value == "159654");
        uploadedPatient.ShouldNotBeNull();

        // cleanup
        var deleteResult = await _systemAppFixture.Fhir.DeleteByIdAsync<Patient>(uploadedPatient.Id);
        deleteResult.Success.ShouldBeTrue();
    }
}
