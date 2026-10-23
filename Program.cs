using System;
using System.Text;

namespace LabW07;

internal class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("LAB W07 - QUAN LY THIET BI PHONG HOC");

        IRepository<ProjectorRemote> projectorRepo = new InMemoryRepository<ProjectorRemote>();
        IRepository<AirConditionerRemote> airRepo = new InMemoryRepository<AirConditionerRemote>();
        IRepository<WirelessMicrophone> micRepo = new InMemoryRepository<WirelessMicrophone>();

        // 1. Tao du lieu va them vao kho
        PrintTitle("1. Them thiet bi");
        projectorRepo.Add(new ProjectorRemote("P01", "Dieu khien may chieu 1", "Kho A", 50));
        projectorRepo.Add(new ProjectorRemote("P02", "Dieu khien may chieu 2", "Kho A", 20));
        projectorRepo.Add(new ProjectorRemote("P03", "Dieu khien may chieu 3", "Kho B", 90));

        airRepo.Add(new AirConditionerRemote("A01", "Dieu khien dieu hoa 1", "Kho B",
            new[] { "A203", "A204" }));
        airRepo.Add(new AirConditionerRemote("A02", "Dieu khien dieu hoa 2", "Kho B",
            new[] { "B101", "B102" }));

        micRepo.Add(new WirelessMicrophone("M01", "Micro 1", "Kho C", 30, 40));
        micRepo.Add(new WirelessMicrophone("M02", "Micro 2", "Kho C", 75, 60));
        micRepo.Add(new WirelessMicrophone("M03", "Micro 3", "Kho C", 55, 72));

        Console.WriteLine($"Da them {projectorRepo.GetAll().Count + airRepo.GetAll().Count + micRepo.GetAll().Count} thiet bi.");

        // 2. Trung ma
        PrintTitle("2. Thu them trung Id");
        try
        {
            projectorRepo.Add(new ProjectorRemote("P01", "May chieu khac", "Kho A", 80));
        }
        catch (DuplicateIdException ex)
        {
            Console.WriteLine("Da bat loi: " + ex.Message);
        }

        // 3. Tim theo ma
        PrintTitle("3. Tim thiet bi theo Id");
        var found = projectorRepo.FindById("P02");
        Console.WriteLine(found == null ? "Khong tim thay" : found.ToString());

        // 4. Loc cac thiet bi san sang
        PrintTitle("4. Loc thiet bi san sang");
        var readyProjectors = projectorRepo.FindAll(p => p.IsReadyForUse());
        foreach (ProjectorRemote p in readyProjectors)
            Console.WriteLine($"{p.Id}: pin {p.BatteryLevel}%");

        // 5. Chon may chieu co diem uu tien cao nhat
        PrintTitle("5. Cap phat may chieu pin cao nhat");
        var projectorService = new AllocationService<ProjectorRemote>(projectorRepo,
            new HighestPriorityStrategy<ProjectorRemote>(p => p.GetDefaultPriorityScore()));
        ProjectorRemote chosenProjector = projectorService.Allocate(p => true);
        Console.WriteLine($"Da chon {chosenProjector.Id}, pin {chosenProjector.BatteryLevel}%");

        // 6. Chon dieu hoa hop phong A203
        PrintTitle("6. Cap phat dieu hoa cho phong A203");
        var airService = new AllocationService<AirConditionerRemote>(airRepo,
            new HighestPriorityStrategy<AirConditionerRemote>(a => a.GetDefaultPriorityScore()));
        AirConditionerRemote chosenAir = airService.Allocate(a => a.IsCompatibleWith("A203"));
        Console.WriteLine("Da chon: " + chosenAir.Id);

        // 7. Chon micro dau tien san sang
        PrintTitle("7. Cap phat micro dau tien san sang");
        var micService = new AllocationService<WirelessMicrophone>(micRepo,
            new FirstAvailableStrategy<WirelessMicrophone>());
        WirelessMicrophone chosenMic = micService.Allocate(m => true);
        Console.WriteLine("Da chon: " + chosenMic.Id);

        // 8. Thay doi chien luoc khi chuong trinh dang chay
        PrintTitle("8. Doi chien luoc chon may chieu");
        chosenProjector.Return();
        projectorService.ChangeStrategy(new FirstAvailableStrategy<ProjectorRemote>());
        ProjectorRemote afterChange = projectorService.Allocate(p => true);
        Console.WriteLine("Sau khi doi strategy, chon: " + afterChange.Id);

        // 9. Khong tim thay ung vien
        PrintTitle("9. Thu cap phat cho phong C999");
        try
        {
            airService.Allocate(a => a.IsCompatibleWith("C999"));
        }
        catch (NoSuitableItemException ex)
        {
            Console.WriteLine("Da bat loi: " + ex.Message);
        }

        // 10. Kiem tra trang thai va tra thiet bi
        PrintTitle("10. Kiem tra Borrowed va Return");
        Console.WriteLine($"{afterChange.Id}: {afterChange.Status}");
        afterChange.Return();
        Console.WriteLine($"{afterChange.Id} sau khi tra: {afterChange.Status}");

        // 11. Cung 1 repository generic cho nhieu loai
        PrintTitle("11. Kiem tra generic repository");
        Console.WriteLine($"ProjectorRepo: {projectorRepo.GetAll().Count} thiet bi");
        Console.WriteLine($"AirRepo: {airRepo.GetAll().Count} thiet bi");
        Console.WriteLine($"MicRepo: {micRepo.GetAll().Count} thiet bi");

        PrintTitle("Kiem tra them truong hop bien");
        var p20 = projectorRepo.FindById("P02");
        var m01 = micRepo.FindById("M01");
        var m03 = micRepo.FindById("M03");
        Console.WriteLine($"May chieu pin 20% san sang? {p20?.IsReadyForUse()}");
        Console.WriteLine($"Micro pin 30%, tin hieu 40% san sang? {m01?.IsReadyForUse()}");
        Console.WriteLine($"Diem uu tien micro M03: {m03?.GetDefaultPriorityScore()}");

        Console.WriteLine("\nHoan thanh cac buoc demo.");
    }

    static void PrintTitle(string title)
    {
        Console.WriteLine();
        Console.WriteLine("-----------------------------");
        Console.WriteLine(title);
    }
}
