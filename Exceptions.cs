
using System;

namespace LabW07;

public class DuplicateIdException : Exception
{
    public string DuplicateId { get; }

    public DuplicateIdException(string id)
        : base($"Ma thiet bi {id} da ton tai.")
    {
        DuplicateId = id;
    }
}

public class NoSuitableItemException : Exception
{
    public NoSuitableItemException()
        : base("Khong co thiet bi phu hop de cap phat.")
    {
    }
}

public class DeviceNotReadyException : Exception
{
    public string DeviceId { get; }
    public DeviceStatus CurrentStatus { get; }

    public DeviceNotReadyException(string id, DeviceStatus status)
        : base($"Thiet bi {id} chua san sang.")
    {
        DeviceId = id;
        CurrentStatus = status;
    }
}
