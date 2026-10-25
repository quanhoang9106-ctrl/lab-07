
using System;
using System.Text;

namespace LabW07;

internal class Program
{
    private static int total = 0;
    private static int passed = 0;

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("=== LAB W07 - QUAN LY THIET BI ===");

        // Kho luu tru cho 3 loai thiet bi
        IRepository<ProjectorRemote> projectorRepo =
            new InMemoryRepository<ProjectorRemote>();

        IRepository<AirConditionerRemote> airRepo =
            new InMemoryRepository<AirConditionerRemote>();

        IRepository<WirelessMicrophone> micRepo =
            new InMemoryRepository<WirelessMicrophone>();

        // 1. Tao va them thiet bi
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

        Check("1. Them du 8 thiet bi",
            projectorRepo.GetAll().Count +
            airRepo.GetAll().Count +
            micRepo.GetAll().Count == 8);

        // 2. Kiem tra trung ID
        Check("2. Phat hien ID trung",
            Throws<DuplicateIdException>(() =>
                projectorRepo.Add(new ProjectorRemote(
                    "P01", "May chieu khac", "Kho A", 80))));

        // 3. Tim kiem theo ID
        Check("3. Tim thiet bi P02",
            projectorRepo.FindById("P02")?.BatteryLevel == 20);

        // 4. Loc thiet bi san sang
        Check("4. Loc may chieu san sang",
            projectorRepo.FindAll(p => p.IsReadyForUse()).Count == 3);

        // 5. Chon may chieu co diem cao nhat
        var projectorService = new AllocationService<ProjectorRemote>(
            projectorRepo,
            new HighestPriorityStrategy<ProjectorRemote>(
                p => p.GetDefaultPriorityScore()));

        ProjectorRemote selectedProjector = projectorService.Allocate();

        Check("5. Chon may chieu pin cao nhat",
            selectedProjector.Id == "P03");

        // 6. Chon dieu hoa phu hop phong A203
        var airService = new AllocationService<AirConditionerRemote>(
            airRepo,
            new HighestPriorityStrategy<AirConditionerRemote>(
                a => a.GetDefaultPriorityScore()));

        AirConditionerRemote selectedAir =
            airService.Allocate(a => a.IsCompatibleWith("A203"));

        Check("6. Chon dieu hoa cho A203",
            selectedAir.Id == "A01");

        // 7. Chon micro dau tien san sang
        var micService = new AllocationService<WirelessMicrophone>(
            micRepo,
            new FirstAvailableStrategy<WirelessMicrophone>());

        WirelessMicrophone selectedMic = micService.Allocate();

        Check("7. Chon micro dau tien",
            selectedMic.Id == "M01");

        // 8. Thay doi chien luoc tai runtime
        selectedProjector.Return();

        projectorService.ChangeStrategy(
            new FirstAvailableStrategy<ProjectorRemote>());

        ProjectorRemote firstProjector = projectorService.Allocate();

        Check("8. Doi strategy thanh cong",
            firstProjector.Id == "P01");

        // 9. Khong co ung vien phu hop
        Check("9. Phat hien khong co thiet bi",
            Throws<NoSuitableItemException>(() =>
                airService.Allocate(a => a.IsCompatibleWith("C999"))));

        // 10. Muon va tra thiet bi
        bool borrowed = firstProjector.Status == DeviceStatus.Borrowed;

        firstProjector.Return();

        Check("10. Borrow va Return dung",
            borrowed &&
            firstProjector.Status == DeviceStatus.Available);

        // 11. Generic repository dung cho nhieu loai
        Check("11. Repository hoat dong voi 3 loai",
            projectorRepo.GetAll().Count == 3 &&
            airRepo.GetAll().Count == 2 &&
            micRepo.GetAll().Count == 3);

        // Kiem tra them cac truong hop bien
        Console.WriteLine("\n=== KIEM TRA BO SUNG ===");

        Check("May chieu pin 20% san sang",
            projectorRepo.FindById("P02")!.IsReadyForUse());

        var boundaryMic = new WirelessMicrophone(
            "TEST01", "Test Micro", "Kho C", 30, 40);

        Check("Micro pin 30%, tin hieu 40% san sang",
            boundaryMic.IsReadyForUse());

        Check("Diem micro M03 bang 63.5",
            micRepo.FindById("M03")!.GetDefaultPriorityScore() == 63.5);

        Check("Khong cho muon thiet bi pin yeu",
            Throws<DeviceNotReadyException>(() =>
                new ProjectorRemote(
                    "TEST02", "Pin yeu", "Kho A", 10).Borrow()));

        Check("Khong cho muon thiet bi hai lan",
            Throws<DeviceNotReadyException>(() => selectedAir.Borrow()));

        Check("Khong cho tra thiet bi chua muon",
            Throws<InvalidOperationException>(() =>
                new ProjectorRemote(
                    "TEST03", "Thiet bi moi", "Kho A", 50).Return()));

        Check("Xu ly gia tri pin khong hop le",
            Throws<ArgumentOutOfRangeException>(() =>
                new ProjectorRemote(
                    "TEST04", "Pin sai", "Kho A", 101)));

        Check("Tim ID khong ton tai tra null",
            projectorRepo.FindById("P99") == null);

        Check("Dieu hoa khong phan biet chu hoa",
            selectedAir.IsCompatibleWith("a203"));

        Check("Strategy khong chap nhan danh sach rong",
            Throws<NoSuitableItemException>(() =>
                new FirstAvailableStrategy<WirelessMicrophone>()
                    .Select(Array.Empty<WirelessMicrophone>())));

        // Kiem tra tinh an toan cua repository
        var snapshot = projectorRepo.GetAll();
        bool removed = projectorRepo.Remove("P02");

        Check("Xoa thiet bi theo ID",
            removed && projectorRepo.FindById("P02") == null);

        Check("GetAll khong lo collection noi bo",
            snapshot.Count == 3 &&
            projectorRepo.GetAll().Count == 2);

        Check("Remove tra false neu ID khong ton tai",
            !projectorRepo.Remove("P99"));

        Check("ChangeStrategy khong nhan null",
            Throws<ArgumentNullException>(() =>
                projectorService.ChangeStrategy(null!)));

        Console.WriteLine("\n==============================");
        Console.WriteLine($"KET QUA: {passed}/{total} TEST PASS");
        Console.WriteLine("==============================");

        Environment.ExitCode = passed == total ? 0 : 1;
    }

    static void Check(string message, bool condition)
    {
        total++;

        if (condition)
            passed++;

        Console.WriteLine(
            $"[{(condition ? "PASS" : "FAIL")}] {message}");
    }

    static bool Throws<TException>(Action action)
        where TException : Exception
    {
        try
        {
            action();
            return false;
        }
        catch (TException)
        {
            return true;
        }
    }
}
