namespace DarenaSolutions.Bbp.Api.Formatters;

/// <summary>
/// The input formatter for a JSON representation of a resource
/// </summary>
public class FhirJsonInputFormatter : BaseInputFormatter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FhirJsonInputFormatter"/> class
    /// </summary>
    public FhirJsonInputFormatter()
        : base("json", "text/json", "application/json", "application/fhir+json")
    {
    }

    protected override async Task<Hl7.Fhir.Model.Resource> ParseStreamAsync(Stream stream, Type type)
    {
        var parsedObject = await JsonSerializer.DeserializeAsync(stream, type, FhirJsonSerializerOptionsFactory.GetFhirJsonSerializerOptions());
        return parsedObject as Hl7.Fhir.Model.Resource;
    }
}
