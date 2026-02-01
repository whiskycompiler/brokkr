using System.Text.Json;

using Brokkr.DDD.ChangeTracking;
using Brokkr.DDD.FileSystem.IntegrationTests.Utils;
using Brokkr.DDD.UoW;
using Brokkr.Location.Abstractions;
using Brokkr.Testing.XUnit.Assertions;

namespace Brokkr.DDD.FileSystem.IntegrationTests.Tests;

public sealed class SingleJsonFileEntityDataSetTests : IDisposable
{
    private readonly LocalPath _filePath;
    private readonly SingleFilePeopleContext _context;

    public SingleJsonFileEntityDataSetTests()
    {
        (_context, _filePath) = SingleFilePeopleContext.GetNewTestContext();
    }

    [Fact]
    public async Task AddEntity_WithNewEntity_CreatesDeserializableJsonFileOnDisk()
    {
        // arrange
        var person = new PersonEntity
        {
            Name = "Bob",
            Age = 25,
        };

        // act
        _context.People.AddEntity(person);
        await _context.SaveTrackedChanges(TestContext.Current.CancellationToken);

        // assert
        File.Exists(_filePath).AssertTrue();

        var json = await File.ReadAllTextAsync(_filePath, TestContext.Current.CancellationToken);
        
        // we use an array of KVPs internally not dictionary which are objects in json
        var dict = JsonSerializer.Deserialize<KeyValuePair<Guid, PersonEntity>[]>(json);
        dict.AssertNotNull();
        dict.AssertExactLength(1);
        dict[0].Key.AssertEqual(person.Id);
        dict[0].Value.AssertEqual(person);
    }

