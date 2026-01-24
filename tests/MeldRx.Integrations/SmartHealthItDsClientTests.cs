using Hl7.Fhir.Model;
using Meldh.Fhir.Core.Models.Search;
using MeldRx.Sdk.Fhir.Client;
using Moq;

namespace MeldRx.Integrations;

/// <summary>
/// Tests the DsFhirClient by making requests to a real FHIR server
/// </summary>
public class SmartHealthItDsClientTests
{
    private readonly Mock<IHttpMessageHandlerFactory> _httpHandlerFactoryMock;

    public SmartHealthItDsClientTests()
    {
        _httpHandlerFactoryMock = new Mock<IHttpMessageHandlerFactory>();
        _httpHandlerFactoryMock.Setup(x => x.CreateHandler(It.IsAny<string>()))
            .Returns(new HttpClientHandler());
    }
    
    /// <summary>
    /// Create a patient FHIR object that we can use when testing
    /// </summary>
    public static Patient GetPatient(string firstName, string lastName, DateTime birthDate, AdministrativeGender gender)
    {
        return new Patient()
        {
            Name = new List<HumanName>()
            {
                new HumanName().WithGiven(firstName).AndFamily(lastName)
            },
            BirthDate = birthDate.ToString("yyyy-MM-dd"),
            Gender = gender
        };
    }

    [Fact]
    public async void SearchAsync_SearchPatientByName_FindsResults()
    {
        var factory = new DsFhirClientFactory(_httpHandlerFactoryMock.Object);
        var fhirClient = factory.ForNoAuth("https://launch.smarthealthit.org/v/r4/fhir");
        
        //  Perform the search...
        var bundle = await fhirClient.SearchAsync<Patient>(new List<FhirQueryParameter>() { new FhirQueryParameter("given", "gerardo") });
        var entries = bundle.Entry;
        var firstEntry = entries.FirstOrDefault().Resource as Patient;

        // Verify we found the correct result...
        Assert.Equal(1, entries.Count); // Should only have one result
        Assert.Equal("8de3051f-6298-43e6-9b7f-2aa6443ee760", firstEntry!.Id);
        Assert.Equal("2010-09-07", firstEntry.BirthDate);
        Assert.Equal("Botello", firstEntry.Name[0].Family);
    }
    
    [Fact]
    public async void ReadAsync_CanReadPatientById_FindsPatient()
    {
        var factory = new DsFhirClientFactory(_httpHandlerFactoryMock.Object);
        var fhirClient = factory.ForNoAuth("https://launch.smarthealthit.org/v/r4/fhir");

        // Read a patient by ID...
        const string patientIdGerardoBotello = "8de3051f-6298-43e6-9b7f-2aa6443ee760";
        var readResult = await fhirClient.ReadByIdAsync<Patient>(patientIdGerardoBotello);
        Assert.Equal(patientIdGerardoBotello, readResult?.Id);
    }
    
    [Fact]
    public async void SearchAsync_SearchPatientResources_FindsResults()
    {
        var factory = new DsFhirClientFactory(_httpHandlerFactoryMock.Object);
        var fhirClient = factory.ForNoAuth("https://launch.smarthealthit.org/v/r4/fhir");
        
        //  Search for patient resources...
        var patientIdGerardoBotello = "8de3051f-6298-43e6-9b7f-2aa6443ee760";
        var allergyBundle = await fhirClient.SearchAsync<AllergyIntolerance>(new List<FhirQueryParameter>() { new FhirQueryParameter("patient", patientIdGerardoBotello) });
        var conditionBundle = await fhirClient.SearchAsync<Condition>(new List<FhirQueryParameter>() { new FhirQueryParameter("patient", patientIdGerardoBotello) });
        var immunizationBundle = await fhirClient.SearchAsync<Immunization>(new List<FhirQueryParameter>() { new FhirQueryParameter("patient", patientIdGerardoBotello) });
        
        // Verify we got the correct results...
        Assert.Equal(1, allergyBundle.Entry.Count);
        Assert.Equal(6, conditionBundle.Entry.Count);
        Assert.Equal(27, immunizationBundle.Entry.Count);
    }

    [Fact]
    public async void Create_CreateReadUpdateDeletePatient_Succeeds()
    {
        var factory = new DsFhirClientFactory(_httpHandlerFactoryMock.Object);
        var fhirClient = factory.ForNoAuth("https://launch.smarthealthit.org/v/r4/fhir");

        // Initialize a patient object...
        var firstName = "Rusty";
        var lastName = "Shackleford";
        var birthDate = new DateTime(1980, 1, 1);
        var gender = AdministrativeGender.Male;
        var patient = GetPatient(firstName, lastName, birthDate, gender);

        // Create the patient and verify the created patient's data matches the object we created...
        var createdPatient = await fhirClient.CreateAsync(patient);
        Assert.Equal(createdPatient?.Name[0].Given.FirstOrDefault(), firstName);
        Assert.Equal(createdPatient?.Name[0].Family, lastName);
        Assert.Equal(createdPatient?.BirthDate, birthDate.ToString("yyyy-MM-dd"));
        Assert.Equal(createdPatient?.Gender, gender);

        // Read the patient back and verify the data matches...
        var readPatient = await fhirClient.ReadByIdAsync<Patient>(createdPatient.Id);
        Assert.Equal(firstName, readPatient?.Name[0].Given.FirstOrDefault());
        Assert.Equal(lastName, readPatient?.Name[0].Family);
        Assert.Equal(birthDate.ToString("yyyy-MM-dd"), readPatient?.BirthDate);
        Assert.Equal(gender, createdPatient?.Gender);

        // Update the patient's name, then read it back and verify the name is updated...
        readPatient!.Name[0].Family = "NewFamilyName";
        var updatedPatient = await fhirClient.UpdateAsync(readPatient);
        Assert.Equal("NewFamilyName", updatedPatient?.Name[0].Family);

        // Read the patient back and verify the data matches...
        readPatient = await fhirClient.ReadByIdAsync<Patient>(createdPatient.Id);
        Assert.Equal(firstName, readPatient?.Name[0].Given.FirstOrDefault());
        Assert.Equal("NewFamilyName", readPatient?.Name[0].Family);
        Assert.Equal(birthDate.ToString("yyyy-MM-dd"), readPatient?.BirthDate);
        Assert.Equal(gender, createdPatient?.Gender);

        // Delete the patient...
        await fhirClient.DeleteByIdAsync<Patient>(readPatient.Id);

        // TODO: This keeps getting "version 2" of the resource back, which is not what we want...
        // Read the patient back and verify it's gone...
        //readPatient = await externalFhirClient.ReadById<Patient>(readPatient.Id);
        //Assert.Null(readPatient);
    }
}