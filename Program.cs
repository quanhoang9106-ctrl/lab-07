
using System;
using System.Text;

namespace LabW07;

internal class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine("LAB W07 - QUAN LY THIET BI PHONG HOC");

        // Tao ba kho luu tru
        IRepository<ProjectorRemote> projectorRepo =
            new InMemoryRepository<ProjectorRemote>();

        IRepository<AirConditionerRemote> airRepo =
            new InMemoryRepository<AirConditionerRemote>();

        IRepository<WirelessMicrophone> micRepo =
            new InMemoryRepository<WirelessMicrophone>();

        // TEST 1: Them thiet bi
        PrintTitle("1. Them thiet bi vao repository");

        projectorRepo.Add(new ProjectorRemote(
            "P01", "May chieu 1", "Kho A", 50));

        projectorRepo.Add(new ProjectorRemote(
            "P02", "May chieu 2", "Kho A", 20));

        projectorRepo.Add(new ProjectorRemote(
            "P03", "May chieu 3", "Kho B", 90));

        airRepo.Add(new AirConditionerRemote(
            "A01", "Dieu hoa 1", "Kho B",
            new[] { "A203", "A204" }));

        airRepo.Add(new AirConditionerRemote(
            "A02", "Dieu hoa 2", "Kho B",
            new[] { "B101", "B102" }));

        micRepo.Add(new WirelessMicrophone(
            "M01", "Micro 1", "Kho C", 30, 40));

        micRepo.Add(new WirelessMicrophone(
            "M02", "Micro 2", "Kho C", 75, 60));

        micRepo.Add(new WirelessMicrophone(
            "M03", "Micro 3", "Kho C", 55, 72));

        int total = projectorRepo.GetAll().Count
                  + airRepo.GetAll().Count
                  + micRepo.GetAll().Count;

        PrintCheck("Da them du 8 thiet bi", total == 8);

        // TEST 2: Trung ma thiet bi
        PrintTitle("2. Kiem tra trung ID");

        bool duplicateCaught = false;

        try
        {
            projectorRepo.Add(new ProjectorRemote(
                "P01", "May chieu khac", "Kho A", 80));
        }
        catch (DuplicateIdException ex)
        {
            duplicateCaught = true;
            Console.WriteLine(ex.Message);
        }

        PrintCheck("Phat hien ID trung", duplicateCaught);

        // TEST 3: Tim thiet bi theo ma
        PrintTitle("3. Tim kiem thiet bi");

        ProjectorRemote? found = projectorRepo.FindById("P02");

        if (found != null)
        {
            Console.WriteLine(found);
        }

        PrintCheck("Tim thay P02", found != null && found.Id == "P02");
        PrintCheck("Ma khong ton tai tra ve null",
            projectorRepo.FindById("P99") == null);

        // TEST 4: Loc thiet bi san sang
        PrintTitle("4. Loc cac may chieu san sang");

        var readyProjectors =
            projectorRepo.FindAll(p => p.IsReadyForUse());

        foreach (ProjectorRemote p in readyProjectors)
        {
            Console.WriteLine($"{p.Id} - Pin: {p.BatteryLevel}%");
        }

        PrintCheck("Co 3 may chieu san sang",
            readyProjectors.Count == 3);

        // TEST 5: Chon may chieu diem cao nhat
        PrintTitle("5. Chon may chieu co diem cao nhat");

        var projectorStrategy =
            new HighestPriorityStrategy<ProjectorRemote>(
                p => p.GetDefaultPriorityScore());

        var projectorService =
            new AllocationService<ProjectorRemote>(
                projectorRepo, projectorStrategy);

        ProjectorRemote selectedProjector =
            projectorService.Allocate(p => true);

        Console.WriteLine($"Da chon: {selectedProjector.Id}");

        PrintCheck("May chieu duoc chon la P03",
            selectedProjector.Id == "P03");

        // TEST 6: Chon dieu hoa theo phong
        PrintTitle("6. Chon dieu hoa cho phong A203");

        var airStrategy =
            new HighestPriorityStrategy<AirConditionerRemote>(
                a => a.GetDefaultPriorityScore());

        var airService =
            new AllocationService<AirConditionerRemote>(
                airRepo, airStrategy);

        AirConditionerRemote selectedAir =
            airService.Allocate(a => a.IsCompatibleWith("A203"));

        Console.WriteLine($"Da chon: {selectedAir.Id}");

        PrintCheck("Dieu hoa tuong thich phong A203",
            selectedAir.Id == "A01");

        // TEST 7: Chon micro dau tien san sang
        PrintTitle("7. Chon micro dau tien san sang");

        var micStrategy =
            new FirstAvailableStrategy<WirelessMicrophone>();

        var micService =
            new AllocationService<WirelessMicrophone>(
                micRepo, micStrategy);

        WirelessMicrophone selectedMic =
            micService.Allocate(m => true);

        Console.WriteLine($"Da chon: {selectedMic.Id}");

        PrintCheck("Micro dau tien la M01",
            selectedMic.Id == "M01");

        // TEST 8: Thay doi strategy
        PrintTitle("8. Thay doi chien luoc");

        selectedProjector.Return();

        projectorService.ChangeStrategy(
            new FirstAvailableStrategy<ProjectorRemote>());

        ProjectorRemote selectedAfterChange =
            projectorService.Allocate(p => true);

        Console.WriteLine(
            $"Sau khi doi strategy: {selectedAfterChange.Id}");

        PrintCheck("Chien luoc moi chon P01",
            selectedAfterChange.Id == "P01");

        // TEST 9: Khong co thiet bi phu hop
        PrintTitle("9. Khong tim thay thiet bi phu hop");

        bool noItemCaught = false;

        try
        {
            airService.Allocate(
                a => a.IsCompatibleWith("C999"));
        }
        catch (NoSuitableItemException ex)
        {
            noItemCaught = true;
            Console.WriteLine(ex.Message);
        }

        PrintCheck("Bat loi khi khong co ung vien",
            noItemCaught);

        // TEST 10: Kiem tra Borrow va Return
        PrintTitle("10. Kiem tra trang thai thiet bi");

        Console.WriteLine(
            $"Truoc khi tra: {selectedAfterChange.Status}");

        PrintCheck("Trang thai da chuyen sang Borrowed",
            selectedAfterChange.Status == DeviceStatus.Borrowed);

        selectedAfterChange.Return();

        Console.WriteLine(
            $"Sau khi tra: {selectedAfterChange.Status}");

        PrintCheck("Trang thai tro ve Available",
            selectedAfterChange.Status == DeviceStatus.Available);

        // TEST 11: Kiem tra generic repository
        PrintTitle("11. Kiem tra generic repository");

        Console.WriteLine(
            $"Projector repository: {projectorRepo.GetAll().Count}");

        Console.WriteLine(
            $"Air repository: {airRepo.GetAll().Count}");

        Console.WriteLine(
            $"Micro repository: {micRepo.GetAll().Count}");

        PrintCheck("Cung repository dung cho 3 loai",
            projectorRepo.GetAll().Count == 3
            && airRepo.GetAll().Count == 2
            && micRepo.GetAll().Count == 3);

        // KIEM TRA THEM CAC TRUONG HOP BIEN
        PrintTitle("12. Kiem tra logic o cac nguong");

        // Tao doi tuong rieng de khong bi anh huong
        // boi trang thai muon thiet bi o cac test tren
        ProjectorRemote testProjector =
            new ProjectorRemote("P20", "Test pin 20", "Kho A", 20);

        WirelessMicrophone testMic =
            new WirelessMicrophone("M40", "Test micro", "Kho C", 30, 40);

        WirelessMicrophone scoreMic =
            new WirelessMicrophone("M55", "Test diem", "Kho C", 55, 72);

        PrintCheck("May chieu pin 20% san sang",
            testProjector.IsReadyForUse());

        PrintCheck("Micro pin 30%, tin hieu 40% san sang",
            testMic.IsReadyForUse());

        PrintCheck("Diem uu tien micro bang 63.5",
            Math.Abs(scoreMic.GetDefaultPriorityScore() - 63.5) < 0.0001);

        // Kiem tra muon thiet bi khong du dieu kien
        bool notReadyCaught = false;

        ProjectorRemote lowBattery =
            new ProjectorRemote("P10", "May chieu pin yeu", "Kho A", 10);

        try
        {
            lowBattery.Borrow();
        }
        catch (DeviceNotReadyException ex)
        {
            notReadyCaught = true;
            Console.WriteLine(ex.Message);
        }

        PrintCheck("Khong cho muon thiet bi pin yeu",
            notReadyCaught);

        // Kiem tra xoa ma khong ton tai
        PrintCheck("Remove tra false khi khong tim thay",
            projectorRepo.Remove("P99") == false);

        Console.WriteLine();
        Console.WriteLine("HOAN THANH KIEM THU LAB W07");
    }

    static void PrintTitle(string title)
    {
        Console.WriteLine();
        Console.WriteLine("-----------------------------------");
        Console.WriteLine(title);
        Console.WriteLine("-----------------------------------");
    }

    static void PrintCheck(string message, bool result)
    {
        if (result)
        {
            Console.WriteLine("[PASS] " + message);
        }
        else
        {
            Console.WriteLine("[FAIL] " + message);
        }
    }
}