    [Fact]
    public async Task GetEntityById_WithExistingEntity_ReturnsCorrectEntity()
    {
        // arrange
        _ = AddTestEntity();
        var existingEntity2 = AddTestEntity();
        _ = AddTestEntity();

        // act
        var fetchedEntity = await _context.People.GetEntityById(existingEntity2.Id);

        // assert
        fetchedEntity.AssertNotNull();
        fetchedEntity.Name.AssertEqual(existingEntity2.Name);
        fetchedEntity.Age.AssertEqual(existingEntity2.Age);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetEntityById_WithExistingEntity_TracksEntityWhenRequired(bool track)
    {
        // arrange
        var existingEntity = AddTestEntity();

        // act
        var fetchedEntity = await _context.People.GetEntityById(existingEntity.Id, track);

        // assert
        fetchedEntity.AssertNotNull();

        if (track)
        {
            var trackerEntry = _context.ChangeTracker.Entries[0];
            fetchedEntity.AssertEqual(trackerEntry.TrackedInstance);
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
        var entity1 = AddTestEntity();
        var entity2 = AddTestEntity();
        var entity3 = AddTestEntity();

        var entityToUpdate = await _context.People.GetEntityById(entity2.Id, track: true);

        // act
        var newAge = entityToUpdate!.Age + 3;
        entityToUpdate.Age = newAge;
        await _context.SaveTrackedChanges(TestContext.Current.CancellationToken);

        // assert
        var fetchedEntity1 = await _context.People.GetEntityById(entity1.Id);
        fetchedEntity1.AssertNotNull();
        fetchedEntity1.AssertEqual(entity1); // unchanged

        var fetchedEntity2 = await _context.People.GetEntityById(entity2.Id);
        fetchedEntity2.AssertNotNull();
        fetchedEntity2.Id.AssertEqual(entity2.Id);
        fetchedEntity2.Age.AssertEqual(newAge);
        fetchedEntity2.Name.AssertEqual(entity2.Name);

        var fetchedEntity3 = await _context.People.GetEntityById(entity3.Id);
        fetchedEntity3.AssertNotNull();
        fetchedEntity3.AssertEqual(entity3); // unchanged

        // test file directly
        var json = await File.ReadAllTextAsync(_filePath, TestContext.Current.CancellationToken);
        
        // we use an array of KVPs internally not dictionary which are objects in json
        var entries = JsonSerializer.Deserialize<KeyValuePair<Guid, PersonEntity>[]>(json);
        entries.AssertNotNull();
        entries.AssertExactLength(3);
        entries.FirstOrDefault(f => f.Key == entity2.Id).Value.Age.AssertEqual(newAge);
    }

    [Fact]
    public async Task DeleteEntity_WithExistingEntity_RemovesEntityCompletely()
    {
        // arrange
        var existingEntity = AddTestEntity();

        // act
        _context.People.DeleteEntity(existingEntity);
        await _context.SaveTrackedChanges(TestContext.Current.CancellationToken);

        // assert
        var fetchedEntity = await _context.People.GetEntityById(existingEntity.Id);
        fetchedEntity.AssertNull();

        var json = await File.ReadAllTextAsync(_filePath, TestContext.Current.CancellationToken);
        var dict = JsonSerializer.Deserialize<KeyValuePair<Guid, PersonEntity>[]>(json);
        dict.AssertNotNull();
        dict.AssertEmpty();
    }

    [Fact]
    public async Task GetEntities_WithExistingEntities_ReturnsAllEntities()
    {
        // arrange
        var existingEntity1 = AddTestEntity();
        var existingEntity2 = AddTestEntity();
        var existingEntity3 = AddTestEntity();

        // act
        var all = await _context.People.GetEntities();

        // assert
        all.AssertExactLength(3);
        all.AssertContains(existingEntity1);
        all.AssertContains(existingEntity2);
        all.AssertContains(existingEntity3);
    }

    [Fact]
    public async Task UpdateEntity_WhenEntityDoesNotExistAndUpsertDisabled_ThrowsException()
    {
        // arrange
        var person = new PersonEntity
        {
            Name = "Bob",
            Age = 40,
        };

        // act
        _context.PeopleStrict.UpdateEntity(person);
        var actTask = _context.SaveTrackedChanges(TestContext.Current.CancellationToken);

        // assert
        var ex = await actTask.AssertThrows<EntityOperationException>();
        ex.FailedEntities.AssertExactLength(1);
        ex.FailedEntities.Single().Entity.AssertEqual(person);
        ex.FailedEntities.Single().ErrorCode.AssertEqual(EntityOperationErrorCode.EntityDoesNotExist);
    }

    [Fact]
    public async Task DeleteEntity_WhenEntityDoesNotExistAndIgnoreNonexistentFilesWhenDeletingDisabled_ThrowsException()
    {
        // arrange
        var person = new PersonEntity
        {
            Name = "Bob",
            Age = 40,
        };

        // act
        _context.PeopleStrict.DeleteEntity(person);
        var actTask = _context.SaveTrackedChanges(TestContext.Current.CancellationToken);

        // assert
        var ex = await actTask.AssertThrows<EntityOperationException>();
        ex.FailedEntities.AssertExactLength(1);
        ex.FailedEntities.Single().Entity.AssertEqual(person);
        ex.FailedEntities.Single().ErrorCode.AssertEqual(EntityOperationErrorCode.EntityDoesNotExist);
    }

    public void Dispose()
    {
        try
        {
            var dir = Path.GetDirectoryName(_filePath);
            if (dir != null)
            {
                Directory.Delete(dir, true);
            }
        }
        catch
        {
            /* ignore */
        }
    }

    private PersonEntity AddTestEntity()
    {
        var testPerson = new PersonEntity
        {
            Id = Guid.NewGuid(),
            Name = $"Alice {Guid.NewGuid()}",
            Age = Random.Shared.Next(1, 100),
        };

        Dictionary<Guid, PersonEntity> dict;
        if (File.Exists(_filePath))
        {
            var json = File.ReadAllText(_filePath);
            dict = new Dictionary<Guid, PersonEntity>(JsonSerializer
                    // we use an array of KVPs internally not dictionary which are objects in json
                    .Deserialize<KeyValuePair<Guid, PersonEntity>[]>(json)
                ?? []);
        }
        else
        {
            dict = [];
        }

        dict[testPerson.Id] = testPerson;
        // we use an array of KVPs internally not dictionary which are objects in json
        var jsonContent = JsonSerializer.Serialize<IEnumerable<KeyValuePair<Guid, PersonEntity>>>(dict);
        File.WriteAllText(_filePath, jsonContent);
        return testPerson;
    }
}
