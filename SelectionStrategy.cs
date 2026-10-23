using System;
using System.Collections.Generic;

namespace LabW07;

public interface ISelectionStrategy<T>
{
    T Select(IReadOnlyList<T> candidates);
}

public class HighestPriorityStrategy<T> : ISelectionStrategy<T>
{
    private readonly Func<T, double> scoreSelector;

    public HighestPriorityStrategy(Func<T, double> scoreSelector)
    {
        this.scoreSelector = scoreSelector ?? throw new ArgumentNullException(nameof(scoreSelector));
    }

    public T Select(IReadOnlyList<T> candidates)
    {
        if (candidates == null)
            throw new ArgumentNullException(nameof(candidates));
        if (candidates.Count == 0)
            throw new NoSuitableItemException();

        T best = candidates[0];
        double bestScore = scoreSelector(best);

        for (int i = 1; i < candidates.Count; i++)
        {
            double score = scoreSelector(candidates[i]);
            if (score > bestScore)
            {
                best = candidates[i];
                bestScore = score;
            }
        }
        return best;
    }
}

public class FirstAvailableStrategy<T> : ISelectionStrategy<T>
{
    public T Select(IReadOnlyList<T> candidates)
    {
        if (candidates == null)
            throw new ArgumentNullException(nameof(candidates));
        if (candidates.Count == 0)
            throw new NoSuitableItemException();

        return candidates[0];
    }
}
