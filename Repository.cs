using System;
using System.Collections.Generic;

namespace LabW07;

public interface IRepository<T> where T : IIdentifiable
{
    void Add(T item);
    bool Remove(string id);
    T? FindById(string id);
    IReadOnlyList<T> GetAll();
    IReadOnlyList<T> FindAll(Predicate<T> condition);
}

public class InMemoryRepository<T> : IRepository<T> where T : IIdentifiable
{
    private readonly List<T> items = new List<T>();

    public void Add(T item)
    {
        if (item is null)
            throw new ArgumentNullException(nameof(item));

        if (FindById(item.Id) is not null)
            throw new DuplicateIdException(item.Id);

        items.Add(item);
    }

    public bool Remove(string id)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].Id == id)
            {
                items.RemoveAt(i);
                return true;
            }
        }
        return false;
    }

    public T? FindById(string id)
    {
        foreach (T item in items)
        {
            if (item.Id == id)
                return item;
        }
        return default;
    }

    public IReadOnlyList<T> GetAll()
    {
        // Tra ve ban sao, khong cho ben ngoai sua list goc.
        return new List<T>(items).AsReadOnly();
    }

    public IReadOnlyList<T> FindAll(Predicate<T> condition)
    {
        if (condition == null)
            throw new ArgumentNullException(nameof(condition));

        List<T> result = new List<T>();
        foreach (T item in items)
        {
            if (condition(item))
                result.Add(item);
        }
        return result.AsReadOnly();
    }
}
