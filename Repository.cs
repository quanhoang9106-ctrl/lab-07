
using System;
using System.Collections.Generic;

namespace LabW07;

// Interface quan ly kho luu tru
public interface IRepository<T> where T : IIdentifiable
{
    void Add(T item);
    bool Remove(string id);
    T? FindById(string id);
    IReadOnlyList<T> GetAll();
    IReadOnlyList<T> FindAll(Predicate<T> condition);
}

// Kho luu tru su dung List
public class InMemoryRepository<T> : IRepository<T>
    where T : IIdentifiable
{
    private readonly List<T> items = new List<T>();

    public void Add(T item)
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        if (FindById(item.Id) != null)
        {
            throw new DuplicateIdException(item.Id);
        }

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
            {
                return item;
            }
        }

        return default;
    }

    public IReadOnlyList<T> GetAll()
    {
        // Tao ban sao de khong sua truc tiep danh sach goc
        List<T> result = new List<T>(items);
        return result.AsReadOnly();
    }

    public IReadOnlyList<T> FindAll(Predicate<T> condition)
    {
        if (condition == null)
        {
            throw new ArgumentNullException(nameof(condition));
        }

        List<T> result = new List<T>();

        foreach (T item in items)
        {
            if (condition(item))
            {
                result.Add(item);
            }
        }

        return result.AsReadOnly();
    }
}
