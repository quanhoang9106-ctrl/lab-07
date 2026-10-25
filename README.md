
# Lab W07 - Quan ly thiet bi phong hoc

## 1. Gioi thieu
Chuong trinh quan ly va cap phat thiet bi cho phong hoc
duoc xay dung bang C# theo phuong phap lap trinh huong doi tuong.

He thong quan ly 3 loai thiet bi:
- Dieu khien may chieu
- Dieu khien dieu hoa
- Micro khong day

## 2. Chuc nang
- Them, xoa va tim kiem thiet bi
- Kiem tra thiet bi san sang
- Loc thiet bi theo dieu kien
- Chon thiet bi theo chien luoc
- Muon va tra thiet bi
- Xu ly cac truong hop ngoai le

## 3. Cau truc chuong trinh
- Device.cs: Lop thiet bi va ke thua
- Repository.cs: Generic repository
- SelectionStrategy.cs: Chien luoc lua chon
- AllocationService.cs: Dich vu cap phat
- Exceptions.cs: Xu ly ngoai le
- Program.cs: Chay thu cac tinh huong
- LabW07_ClassDiagram.drawio: So do UML

## 4. Kien thuc OOP su dung
- Abstraction
- Encapsulation
- Inheritance
- Polymorphism
- Interface
- Generic
- Strategy Pattern

## 5. Quyet dinh thiet ke
Chuong trinh su dung List<T> de luu tru thiet bi.

Uu diem:
- De hieu
- De cai dat
- De duyet va xu ly theo thu tu

Nhuoc diem:
- Tim kiem theo ID co do phuc tap O(n)
- Khi so luong thiet bi lon, hieu nang chua tot

Co the thay the bang Dictionary<string, T> de
cai thien toc do tim kiem.

Voi dieu khien dieu hoa, phong tuong thich
duoc loc trong Allocate() truoc khi chon thiet bi.

## 6. Cach chay
Mo thu muc du an bang VS Code.

Trong Terminal, nhap:

dotnet restore
dotnet build
dotnet run

## 7. Kiem thu
Chuong trinh kiem tra 11 tinh huong theo de bai
va cac truong hop bien ve pin, tin hieu, diem uu tien.

Ket qua duoc hien thi tren Terminal bang PASS/FAIL.

## 8. Trang thai
Phien ban da sua cac loi logic da phat hien.
Chuong trinh van su dung List<T> va vong lap don gian,
chua toi uu ve hieu nang.
