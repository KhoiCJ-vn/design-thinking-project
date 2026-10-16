# 🎓 CỔNG THÔNG TIN SINH VIÊN UEH & TÍNH NĂNG UEH MY WEEK
> **Đồ án Prototype môn học / Báo cáo thực hành Full-Stack**  
> **Khoa / Ngành:** Công nghệ thông tin kinh doanh — Đại học Kinh tế TP. Hồ Chí Minh (UEH)  
> **Công nghệ sử dụng:** ASP.NET Core 8.0 Web API • Entity Framework Core (Database-First) • Microsoft SQL Server 2025 • HTML5/CSS3/JavaScript thuần.

---

## 👨‍🏫 DÀNH CHO GIẢNG VIÊN: HƯỚNG DẪN CHẤM ĐIỂM & TEST PROTOTYPE

Kính gửi Quý Thầy/Cô, dưới đây là hướng dẫn nhanh để Thầy/Cô có thể chạy và kiểm thử toàn bộ các luồng nghiệp vụ của hệ thống trong vòng 3 phút.

### 1. Chuẩn bị môi trường (Yêu cầu tối thiểu)
- Máy tính có cài **.NET 8.0 SDK** (hoặc mới hơn).
- **Microsoft SQL Server** (bản Developer, Express hoặc LocalDB) đang chạy tại `localhost`.

### 2. Khởi tạo Database (Chỉ chạy 1 lần duy nhất)
Mở cửa sổ Terminal (PowerShell hoặc CMD) tại thư mục gốc của dự án và chạy 3 lệnh sau:

```powershell
# Bước 1: Tạo Database UEHStudentDB
sqlcmd -S localhost -E -C -i "database\01_CreateDatabase.sql"

# Bước 2: Tạo toàn bộ 14 bảng, ràng buộc khóa chính, khóa ngoại và indexes
sqlcmd -S localhost -E -C -i "database\02_CreateSchema.sql"

# Bước 3: Nạp dữ liệu mẫu thực tế của học kỳ
sqlcmd -S localhost -E -C -i "database\03_SeedDemoData.sql"
```

### 3. Khởi chạy Ứng dụng
Chạy lệnh sau tại thư mục gốc:

```powershell
dotnet run --project src/UEHStudent.Api/UEHStudent.Api.csproj --launch-profile http
```

