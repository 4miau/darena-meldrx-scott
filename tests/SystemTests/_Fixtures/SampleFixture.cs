namespace SystemTests._Fixtures;

public class SampleFixture
{
    public static FileStream GetSampleAsStream(params string[] path)
    {
        return File.OpenRead(
            Path.Combine("_SampleData", Path.Combine(path))
        );
    }

    public static async Task<string> GetSampleAsString(params string[] path)
    {
        return await File.ReadAllTextAsync(
            Path.Combine("_SampleData", Path.Combine(path))
        );
    }
}
