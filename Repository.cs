
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

public class InMemoryRepository<T> : IRepository<T>
    where T : IIdentifiable
{
    // Luu thiet bi theo thu tu them
    private readonly LinkedList<T> items = new();

    // Tra cuu nhanh theo ID
    private readonly Dictionary<string, LinkedListNode<T>> byId =
        new(StringComparer.Ordinal);

    public void Add(T item)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (string.IsNullOrWhiteSpace(item.Id))
            throw new ArgumentException("Ma thiet bi khong hop le.");

        if (byId.ContainsKey(item.Id))
            throw new DuplicateIdException(item.Id);

        LinkedListNode<T> node = items.AddLast(item);
        byId.Add(item.Id, node);
    }

    public bool Remove(string id)
    {
        if (string.IsNullOrWhiteSpace(id) ||
            !byId.TryGetValue(id, out var node))
            return false;

        items.Remove(node);
        byId.Remove(id);

        return true;
    }

    public T? FindById(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return default;

        return byId.TryGetValue(id, out var node)
            ? node.Value
            : default;
    }

    public IReadOnlyList<T> GetAll() =>
        new List<T>(items).AsReadOnly();

    public IReadOnlyList<T> FindAll(Predicate<T> condition)
    {
        ArgumentNullException.ThrowIfNull(condition);

        var result = new List<T>();

        foreach (T item in items)
        {
            if (condition(item))
                result.Add(item);
        }

        return result.AsReadOnly();
    }
}
