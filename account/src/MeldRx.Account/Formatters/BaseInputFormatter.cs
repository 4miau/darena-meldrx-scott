using DarenaSolutions.Fhir.Validator;
using Hl7.Fhir.Model;
using Meldh.Fhir.Core.Interfaces;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace DarenaSolutions.Bbp.Api.Formatters;

/// <summary>
/// The base input formatter that the other FHIR input formatters must inherit from.
/// This class provides common functionality for formatting between JSON and XML
/// </summary>
public abstract class BaseInputFormatter : TextInputFormatter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseInputFormatter"/> class
    /// </summary>
    /// <param name="mediaTypes">The supported media types</param>
    protected BaseInputFormatter(params string[] mediaTypes)
    {
        foreach (var mediaType in mediaTypes)
        {
            SupportedMediaTypes.Add(mediaType);
        }

        SupportedEncodings.Add(Encoding.UTF8);
    }

    protected abstract Task<Hl7.Fhir.Model.Resource> ParseStreamAsync(Stream stream, Type type);

    /// <inheritdoc />
    public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
    {
        try
        {
            Type clrResourceType = null;
            if (context.ModelType == typeof(Hl7.Fhir.Model.Resource))
            {
                var resourceTypeStr = (string)context.HttpContext.GetRouteValue(HttpConstants.ResourceTypeRouteParameter);
                if (string.IsNullOrWhiteSpace(resourceTypeStr) || !Enum.TryParse(resourceTypeStr, true, out ResourceType resourceType))
                {
                    context.ModelState.AddModelError(
                        ModelStateKeys.FhirSerializationError,
                        $"The resource type could not be recognized");

                    return await InputFormatterResult.FailureAsync();
                }

                var mappingsProvider = context.HttpContext.RequestServices.GetService<IFhirResourceMappingsProvider>();
                clrResourceType = mappingsProvider.FindFhirType(resourceType);
            }
            else
            {
                clrResourceType = context.ModelType;
            }

            var resource = await ParseStreamAsync(context.HttpContext.Request.Body, clrResourceType);
            var fhirRecordGrantsProvider = context.HttpContext.RequestServices.GetService<IFhirRecordGrantsProvider>();
            if (fhirRecordGrantsProvider.WorkspaceDto.ValidationOption == FhirServerValidationOption.Disabled)
            {
                return await InputFormatterResult.SuccessAsync(resource);
            }

            var validator = context.HttpContext.RequestServices.GetService<Firely.Fhir.Validation.Validator>();
            var outcome = validator.Validate(resource);
            var errors = outcome.FilterAppRelevantErrorIssues();

            if (errors.Count == 0)
            {
                return await InputFormatterResult.SuccessAsync(resource);
            }

            foreach (var error in errors)
            {
                context.ModelState.AddModelError(ModelStateKeys.FhirSerializationError, error.Details.Text);
            }

            return await InputFormatterResult.FailureAsync();
        }
        catch (Exception exception)
        {
            context.ModelState.AddModelError(ModelStateKeys.FhirSerializationError, exception.Message);
            return await InputFormatterResult.FailureAsync();
        }
    }

    /// <inheritdoc />
    protected override bool CanReadType(Type type)
    {
        return typeof(Hl7.Fhir.Model.Resource).IsAssignableFrom(type);
    }
}
