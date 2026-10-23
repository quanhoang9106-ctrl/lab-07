using System;

namespace LabW07;

public class DuplicateIdException : Exception
{
    public string DuplicateId { get; }

    public DuplicateIdException(string duplicateId)
        : base($"Ma {duplicateId} da ton tai trong kho.")
    {
        DuplicateId = duplicateId;
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

    public DeviceNotReadyException(string deviceId, DeviceStatus currentStatus)
        : base($"Thiet bi {deviceId} chua san sang su dung.")
    {
        DeviceId = deviceId;
        CurrentStatus = currentStatus;
    }
}