Mở trình duyệt web truy cập: **[http://localhost:5000/login.html](http://localhost:5000/login.html)** (hoặc [http://localhost:5000/](http://localhost:5000/)).  
Trang tài liệu Swagger API: **[http://localhost:5000/swagger](http://localhost:5000/swagger)**.

---

## 🔑 DANH SÁCH TÀI KHOẢN TRẢI NGHIỆM
*(Trên trang đăng nhập có sẵn **các nút 1-click** để Thầy/Cô tự động điền mà không cần gõ phím)*

| Vai trò | Tên đăng nhập | Mật khẩu | Chức năng kiểm thử chính |
|---|---|---|---|
| **Sinh viên 1** | `nguyenminhkhoi` | `Student1` | Trải nghiệm UEH My Week, TKB 4 môn học, Đăng ký sự kiện, Quản lý task cá nhân, Lưu thông báo |
| **Sinh viên 2** | `nguyennhatthang` | `Student2` | Xem lịch học riêng (3 môn), kiểm tra tính cách ly dữ liệu cá nhân giữa các sinh viên |
| **Quản trị viên** | `admin` | `Admin@123` | Quản lý và CRUD Lịch học, Lịch thi, Thông báo và Sự kiện tại trang `/admin.html` |

---

## 📋 KỊCH BẢN KIỂM THỬ GỢI Ý (5 BƯỚC TEST NHANH)

### Bước 1: Đăng nhập Sinh viên
1. Bấm vào nút **🎓 SV Nguyễn Minh Khôi** (hoặc nhập `nguyenminhkhoi` / `Student1`) ➔ Bấm **Đăng nhập hệ thống**.
2. Hệ thống chuyển vào giao diện chính mang phong cách đặc trưng của Cổng thông tin UEH Student (Màu Teal đậm `#005863`, Cam UEH `#E85A0C`, nền trắng).

### Bước 2: Trải nghiệm tính năng trung tâm — UEH My Week
1. **Dòng thời gian tuần (Weekly Timeline):** Hiển thị rõ ràng các buổi học, sự kiện đã đăng ký và công việc cá nhân trong tuần từ Thứ Hai đến Chủ Nhật.
2. **Chọn tuần bất kỳ:** 
   - Nhấp vào ô **"Chọn ngày bất kỳ"** và chọn ngày `28/11/2026`.
   - Hệ thống tự động chuyển sang tuần thi `23/11/2026 – 29/11/2026`, xuất hiện **3 kỳ thi cuối kỳ** có nhãn đỏ nổi bật.
   - Bấm nút **"Tuần hiện tại (16/10)"** để quay lại tuần học thông thường.

### Bước 3: Tương tác tức thì (Không cần tải lại trang)
1. **Đăng ký sự kiện UEH:** Tại cột bên phải (khu vực *Hoạt động UEH trong tuần*), tìm sự kiện *UEH Career Fair 2026* và bấm **Đăng ký**. 
   - Sự kiện ngay lập tức được thêm vào dòng thời gian tuần cá nhân của sinh viên.
   - Bấm **Hủy** để gỡ khỏi lịch cá nhân.
2. **Thêm việc cá nhân (Personal Task):** Bấm nút **+ Thêm việc cá nhân**, nhập tiêu đề và ngày dự kiến ➔ Việc cá nhân xuất hiện ngay trên lịch tuần; có thể tích checkbox hoàn thành hoặc xóa.
3. **Lưu thông báo vào lịch:** Tại khu vực *Thông báo cần chú ý*, chọn một thông báo quan trọng ➔ Bấm **+ Thêm vào lịch** và chọn ngày nhắc nhở.

### Bước 4: Kiểm tra Thời khóa biểu ma trận & Lịch thi
1. Chuyển sang tab **Thời khóa biểu**: Tái hiện chính xác bố cục lưới 13 tiết x 7 thứ của UEH.
2. Chuyển sang tab **Lịch thi**: Hiển thị bảng chi tiết môn thi, phòng thi, hình thức thi kèm quy chế phòng thi.
3. Chuyển sang tab **Thông báo chung**: Tìm kiếm thông báo theo từ khóa và bộ lọc chuyên mục.

### Bước 5: Kiểm tra Cơ chế Đồng bộ Tự động từ Quản trị viên (Admin)
1. Bấm **Đăng xuất** ➔ Đăng nhập bằng tài khoản **🛡️ Quản trị viên** (`admin` / `Admin@123`).
2. Vào mục **Quản lý Lịch học**, thêm một buổi học mới cho lớp *28C1TEC5500183B01 — Tư duy thiết kế* vào **Thứ Bảy**, từ `08:00` đến `11:00`.
3. Đăng xuất và đăng nhập lại tài khoản Sinh viên 1: Buổi học mới vừa tạo **lập tức xuất hiện trên cả trang Thời khóa biểu và UEH My Week**, chứng minh dữ liệu được đồng bộ 100% từ cơ sở dữ liệu thật.

---

## 🛠️ HƯỚNG DẪN DÀNH CHO SINH VIÊN: TỰ CHỈNH SỬA DATABASE

Nếu bạn muốn thay đổi họ tên sinh viên, mã số sinh viên, tên đăng nhập hoặc mật khẩu:

### 1. Database ở đâu?
- **Tên Database:** `UEHStudentDB`
- **Hệ quản trị:** Microsoft SQL Server (kết nối qua `localhost`, Windows Authentication hoặc SQL Authentication).
- **Công cụ mở:** SQL Server Management Studio (SSMS), Azure Data Studio, hoặc tiện ích mở rộng mssql trong VS Code.

### 2. Các bảng cần chỉnh sửa:
1. Bảng `Students`: Lưu hồ sơ sinh viên (Họ tên, Mã SV, Khoa, Ngành).
2. Bảng `AppUsers`: Lưu tài khoản đăng nhập (Username, Mật khẩu, Vai trò, Liên kết `StudentId`).

### 3. Câu lệnh SQL để bạn tự sửa nhanh:

```sql
USE [UEHStudentDB];
GO

-- 1. Sửa thông tin hồ sơ Sinh viên 1
UPDATE [dbo].[Students]
SET [FullName] = N'Nguyễn Minh Khôi',
    [StudentCode] = N'31251022130'
WHERE [StudentId] = 1;

-- 2. Sửa thông tin hồ sơ Sinh viên 2
UPDATE [dbo].[Students]
SET [FullName] = N'Nguyễn Nhựt Thắng',
    [StudentCode] = N'31251020845'
WHERE [StudentId] = 2;

-- 3. Sửa tài khoản đăng nhập và mật khẩu
-- (Hệ thống hỗ trợ gõ mật khẩu trực tiếp dạng chữ như 'Student1', 'Student2' và tự động mã hóa bảo mật khi đăng nhập)
UPDATE [dbo].[AppUsers]
SET [Username] = 'nguyenminhkhoi',
    [PasswordHash] = 'Student1'
WHERE [UserId] = 2;

UPDATE [dbo].[AppUsers]
SET [Username] = 'nguyennhatthang',
    [PasswordHash] = 'Student2'
WHERE [UserId] = 3;

-- 4. Kiểm tra lại dữ liệu sau khi sửa
SELECT u.UserId, u.Username, u.RoleName, s.StudentCode, s.FullName 
FROM AppUsers u 
LEFT JOIN Students s ON u.StudentId = s.StudentId;
GO
```

---

## 🚢 2 CÁCH GỬI BÀI ĐỂ THẦY CÔ CHẤM ĐIỂM

### CÁCH 1: Gửi đường link Online trực tiếp (KHUYÊN DÙNG NHẤT ⭐)
Để Thầy/Cô chỉ cần mở điện thoại hoặc máy tính bấm vào link là dùng được ngay, không cần cài .NET hay SQL Server:

1. Giữ cho ứng dụng của bạn đang chạy ở port 5000.
2. Mở một cửa sổ PowerShell mới và chạy công cụ tạo đường link công khai (miễn phí, không cần đăng ký):
   ```powershell
   winget install Cloudflare.cloudflared
   cloudflared tunnel --url http://localhost:5000
   ```
3. Màn hình sẽ sinh ra một đường link dạng:
   `https://random-subdomain.trycloudflare.com`
4. Bạn gửi đường link này cùng file Word/PDF báo cáo cho Thầy. Thầy bấm vào là vào thẳng trang web và chấm điểm ngay lập tức!

### CÁCH 2: Nộp file mã nguồn (File ZIP hoặc Git Repository)
1. Nén toàn bộ thư mục dự án (đã có sẵn file `.gitignore` để loại bỏ các thư mục rác `bin/`, `obj/`).
2. Trong file nộp kèm, gửi link kèm theo hướng dẫn:
   - *"Kính gửi Thầy, hướng dẫn cài đặt và kịch bản test chi tiết đã được trình bày đầy đủ trong file `README.md` của thư mục đồ án."*
   - Thầy mở máy, chạy 3 script trong thư mục `database/`, chạy `dotnet run` và mở trình duyệt.

---

## 🏛️ CẤU TRÚC DỮ LIỆU ĐỒ ÁN (14 BẢNG CHUẨN HÓA)
1. `AcademicTerms`: Niên khóa và học kỳ.
2. `Courses`: Danh mục môn học.
3. `ClassSections`: Lớp học phần thực tế.
4. `Students`: Hồ sơ sinh viên.
5. `AppUsers`: Tài khoản bảo mật (phân quyền Admin / Student).
6. `StudentCourseEnrollments`: Danh sách môn sinh viên đang theo học.
7. `ClassScheduleRules`: Quy tắc lịch học định kỳ trong học kỳ.
8. `ExamSchedules`: Lịch thi cuối học phần.
9. `Announcements`: Thông báo chung của trường.
10. `AnnouncementTargets`: Đối tượng tiếp nhận thông báo.
11. `Events`: Sự kiện, hội thảo UEH.
12. `EventRegistrations`: Sinh viên đăng ký tham gia sự kiện.
13. `PersonalTasks`: Quản lý việc cá nhân của sinh viên.
14. `StudentSavedAnnouncements`: Thông báo sinh viên lưu vào lịch theo dõi.

---
*Dự án hoàn thành đầy đủ 100% các tiêu chí kỹ thuật: Database-First, SQL Server thật, RESTful Web API, JWT Bearer Token, BCrypt Password Security và Giao diện UEH My Week Responsive.*
