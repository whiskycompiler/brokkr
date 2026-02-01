using System.Text.Json;

using Brokkr.DDD.ChangeTracking;
using Brokkr.DDD.FileSystem.IntegrationTests.Utils;
using Brokkr.DDD.UoW;
using Brokkr.Location.Abstractions;
using Brokkr.Testing.XUnit.Assertions;

namespace Brokkr.DDD.FileSystem.IntegrationTests.Tests;

public sealed class JsonFileEntityDataSetTests : IDisposable
{
    private readonly LocalPath _tempDir;
    private readonly MultiFilePeopleContext _context;

    public JsonFileEntityDataSetTests()
    {
        (_context, _tempDir) = MultiFilePeopleContext.GetNewTestContext();
    }

    [Fact]
    public async Task AddEntity_WithNewEntity_CreatesDeserializableJsonFileOnDisk()
    {
        // arrange
        // use separate context to test subfolder creation as well
        var tempDir = TestHelpers.CreateTempDir();
        var folderPath = LocalPath.Create(Path.Combine(tempDir, "subfolder"));
        var context = new MultiFilePeopleContext(folderPath);

        var person = new PersonEntity
        {
            Name = "Bob",
            Age = 25,
        };

        // act
        context.People.AddEntity(person);
        await context.SaveTrackedChanges(TestContext.Current.CancellationToken);

        // assert
        var filePath = Path.Combine(folderPath, person.Id + ".json");
        File.Exists(filePath).AssertTrue();

        var json = await File.ReadAllTextAsync(filePath, TestContext.Current.CancellationToken);
        JsonSerializer.Deserialize<PersonEntity>(json).AssertNotNull();
    }

    [Fact]
    public async Task GetById_WithExistingEntity_ReturnsCorrectEntity()
    {
        // arrange
        _ = AddTestEntity();
        var existingEntity2 = AddTestEntity();
        _ = AddTestEntity();

        // act
        var fetchedEntity = await _context.People.GetById(existingEntity2.Id);

        // assert
        fetchedEntity.AssertNotNull();
        fetchedEntity.Name.AssertEqual(existingEntity2.Name);
        fetchedEntity.Age.AssertEqual(existingEntity2.Age);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetById_WithExistingEntity_TracksEntityWhenRequired(bool track)
    {
        // arrange
        var existingEntity = AddTestEntity();

        // act
        var fetchedEntity = await _context.People.GetById(existingEntity.Id, track);

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
        
        var entityToUpdate = await _context.People.GetById(entity2.Id, track: true);
    
        // act
        var newAge = entityToUpdate!.Age + 3;
        entityToUpdate.Age = newAge;
        await _context.SaveTrackedChanges(TestContext.Current.CancellationToken);
    
        // assert
        var fetchedEntity1 = await _context.People.GetById(entity1.Id);
        fetchedEntity1.AssertNotNull();
        fetchedEntity1.AssertEqual(entity1); // unchanged
        
        var fetchedEntity2 = await _context.People.GetById(entity2.Id);
        fetchedEntity2.AssertNotNull();
        fetchedEntity2.Id.AssertEqual(entity2.Id);
        fetchedEntity2.Age.AssertEqual(newAge);
        fetchedEntity2.Name.AssertEqual(entity2.Name);
        
        var fetchedEntity3 = await _context.People.GetById(entity3.Id);
        fetchedEntity3.AssertNotNull();
        fetchedEntity3.AssertEqual(entity3); // unchanged
        
        // test file directly
        var filePath = Path.Combine(_tempDir, entity2.Id + ".json");
        File.Exists(filePath).AssertTrue();

        var json = await File.ReadAllTextAsync(filePath, TestContext.Current.CancellationToken);
        var deserializedEntity = JsonSerializer.Deserialize<PersonEntity>(json);
        deserializedEntity.AssertNotNull();
        deserializedEntity.AssertEqual(fetchedEntity2);
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
        Directory.EnumerateFiles(_tempDir).Count().AssertEqual(0);
        
        var fetchedEntity = await _context.People.GetById(existingEntity.Id);
        fetchedEntity.AssertNull();
    }
    
    [Fact]
    public async Task EnumerateAsync_WithOneExistingEntity_ReturnsAllEntities()
    {
        // arrange
        var existingEntity1 = AddTestEntity();
        var existingEntity2 = AddTestEntity();
        var existingEntity3 = AddTestEntity();
        
        // act
        var all = await _context.People.EnumerateAsync()
            .ToArrayAsync(cancellationToken: TestContext.Current.CancellationToken);
    
        // assert
        all.AssertExactLength(3);
        all.AssertContains(existingEntity1);
        all.AssertContains(existingEntity2);
        all.AssertContains(existingEntity3);
    }
    
    [Fact]
    public async Task EnumerateAsync_AfterMultipleSavedEntities_ReturnsAllEntities()
    {
        // arrange
        var person1 = AddTestEntity();
        
        var person2 = new PersonEntity
        {
            Name = "Bob",
            Age = 25,
        };
    
        var person3 = new PersonEntity
        {
            Name = "Hans",
            Age = 55,
        };
    
        _context.People.AddEntity(person2);
        _context.People.AddEntity(person3);
        await _context.SaveTrackedChanges(TestContext.Current.CancellationToken);
    
        // act
        var all = await _context.People.EnumerateAsync()
            .ToArrayAsync(cancellationToken: TestContext.Current.CancellationToken);
    
        // assert
        all.AssertExactLength(3);
        all.AssertContains(person1);
        all.AssertContains(person2);
        all.AssertContains(person3);
    }
    
    [Fact]
    public async Task GetByLocation_ReturnsCorrectEntity()
    {
        // arrange
        var testPerson = AddTestEntity();
        var location = LocalPath.Create(Path.Combine(_tempDir, testPerson.Id + ".json"));
    
        // act
        var entity = await _context.People.GetByLocation(location);
    
        // assert
        entity.AssertNotNull();
        entity.AssertEqual(testPerson);
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
        
        File.Exists(Path.Combine(_tempDir, person.Id + ".json")).AssertFalse();
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
        var actTask =  _context.SaveTrackedChanges(TestContext.Current.CancellationToken);
    
        // assert
        var ex = await actTask.AssertThrows<EntityOperationException>();
        ex.FailedEntities.AssertExactLength(1);
        ex.FailedEntities.Single().Entity.AssertEqual(person);
        ex.FailedEntities.Single().ErrorCode.AssertEqual(EntityOperationErrorCode.EntityDoesNotExist);
        
        File.Exists(Path.Combine(_tempDir, person.Id + ".json")).AssertFalse();
    }
    
    [Fact]
    public async Task DeleteEntity_WhenEntityExistsAndIgnoreNonexistentFilesWhenDeletingDisabled_DoesNotThrow()
    {
        // arrange
        var person = AddTestEntity();
        
        // act
        _context.PeopleStrict.DeleteEntity(person);
        await _context.SaveTrackedChanges(TestContext.Current.CancellationToken);
        
        File.Exists(Path.Combine(_tempDir, person.Id + ".json")).AssertFalse();
    }
    
    public void Dispose()
    {
        try
        {
            Directory.Delete(_tempDir, true);
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

        // Manually create the JSON file for the test person
        var jsonContent = JsonSerializer.Serialize(testPerson);
        var filePath = Path.Combine(_tempDir, testPerson.Id + ".json");
        File.WriteAllText(filePath, jsonContent);
        return testPerson;
    }
}
