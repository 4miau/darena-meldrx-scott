using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json;

namespace DarenaSolutions.Bbp.Api.Formatters;

public class MipsOutputFormatter : TextOutputFormatter
{
    public MipsOutputFormatter()
    {
        SupportedMediaTypes.Add("json; charset=UTF-8");
        SupportedMediaTypes.Add("text/json; charset=UTF-8");
        SupportedMediaTypes.Add("application/json; charset=UTF-8");
        SupportedEncodings.Add(Encoding.UTF8);
    }

    public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
    {
        var payload = JsonConvert.SerializeObject(context.Object, MipsInputFormatter.JsonSettings.Value);
        await context.HttpContext.Response.WriteJsonAsync(payload);
    }

    public override bool CanWriteResult(OutputFormatterCanWriteContext context)
    {
        return context.HttpContext.Request.Path.StartsWithSegments("/mms-api") && base.CanWriteResult(context);
    }
}
