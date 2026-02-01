using Brokkr.Location.Abstractions;

using Microsoft.Extensions.Logging.Abstractions;

namespace Brokkr.DDD.FileSystem.IntegrationTests.Utils;

public sealed class MultiFilePeopleContext : LocalFileStorageContext
{
    public JsonFileEntityDataSet<PersonEntity, Guid> People { get; }
    
    public JsonFileEntityDataSet<PersonEntity, Guid> PeopleStrict { get; }
    

    public MultiFilePeopleContext(LocalPath folder)
    {
        People = new JsonFileEntityDataSet<PersonEntity, Guid>(
            ChangeTracker,
            folder,
            NullLogger.Instance,
            allowUpsert: true,
            ignoreNonexistentFilesWhenDeleting: true);
        
        PeopleStrict = new JsonFileEntityDataSet<PersonEntity, Guid>(
            ChangeTracker,
            folder,
            NullLogger.Instance,
            allowUpsert: false,
            ignoreNonexistentFilesWhenDeleting: false);
    }
    
    public static (MultiFilePeopleContext context, LocalPath folderPath) GetNewTestContext()
    {
        var tempDir = TestHelpers.CreateTempDir();
        var folderPath = LocalPath.Create(tempDir);
        return (new MultiFilePeopleContext(folderPath), folderPath);
    }
}

public sealed class SingleFilePeopleContext : LocalFileStorageContext
{
    public SingleJsonFileEntityDataSet<PersonEntity, Guid> People { get; }
    
    public SingleJsonFileEntityDataSet<PersonEntity, Guid> PeopleStrict { get; }

    public SingleFilePeopleContext(LocalPath jsonFile)
    {
        People = new SingleJsonFileEntityDataSet<PersonEntity, Guid>(
            ChangeTracker,
            jsonFile,
            allowUpsert: true,
            ignoreNonexistentFilesWhenDeleting: true);
        
        PeopleStrict = new SingleJsonFileEntityDataSet<PersonEntity, Guid>(
            ChangeTracker,
            jsonFile,
            allowUpsert: false,
            ignoreNonexistentFilesWhenDeleting: false);
    }
    
    public static (SingleFilePeopleContext context, LocalPath filePath) GetNewTestContext()
    {
        var tempDir = TestHelpers.CreateTempDir();
        var filePath = LocalPath.Create(Path.Combine(tempDir, "people.json"));
        return (new SingleFilePeopleContext(filePath), filePath);
    }
}

public sealed class SingleFileSettingsContext : LocalFileStorageContext
{
    public SingleJsonFileSingleEntityDataSet<AppSettings> Settings { get; }
    
    public SingleJsonFileSingleEntityDataSet<AppSettings> SettingsStrict { get; }

    public SingleFileSettingsContext(LocalPath jsonFile)
    {
        Settings = new SingleJsonFileSingleEntityDataSet<AppSettings>(
            ChangeTracker,
            jsonFile,
            allowUpsert: true,
            ignoreNonexistentFilesWhenDeleting: true);

        SettingsStrict = new SingleJsonFileSingleEntityDataSet<AppSettings>(
            ChangeTracker,
            jsonFile,
            allowUpsert: false,
            ignoreNonexistentFilesWhenDeleting: false);
    }

    public static (SingleFileSettingsContext context, LocalPath filePath) GetNewTestContext()
    {
        var tempDir = TestHelpers.CreateTempDir();
        var filePath = LocalPath.Create(Path.Combine(tempDir, "settings.json"));
        return (new SingleFileSettingsContext(filePath), filePath);
    }
}
