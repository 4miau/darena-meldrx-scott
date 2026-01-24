using Hl7.Fhir.Model;
using MeldRx.Integrations.Config;
using MeldRx.Sdk.Fhir;

namespace MeldRx.Integrations;

/// <summary>
/// Tests connecting to real FHIR servers and performing operations on them.
///
/// The list of FHIR servers to test against is specified in the EhrTestConfigs array.
/// Use this to test new EHRs. And also test MeldRx workspaces.
///
/// </summary>
public class ExternalEhrBackendTests
{
    // Make an array of EHRTestConfigs that we can use to run tests against multiple FHIR servers...
    public static IEnumerable<object[]> EhrTestConfigs = new[]
    {
        new object[] { EhrTestConfigFactory.ForSmartHealthIt() },
        new object[] { EhrTestConfigFactory.ForEpicBackendSandbox() },
        new object[] { EhrTestConfigFactory.ForCernerBackendSandbox() },
        //new object[] { EhrTestConfigFactory.ForAthenaHealthBackendSandbox() }
    };

    [Theory]
    [MemberData(nameof(EhrTestConfigs))]
    public async void ReadPatient(EhrTestConfig ehrConfig)
    {
        // Skip this test if we shouldn't do a read...
        if (!ehrConfig.PatientConfig.ShouldDoRead) { return; }

        // Perform the read...
        var fhirClient = await ehrConfig.GetFhirClient();
        var patient = await fhirClient.ReadByIdAsync<Patient>(ehrConfig.PatientConfig.FhirIdToRead);

        // Verify we found the correct result...
        Assert.NotNull(patient);
        Assert.Equal(ehrConfig.PatientConfig.FhirIdToRead, patient.Id);
    }

    [Theory]
    [MemberData(nameof(EhrTestConfigs))]
    public async void SearchPatient(EhrTestConfig ehrConfig)
    {
        // Skip this test if we shouldn't do a search...
        if (!ehrConfig.PatientConfig.ShouldDoSearch) { return; }

        // Perform the search...
        var fhirClient = await ehrConfig.GetFhirClient();
        var bundle = await fhirClient.SearchAsync<Patient>(ehrConfig.PatientConfig.SearchParams);

        // Verify we found the correct result...
        Assert.NotNull(bundle);
        Assert.Equal(ehrConfig.PatientConfig.ExpectGreaterThanZeroSearchResults, bundle.Entry.Count > 0);
    }

    [Theory]
    [MemberData(nameof(EhrTestConfigs))]
    public async void PatientOperation(EhrTestConfig ehrConfig)
    {

        var fhirClient = await ehrConfig.GetFhirClient();

        var operationResult = await fhirClient.OperationSearch(ResourceType.Patient, ehrConfig.PatientConfig.FhirIdToRead, Operations.Height);

        // The observations bundle contains the correct code for the height operation.
        Assert.NotEmpty(operationResult.Entry);
        Assert.All(operationResult.Entry.Select(x => x.Resource), (resource =>
        {
            var observation = Assert.IsType<Observation>(resource);
            Assert.Contains("8302-2", observation.Code.Coding.Select(x => x.Code));
        }));
    }

}