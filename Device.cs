
using System;
using System.Collections.Generic;
using System.Collections.Frozen;

namespace LabW07;

public enum DeviceStatus
{
    Available,
    Borrowed,
    UnderMaintenance
}

public interface IIdentifiable
{
    string Id { get; }
}

public abstract class TeachingDevice : IIdentifiable
{
    public string Id { get; }
    public string Name { get; }
    public string StorageLocation { get; }
    public DeviceStatus Status { get; protected set; }

    protected TeachingDevice(string id, string name, string location)
    {
        Id = RequiredText(id, nameof(id));
        Name = RequiredText(name, nameof(name));
        StorageLocation = RequiredText(location, nameof(location));
        Status = DeviceStatus.Available;
    }

    private static string RequiredText(string value, string parameter)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Gia tri khong hop le.", parameter);

        return value.Trim();
    }

    protected static int ValidPercent(int value, string parameter)
    {
        if (value < 0 || value > 100)
            throw new ArgumentOutOfRangeException(parameter);

        return value;
    }

    public abstract bool IsReadyForUse();
    public abstract double GetDefaultPriorityScore();

    public void Borrow()
    {
        if (!IsReadyForUse())
            throw new DeviceNotReadyException(Id, Status);

        Status = DeviceStatus.Borrowed;
    }

    public void Return()
    {
        if (Status != DeviceStatus.Borrowed)
            throw new InvalidOperationException("Thiet bi chua duoc muon.");

        Status = DeviceStatus.Available;
    }

    public override string ToString() =>
        $"{Id} - {Name} - {StorageLocation} - {Status}";
}

// Dieu khien may chieu
public class ProjectorRemote : TeachingDevice
{
    public int BatteryLevel { get; }

    public ProjectorRemote(string id, string name, string location, int battery)
        : base(id, name, location)
    {
        BatteryLevel = ValidPercent(battery, nameof(battery));
    }

    public override bool IsReadyForUse() =>
        Status == DeviceStatus.Available && BatteryLevel >= 20;

    public override double GetDefaultPriorityScore() => BatteryLevel;
}

// Dieu khien dieu hoa
public class AirConditionerRemote : TeachingDevice
{
    public IReadOnlySet<string> CompatibleRooms { get; }

    public AirConditionerRemote(
        string id, string name, string location,
        IEnumerable<string> compatibleRooms)
        : base(id, name, location)
    {
        ArgumentNullException.ThrowIfNull(compatibleRooms);

        var rooms = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (string room in compatibleRooms)
        {
            if (string.IsNullOrWhiteSpace(room))
                throw new ArgumentException("Ma phong khong hop le.");

            rooms.Add(room.Trim());
        }

        if (rooms.Count == 0)
            throw new ArgumentException("Danh sach phong khong duoc rong.");

        CompatibleRooms = rooms.ToFrozenSet(StringComparer.OrdinalIgnoreCase);
    }

    public bool IsCompatibleWith(string roomId) =>
        !string.IsNullOrWhiteSpace(roomId) &&
        CompatibleRooms.Contains(roomId.Trim());

    public override bool IsReadyForUse() =>
        Status == DeviceStatus.Available;

    // Chi tinh diem sau khi da loc theo phong tuong thich
    public override double GetDefaultPriorityScore() =>
        IsReadyForUse() ? 100 : 0;
}

// Micro khong day
public class WirelessMicrophone : TeachingDevice
{
    public int BatteryLevel { get; }
    public int SignalQuality { get; }

    public WirelessMicrophone(
        string id, string name, string location,
        int battery, int signal)
        : base(id, name, location)
    {
        BatteryLevel = ValidPercent(battery, nameof(battery));
        SignalQuality = ValidPercent(signal, nameof(signal));
    }

    public override bool IsReadyForUse() =>
        Status == DeviceStatus.Available &&
        BatteryLevel >= 30 &&
        SignalQuality >= 40;

    public override double GetDefaultPriorityScore() =>
        (BatteryLevel + SignalQuality) / 2.0;
}
