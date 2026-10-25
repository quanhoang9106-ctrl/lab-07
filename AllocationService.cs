
using System;

namespace LabW07;

public class AllocationService<T> where T : TeachingDevice
{
    private readonly IRepository<T> repository;
    private ISelectionStrategy<T> strategy;

    public AllocationService(
        IRepository<T> repository,
        ISelectionStrategy<T> strategy)
    {
        this.repository = repository
            ?? throw new ArgumentNullException(nameof(repository));

        this.strategy = strategy
            ?? throw new ArgumentNullException(nameof(strategy));
    }

    public void ChangeStrategy(ISelectionStrategy<T> newStrategy)
    {
        strategy = newStrategy
            ?? throw new ArgumentNullException(nameof(newStrategy));
    }

    public T Allocate(Predicate<T>? additionalCondition = null)
    {
        var candidates = repository.FindAll(device =>
            device.IsReadyForUse() &&
            (additionalCondition == null || additionalCondition(device)));

        T selected = strategy.Select(candidates);
        selected.Borrow();

        return selected;
    }
}
