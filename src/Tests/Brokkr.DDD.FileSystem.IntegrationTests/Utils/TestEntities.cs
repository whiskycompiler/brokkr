using Brokkr.DDD.ChangeTracking;
using Brokkr.DDD.UoW;

namespace Brokkr.DDD.FileSystem.IntegrationTests.Utils;

public sealed class PersonEntity : TrackableObject<PersonEntity>, IEntity<Guid>
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? Name { get; set; }
    public int Age { get; set; }

    public override PersonEntity DeepClone()
    {
        return new PersonEntity
        {
            Id = Id,
            Name = Name,
            Age = Age,
        };
    }

    public override bool Equals(PersonEntity? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Id == other.Id
            && Name == other.Name
            && Age == other.Age;
    }
}

public sealed class AppSettings : TrackableObject<AppSettings>
{
    public string? Environment { get; set; }
    public int MaxItems { get; set; }

    public override AppSettings DeepClone()
    {
        return new AppSettings
        {
            Environment = Environment,
            MaxItems = MaxItems,
        };
    }

    public override bool Equals(AppSettings? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Environment == other.Environment
            && MaxItems == other.MaxItems;
    }
}
