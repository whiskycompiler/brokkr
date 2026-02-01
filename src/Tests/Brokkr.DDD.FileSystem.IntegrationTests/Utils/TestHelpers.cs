namespace Brokkr.DDD.FileSystem.IntegrationTests.Utils;

public static class TestHelpers
{
    public static string CreateTempDir()
    {
        var dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        return dir;
    }
}
