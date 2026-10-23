
        Id = id;
        Name = name;
        StorageLocation = storageLocation;
        Status = DeviceStatus.Available;
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
            throw new InvalidOperationException("Thiet bi nay chua duoc muon.");

        Status = DeviceStatus.Available;
    }

    public override string ToString()
    {
        return $"{Id} - {Name} - {StorageLocation} - {Status}";
    }
}

public class ProjectorRemote : TeachingDevice
{
    public int BatteryLevel { get; }

    public ProjectorRemote(string id, string name, string location, int batteryLevel)
        : base(id, name, location)
    {
        if (batteryLevel < 0 || batteryLevel > 100)
            throw new ArgumentOutOfRangeException(nameof(batteryLevel));

        BatteryLevel = batteryLevel;
    }

    public override bool IsReadyForUse()
    {
        return Status == DeviceStatus.Available && BatteryLevel > 20;
    }

    public override double GetDefaultPriorityScore()
    {
        return BatteryLevel;
    }
}

public class AirConditionerRemote : TeachingDevice
{
    public IReadOnlySet<string> CompatibleRooms { get; }

    public AirConditionerRemote(string id, string name, string location,
        IEnumerable<string> compatibleRooms) : base(id, name, location)
    {
        if (compatibleRooms == null)
            throw new ArgumentNullException(nameof(compatibleRooms));

        var rooms = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (string room in compatibleRooms)
        {
            if (string.IsNullOrWhiteSpace(room))
                throw new ArgumentException("Ma phong khong hop le.");
            rooms.Add(room);
        }
        if (rooms.Count == 0)
            throw new ArgumentException("Can co it nhat mot phong tuong thich.");

        CompatibleRooms = rooms;
    }

    public bool IsCompatibleWith(string roomId)
    {
        return !string.IsNullOrWhiteSpace(roomId) && CompatibleRooms.Contains(roomId);
    }

    public override bool IsReadyForUse()
    {
        return Status == DeviceStatus.Available;
    }

    public override double GetDefaultPriorityScore()
    {
        // Phong phu hop duoc loc truoc trong Allocate().
        return IsReadyForUse() ? 100 : 0;
    }
}

public class WirelessMicrophone : TeachingDevice
{
    public int BatteryLevel { get; }
    public int SignalQuality { get; }

    public WirelessMicrophone(string id, string name, string location,
        int batteryLevel, int signalQuality) : base(id, name, location)
    {
        if (batteryLevel < 0 || batteryLevel > 100)
            throw new ArgumentOutOfRangeException(nameof(batteryLevel));
        if (signalQuality < 0 || signalQuality > 100)
            throw new ArgumentOutOfRangeException(nameof(signalQuality));

        BatteryLevel = batteryLevel;
        SignalQuality = signalQuality;
    }

    public override bool IsReadyForUse()
    {
        return Status == DeviceStatus.Available && BatteryLevel >= 30 && SignalQuality > 40;
    }

    public override double GetDefaultPriorityScore()
    {
        return (BatteryLevel + SignalQuality) / 2;
    }
}
