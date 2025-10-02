using Brokkr.DDD.ChangeTracking;
using Brokkr.DDD.Saga;
using Brokkr.DDD.UoW;

namespace Brokkr.DDD.UnitTests;

internal sealed class TestSaga : SagaBase
{
    public void Register<TEntity>(IUnitOfWorkPatternRepository<TEntity> repository, IUnitOfWork unitOfWork)
    {
        EntityContextMap[typeof(TEntity)] = new EntityContext(repository, unitOfWork);
    }
}

internal sealed class DummyDataSet : IDataSet
{
    public Func<Task<EntityOperationFailure?>> GetOperationForEntityChange(ITrackerEntry entry)
    {
        return () => Task.FromResult<EntityOperationFailure?>(null);
    }
}

public sealed class Person : TrackableObject<Person>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int Age { get; set; }

    public override Person DeepClone()
    {
        return new Person
        {
            FirstName = FirstName,
            LastName = LastName,
            Age = Age,
        };
    }

    public override bool Equals(Person? other)
    {
        if (other is null)
        {
            return false;
        }

        return FirstName == other.FirstName && LastName == other.LastName && Age == other.Age;
    }

    public override string ToString()
    {
        return $"{FirstName} {LastName} ({Age})";
    }
}
