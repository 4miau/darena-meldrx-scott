using System.Text.Json;

namespace SystemTests._Fixtures;

public static class GlobalState
{
    public static TestSettings Settings { get; } =
        JsonSerializer.Deserialize<TestSettings>(
            File.OpenRead("testsettings.local.json5"),
            new JsonSerializerOptions()
            {
                ReadCommentHandling = JsonCommentHandling.Skip
            }
        )
        ?? throw new Exception("failed to initialise test settings.");
}
