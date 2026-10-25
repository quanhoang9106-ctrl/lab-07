
using System;

namespace LabW07;

// Loi khi them thiet bi trung ma
public class DuplicateIdException : Exception
{
    public string DuplicateId { get; }

    public DuplicateIdException(string duplicateId)
        : base($"Ma thiet bi {duplicateId} da ton tai.")
    {
        DuplicateId = duplicateId;
    }
}

// Loi khi khong co ung vien phu hop
public class NoSuitableItemException : Exception
{
    public NoSuitableItemException()
        : base("Khong tim thay thiet bi phu hop de cap phat.")
    {
    }
}

// Loi khi muon mot thiet bi chua san sang
public class DeviceNotReadyException : Exception
{
    public string DeviceId { get; }
    public DeviceStatus CurrentStatus { get; }

    public DeviceNotReadyException(
        string deviceId,
        DeviceStatus currentStatus)
        : base($"Thiet bi {deviceId} chua san sang su dung.")
    {
        DeviceId = deviceId;
        CurrentStatus = currentStatus;
    }
}
