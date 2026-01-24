using JasperFx.Core;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Newtonsoft.Json;

namespace DarenaSolutions.Bbp.Api.Formatters;

public class MipsInputFormatter : TextInputFormatter
{
    public MipsInputFormatter()
    {
        SupportedMediaTypes.Add("json");
        SupportedMediaTypes.Add("text/json");
        SupportedMediaTypes.Add("application/json");
        SupportedEncodings.Add(Encoding.UTF8);
    }

    public static readonly Lazy<JsonSerializerSettings> JsonSettings = new(() =>
    {
        var settings = JsonSerializerSettingsProvider.CreateSerializerSettings();
        settings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
        settings.NullValueHandling = NullValueHandling.Ignore;

        return settings;
    });

    public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
    {
        var jsonStr = await context.HttpContext.Request.Body.ReadAllTextAsync();
        if (string.IsNullOrWhiteSpace(jsonStr))
        {
            return await InputFormatterResult.FailureAsync();
        }

        var parsedObject = JsonConvert.DeserializeObject(jsonStr, context.ModelType, JsonSettings.Value);

        return await InputFormatterResult.SuccessAsync(parsedObject);
    }

    public override bool CanRead(InputFormatterContext context)
    {
        return context.HttpContext.Request.Path.StartsWithSegments("/mms-api") && base.CanRead(context);
    }
}
