
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
        if (repository == null)
        {
            throw new ArgumentNullException(nameof(repository));
        }

        if (strategy == null)
        {
            throw new ArgumentNullException(nameof(strategy));
        }

        this.repository = repository;
        this.strategy = strategy;
    }

    // Cho phep thay doi chien luoc khi dang chay
    public void ChangeStrategy(ISelectionStrategy<T> newStrategy)
    {
        if (newStrategy == null)
        {
            throw new ArgumentNullException(nameof(newStrategy));
        }

        strategy = newStrategy;
    }

    public T Allocate(Predicate<T>? additionalCondition = null)
    {
        // Loc cac thiet bi san sang va thoa man dieu kien
        IReadOnlyList<T> candidates = repository.FindAll(device =>
            device.IsReadyForUse() &&
            (additionalCondition == null || additionalCondition(device)));

        // Chon thiet bi theo chien luoc hien tai
        T selected = strategy.Select(candidates);

        // Chuyen trang thai sang Borrowed
        selected.Borrow();

        return selected;
    }
}
