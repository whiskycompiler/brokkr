using System.Text.Json;

using Brokkr.DDD.ChangeTracking;
using Brokkr.DDD.FileSystem.IntegrationTests.Utils;
using Brokkr.DDD.UoW;
using Brokkr.Location.Abstractions;
using Brokkr.Testing.XUnit.Assertions;

namespace Brokkr.DDD.FileSystem.IntegrationTests.Tests;

public sealed class SingleJsonFileSingleEntityDataSetTests : IDisposable
{
    private readonly LocalPath _filePath;
    private readonly SingleFileSettingsContext _context;

    public SingleJsonFileSingleEntityDataSetTests()
    {
        (_context, _filePath) = SingleFileSettingsContext.GetNewTestContext();
    }

    [Fact]
    public async Task AddEntity_WithNewEntity_CreatesDeserializableJsonFileOnDisk()
    {
        // arrange
        var settings = new AppSettings
        {
            Environment = "Production",
            MaxItems = 100,
        };

        // act
        _context.Settings.AddEntity(settings);
        await _context.SaveTrackedChanges(TestContext.Current.CancellationToken);

        // assert
        File.Exists(_filePath).AssertTrue();

        var json = await File.ReadAllTextAsync(_filePath, TestContext.Current.CancellationToken);
        JsonSerializer.Deserialize<AppSettings>(json).AssertNotNull();
    }

    [Fact]
    public async Task GetEntity_WithExistingEntity_ReturnsCorrectEntity()
    {
        // arrange
        var existingSettings = AddTestSettings();

        // act
        var fetchedSettings = await _context.Settings.GetEntity();

        // assert
        fetchedSettings.AssertNotNull();
        fetchedSettings.Environment.AssertEqual(existingSettings.Environment);
        fetchedSettings.MaxItems.AssertEqual(existingSettings.MaxItems);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetEntity_WithExistingEntity_TracksEntityWhenRequired(bool track)
    {
        // arrange
        var existingSettings = AddTestSettings();

        // act
        var fetchedSettings = await _context.Settings.GetEntity(track);

        // assert
        fetchedSettings.AssertNotNull();

        if (track)
        {
            var trackerEntry = _context.ChangeTracker.Entries[0];
            fetchedSettings.AssertEqual(trackerEntry.TrackedInstance);
            existingSettings.AssertEqual(trackerEntry.TrackedInstance);
            trackerEntry.State.AssertEqual(TrackingState.Unchanged);
        }
        else
        {
            _context.ChangeTracker.Entries.AssertEmpty();
        }
    }

    [Fact]
    public async Task UpdateEntity_WithExistingEntity_UpdatesCorrectly()
    {
        // arrange
        var existingSettings = AddTestSettings();
        var settingsToUpdate = await _context.Settings.GetEntity(track: true);

        // act
        var newMaxItems = settingsToUpdate!.MaxItems + 10;
        settingsToUpdate.MaxItems = newMaxItems;
        await _context.SaveTrackedChanges(TestContext.Current.CancellationToken);

        // assert
        var fetchedSettings = await _context.Settings.GetEntity();
        fetchedSettings.AssertNotNull();
        fetchedSettings.MaxItems.AssertEqual(newMaxItems);
        fetchedSettings.Environment.AssertEqual(existingSettings.Environment);

        // test file directly
        File.Exists(_filePath).AssertTrue();

        var json = await File.ReadAllTextAsync(_filePath, TestContext.Current.CancellationToken);
        var deserializedSettings = JsonSerializer.Deserialize<AppSettings>(json);
        deserializedSettings.AssertNotNull();
        deserializedSettings.AssertEqual(fetchedSettings);
    }

    [Fact]
    public async Task DeleteEntity_WithExistingEntity_RemovesEntityCompletely()
    {
        // arrange
        var existingSettings = AddTestSettings();

        // act
        _context.Settings.DeleteEntity(existingSettings);
        await _context.SaveTrackedChanges(TestContext.Current.CancellationToken);

        // assert
        File.Exists(_filePath).AssertFalse();

        var fetchedSettings = await _context.Settings.GetEntity();
        fetchedSettings.AssertNull();
    }

    [Fact]
    public async Task UpdateEntity_WhenEntityDoesNotExistAndUpsertDisabled_ThrowsException()
    {
        // arrange
        var settings = new AppSettings
        {
            Environment = "Dev",
            MaxItems = 5,
        };

        // act
        _context.SettingsStrict.UpdateEntity(settings);
        var actTask = _context.SaveTrackedChanges(TestContext.Current.CancellationToken);

        // assert
        var ex = await actTask.AssertThrows<EntityOperationException>();
        ex.FailedEntities.AssertExactLength(1);
        ex.FailedEntities.Single().Entity.AssertEqual(settings);
        ex.FailedEntities.Single().ErrorCode.AssertEqual(EntityOperationErrorCode.EntityDoesNotExist);

        File.Exists(_filePath).AssertFalse();
    }

    [Fact]
    public async Task DeleteEntity_WhenEntityDoesNotExistAndIgnoreNonexistentFilesWhenDeletingDisabled_ThrowsException()
    {
        // arrange
        var settings = new AppSettings
        {
            Environment = "Dev",
            MaxItems = 5,
        };

        // act
        _context.SettingsStrict.DeleteEntity(settings);
        var actTask = _context.SaveTrackedChanges(TestContext.Current.CancellationToken);

        // assert
        var ex = await actTask.AssertThrows<EntityOperationException>();
        ex.FailedEntities.AssertExactLength(1);
        ex.FailedEntities.Single().Entity.AssertEqual(settings);
        ex.FailedEntities.Single().ErrorCode.AssertEqual(EntityOperationErrorCode.EntityDoesNotExist);

        File.Exists(_filePath).AssertFalse();
    }

    [Fact]
    public async Task DeleteEntity_WhenEntityExistsAndIgnoreNonexistentFilesWhenDeletingDisabled_DoesNotThrow()
    {
        // arrange
        var settings = AddTestSettings();

        // act
        _context.SettingsStrict.DeleteEntity(settings);
        await _context.SaveTrackedChanges(TestContext.Current.CancellationToken);

        File.Exists(_filePath).AssertFalse();
    }

    public void Dispose()
    {
        try
        {
            var dir = Path.GetDirectoryName(_filePath);
            if (dir != null && Directory.Exists(dir))
            {
                Directory.Delete(dir, true);
            }
        }
        catch
        {
            /* ignore */
        }
    }

    private AppSettings AddTestSettings()
    {
        var testSettings = new AppSettings
        {
            Environment = $"Env {Guid.NewGuid()}",
            MaxItems = Random.Shared.Next(1, 1000),
        };

        var jsonContent = JsonSerializer.Serialize(testSettings);
        File.WriteAllText(_filePath, jsonContent);
        return testSettings;
    }
}
