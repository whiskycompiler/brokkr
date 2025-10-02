using Brokkr.DDD.Saga;
using Brokkr.DDD.UoW;

namespace Brokkr.DDD.UnitTests;

public sealed class SagaTests
{
    [Fact]
    public void AddOperation_GivenUnknownEntityType_ThrowsSagaException()
    {
        var saga = new TestSaga();

        var ex = Assert.Throws<SagaException>(() =>
            saga.AddOperation<Person>(_ =>
            {
                /* no-op */
            }));

        Assert.Equal(SagaErrorCode.UnknownEntityType, ex.ErrorCode);
    }

    [Fact]
    public async Task SaveTrackedChanges_GivenMultipleSyncOperations_ExecutesInOrderAndSavesAfterEach()
    {
        var saga = new TestSaga();
        var repo = A.Fake<IUnitOfWorkPatternRepository<Person>>();
        var uow = A.Fake<IUnitOfWork>();
        A.CallTo(() => uow.SaveTrackedChanges(A<CancellationToken>._)).Returns(Task.CompletedTask);
        saga.Register(repo, uow);

        var log = new List<string>();
        saga.AddOperation<Person>(_ => log.Add("op1"));
        saga.AddOperation<Person>(_ => log.Add("op2"));

        await saga.SaveTrackedChanges();

        Assert.Equal(new[] { "op1", "op2" }, log);
        // one save per operation
        A.CallTo(() => uow.SaveTrackedChanges(A<CancellationToken>._)).MustHaveHappened(2, Times.Exactly);
    }

    [Fact]
    public async Task SaveTrackedChanges_GivenMultipleAsyncOperations_ExecutesInOrderAndSavesAfterEach()
    {
        var saga = new TestSaga();
        var repo = A.Fake<IUnitOfWorkPatternRepository<Person>>();
        var uow = A.Fake<IUnitOfWork>();
        A.CallTo(() => uow.SaveTrackedChanges(A<CancellationToken>._)).Returns(Task.CompletedTask);
        saga.Register(repo, uow);

        var log = new List<string>();
        saga.AddOperation<Person>(async _ =>
        {
            await Task.Delay(1);
            log.Add("op1");
        });

        saga.AddOperation<Person>(async _ =>
        {
            await Task.Delay(1);
            log.Add("op2");
        });

        await saga.SaveTrackedChanges();

        Assert.Equal(new[] { "op1", "op2" }, log);
        A.CallTo(() => uow.SaveTrackedChanges(A<CancellationToken>._)).MustHaveHappened(2, Times.Exactly);
    }

    [Fact]
    public async Task SaveTrackedChanges_GivenMultipleMixedOperations_ExecutesInOrderAndSavesAfterEach()
    {
        var saga = new TestSaga();
        var repo = A.Fake<IUnitOfWorkPatternRepository<Person>>();
        var uow = A.Fake<IUnitOfWork>();
        A.CallTo(() => uow.SaveTrackedChanges(A<CancellationToken>._)).Returns(Task.CompletedTask);
        saga.Register(repo, uow);

        var log = new List<string>();
        saga.AddOperation<Person>(_ => log.Add("op1-sync"));
        saga.AddOperation<Person>(async _ =>
        {
            await Task.Delay(1);
            log.Add("op2-async");
        });

        saga.AddOperation<Person>(_ => log.Add("op3-sync"));

        await saga.SaveTrackedChanges();

        Assert.Equal(new[] { "op1-sync", "op2-async", "op3-sync" }, log);
        A.CallTo(() => uow.SaveTrackedChanges(A<CancellationToken>._)).MustHaveHappened(3, Times.Exactly);
    }

    [Fact]
    public async Task SaveTrackedChanges_OnOperationFailure_PerformsCompensationInReverseAndThrowsSagaFailedException()
    {
        var saga = new TestSaga();
        var repo = A.Fake<IUnitOfWorkPatternRepository<Person>>();
        var uow = A.Fake<IUnitOfWork>();
        A.CallTo(() => uow.SaveTrackedChanges(A<CancellationToken>._)).Returns(Task.CompletedTask);
        saga.Register(repo, uow);

        var log = new List<string>();
        saga.AddOperation<Person>(_ => log.Add("op1"), _ => log.Add("comp1"));
        saga.AddOperation<Person>(_ => throw new InvalidOperationException("fail2"), _ => log.Add("comp2"));
        saga.AddOperation<Person>(_ => log.Add("op3"), _ => log.Add("comp3"));

        var ex = await Assert.ThrowsAsync<SagaFailedException>(async () => await saga.SaveTrackedChanges());

        // Compensation should have been called only for successful prior operations, in reverse order
        Assert.Equal(new[] { "comp1" },
            log.Where(s => s.StartsWith("comp", StringComparison.InvariantCulture)).ToArray());

        Assert.IsType<InvalidOperationException>(ex.InnerException);
        Assert.Empty(ex.RollbackExceptions);

        // Save was called twice: once for op1 success, and once for compensation of op1
        A.CallTo(() => uow.SaveTrackedChanges(A<CancellationToken>._)).MustHaveHappened(2, Times.Exactly);
    }

    [Fact]
    public async Task SaveTrackedChanges_OnRollbackFailure_RollbackExceptionsAreInSagaFailedException()
    {
        var saga = new TestSaga();
        var repo = A.Fake<IUnitOfWorkPatternRepository<Person>>();
        var uow = A.Fake<IUnitOfWork>();
        A.CallTo(() => uow.SaveTrackedChanges(A<CancellationToken>._)).Returns(Task.CompletedTask);
        saga.Register(repo, uow);

        var log = new List<string>();
        saga.AddOperation<Person>(_ => log.Add("op1"), _ => throw new ApplicationException("comp1-fail"));
        saga.AddOperation<Person>(_ => throw new InvalidOperationException("op2-fail"), _ => log.Add("comp2"));

        var ex = await Assert.ThrowsAsync<SagaFailedException>(async () => await saga.SaveTrackedChanges());

        Assert.Single(log);
        Assert.IsType<InvalidOperationException>(ex.InnerException);
        Assert.Single(ex.RollbackExceptions);
        Assert.IsType<ApplicationException>(ex.RollbackExceptions.First());
    }

    [Fact]
    public async Task SaveTrackedChanges_GivenNullCompensation_WorksNormally()
    {
        var saga = new TestSaga();
        var repo = A.Fake<IUnitOfWorkPatternRepository<Person>>();
        var uow = A.Fake<IUnitOfWork>();
        A.CallTo(() => uow.SaveTrackedChanges(A<CancellationToken>._)).Returns(Task.CompletedTask);
        saga.Register(repo, uow);

        saga.AddOperation<Person>(_ =>
        {
            /* op1 */
        });

        saga.AddOperation<Person>(_ => throw new Exception("boom"));

        var ex = await Assert.ThrowsAsync<SagaFailedException>(() => saga.SaveTrackedChanges());
        Assert.NotNull(ex.InnerException);
        // Only compensation for op1 would run but it is null -> no extra calls beyond the first save
        A.CallTo(() => uow.SaveTrackedChanges(A<CancellationToken>._)).MustHaveHappened(2, Times.Exactly);
    }
}
