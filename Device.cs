
using System;
using System.Collections.Generic;

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

// Lop cha cua tat ca cac loai thiet bi
public abstract class TeachingDevice : IIdentifiable
{
    public string Id { get; }
    public string Name { get; }
    public string StorageLocation { get; }
    public DeviceStatus Status { get; protected set; }

    protected TeachingDevice(string id, string name, string storageLocation)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Ma thiet bi khong hop le.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Ten thiet bi khong hop le.");

        if (string.IsNullOrWhiteSpace(storageLocation))
            throw new ArgumentException("Vi tri luu tru khong hop le.");

        Id = id.Trim();
        Name = name.Trim();
        StorageLocation = storageLocation.Trim();
        Status = DeviceStatus.Available;
    }

    public abstract bool IsReadyForUse();

    public abstract double GetDefaultPriorityScore();

    public void Borrow()
    {
        if (!IsReadyForUse())
        {
            throw new DeviceNotReadyException(Id, Status);
        }

        Status = DeviceStatus.Borrowed;
    }

    public void Return()
    {
        if (Status != DeviceStatus.Borrowed)
        {
            throw new InvalidOperationException(
                "Thiet bi nay chua duoc muon.");
        }

        Status = DeviceStatus.Available;
    }

    public override string ToString()
    {
        return $"{Id} - {Name} - {StorageLocation} - {Status}";
    }
}

// Dieu khien may chieu
public class ProjectorRemote : TeachingDevice
{
    public int BatteryLevel { get; }

    public ProjectorRemote(
        string id,
        string name,
        string location,
        int batteryLevel)
        : base(id, name, location)
    {
        if (batteryLevel < 0 || batteryLevel > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(batteryLevel));
        }

        BatteryLevel = batteryLevel;
    }

    public override bool IsReadyForUse()
    {
        return Status == DeviceStatus.Available && BatteryLevel >= 20;
    }

    public override double GetDefaultPriorityScore()
    {
        return BatteryLevel;
    }
}

// Dieu khien dieu hoa
public class AirConditionerRemote : TeachingDevice
{
    public IReadOnlySet<string> CompatibleRooms { get; }

    public AirConditionerRemote(
        string id,
        string name,
        string location,
        IEnumerable<string> compatibleRooms)
        : base(id, name, location)
    {
        if (compatibleRooms == null)
        {
            throw new ArgumentNullException(nameof(compatibleRooms));
        }

        HashSet<string> rooms = new HashSet<string>(
            StringComparer.OrdinalIgnoreCase);

        foreach (string room in compatibleRooms)
        {
            if (string.IsNullOrWhiteSpace(room))
            {
                throw new ArgumentException("Ma phong khong hop le.");
            }

            rooms.Add(room.Trim());
        }

        if (rooms.Count == 0)
        {
            throw new ArgumentException(
                "Phai co it nhat mot phong tuong thich.");
        }

        CompatibleRooms = rooms;
    }

    public bool IsCompatibleWith(string roomId)
    {
        if (string.IsNullOrWhiteSpace(roomId))
        {
            return false;
        }

        return CompatibleRooms.Contains(roomId.Trim());
    }

    public override bool IsReadyForUse()
    {
        return Status == DeviceStatus.Available;
    }

    public override double GetDefaultPriorityScore()
    {
        // Phong tuong thich duoc kiem tra khi loc thiet bi
        if (IsReadyForUse())
        {
            return 100;
        }

        return 0;
    }
}

// Micro khong day
public class WirelessMicrophone : TeachingDevice
{
    public int BatteryLevel { get; }
    public int SignalQuality { get; }

    public WirelessMicrophone(
        string id,
        string name,
        string location,
        int batteryLevel,
        int signalQuality)
        : base(id, name, location)
    {
        if (batteryLevel < 0 || batteryLevel > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(batteryLevel));
        }

        if (signalQuality < 0 || signalQuality > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(signalQuality));
        }

        BatteryLevel = batteryLevel;
        SignalQuality = signalQuality;
    }

    public override bool IsReadyForUse()
    {
        return Status == DeviceStatus.Available
            && BatteryLevel >= 30
            && SignalQuality >= 40;
    }

    public override double GetDefaultPriorityScore()
    {
        return (BatteryLevel + SignalQuality) / 2.0;
    }
}
