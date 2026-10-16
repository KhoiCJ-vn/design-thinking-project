# 🎓 UEH Student Portal & Tính năng trọng tâm UEH My Week

Dự án tái xây dựng cổng thông tin sinh viên UEH Student với tính năng trung tâm **UEH My Week** — Giải pháp tích hợp lịch học, lịch thi, sự kiện hoạt động UEH và việc cá nhân trên một giao diện tuần trực quan, đồng bộ trực tiếp từ cơ sở dữ liệu Microsoft SQL Server thông qua ASP.NET Core Web API và Entity Framework Core theo quy trình **Database-First**.

---

## 📌 1. Tổng quan Kiến trúc & Công nghệ

| Tầng | Công nghệ / Thư viện | Vai trò |
|---|---|---|
| **Backend API** | ASP.NET Core 8.0 Web API | RESTful API, Dependency Injection, Middleware |
| **ORM** | EF Core 8.0 (Database-First) | Scaffold Entities & DbContext trực tiếp từ SQL Server |
| **Database** | Microsoft SQL Server 2025 | Cơ sở dữ liệu quan hệ gồm 14 bảng, ràng buộc & index |
| **Xác thực** | JWT Bearer Token + BCrypt | Phân quyền vai trò: `Student` & `Admin` |
| **Frontend** | HTML5, CSS3, JavaScript thuần | Single Page Application phục vụ từ `wwwroot` (cùng origin) |
| **Màu sắc & Nhận diện** | Dark Teal (`#005863`), Cam UEH (`#E85A0C`), Trắng (`#FFFFFF`) | Chuẩn nhận diện UEH Student Portal |

---

## 🏛️ 2. Cấu trúc Thư mục Dự án

```text
TDTK_Project/
├── database/
│   ├── 01_CreateDatabase.sql        # Script tạo database UEHStudentDB
│   ├── 02_CreateSchema.sql          # 14 bảng, khóa chính, khóa ngoại, unique & index
│   └── 03_SeedDemoData.sql          # Dữ liệu minh họa (LHP, TKB, Lịch thi, Thông báo, Sự kiện)
│
├── src/
│   └── UEHStudent.Api/
│       ├── Controllers/
│       │   ├── AuthController.cs            # Đăng nhập & hồ sơ cá nhân
│       │   ├── MyWeekController.cs          # Trung tâm UEH My Week
│       │   ├── SchedulesController.cs       # Thời khóa biểu theo tuần
│       │   ├── ExamsController.cs           # Lịch thi sinh viên
│       │   ├── AnnouncementsController.cs   # Thông báo chung & tìm kiếm
│       │   ├── EventsController.cs          # Sự kiện & Đăng ký sự kiện
│       │   ├── PersonalTasksController.cs   # Quản lý việc cá nhân
│       │   ├── SavedAnnouncementsController.cs # Đưa thông báo vào lịch
│       │   ├── HealthController.cs          # Kiểm tra kết nối SQL Server
│       │   ├── StudentsController.cs        # Đọc dữ liệu sinh viên
│       │   └── Admin/                       # Phân quyền [Authorize(Roles="Admin")]
│       │       ├── AdminSchedulesController.cs
│       │       ├── AdminExamsController.cs
│       │       ├── AdminAnnouncementsController.cs
│       │       └── AdminEventsController.cs
│       ├── Services/
│       │   ├── DateTimeHelper.cs            # Tính thứ Hai - Chủ Nhật cho bất kỳ ngày nào
│       │   ├── IScheduleService.cs & ScheduleService.cs # Mở rộng quy tắc lịch học
│       │   └── IMyWeekService.cs & MyWeekService.cs     # Tổng hợp dữ liệu My Week
│       ├── Security/
│       │   ├── JwtService.cs                # Ký và xác thực JWT Token
│       │   ├── PasswordHasher.cs            # Băm mật khẩu an toàn bằng BCrypt
│       │   └── ClaimsPrincipalExtensions.cs # Trích xuất StudentId an toàn từ Claims
│       ├── Data/
│       │   ├── Scaffolded/                  # Sinh tự động bằng EF Core Database-First
│       │   │   ├── UEHStudentDbContext.cs
│       │   │   └── [14 Entity Classes]
│       │   └── DbInitializer.cs             # Khởi tạo hash mật khẩu an toàn lúc chạy
│       ├── Middleware/
│       │   └── ErrorHandlingMiddleware.cs   # Bắt lỗi toàn cục, ẩn stack trace
│       ├── DTOs/                            # Data Transfer Objects
│       ├── wwwroot/                         # Frontend website
│       │   ├── index.html                   # Giao diện chính UEH Student & My Week
│       │   ├── login.html                   # Giao diện đăng nhập
│       │   ├── admin.html                   # Giao diện quản trị Admin
│       │   ├── css/styles.css               # Phong cách UEH
│       │   └── js/                          # Các module JavaScript
│       │       ├── api-client.js
│       │       ├── app.js
│       │       ├── my-week.js
│       │       ├── schedule.js
│       │       ├── exams.js
│       │       ├── announcements.js
│       │       └── admin.js
│       ├── Program.cs
│       ├── appsettings.json
│       └── appsettings.Development.json
│
├── reference/                               # Ảnh tham chiếu giao diện gốc
├── .gitignore
└── README.md
```

---

## 🗄️ 3. Cơ sở Dữ liệu & Thiết kế Schema (14 Bảng)

1. `AcademicTerms`: Năm học và học kỳ (`2026-2027`, `Học kỳ cuối`).
2. `Courses`: Danh mục học phần (Mã môn, Tên môn, Số tín chỉ).
3. `ClassSections`: Lớp học phần thực tế (Mã LHP, Nhóm, Số lượng tối đa).
4. `Students`: Hồ sơ sinh viên (Mã SV, Họ tên, Khoa, Ngành, Khóa).
5. `AppUsers`: Tài khoản đăng nhập (Username duy nhất, PasswordHash BCrypt, Role `Admin`/`Student`).
6. `StudentCourseEnrollments`: Đăng ký lớp học phần của sinh viên (ngăn trùng lặp bằng unique index).
7. `ClassScheduleRules`: Quy tắc lịch học định kỳ (`DayOfWeek`, `StartTime`, `EndTime`, `EffectiveStartDate`, `EffectiveEndDate`, `Room`, `Instructor`).
8. `ExamSchedules`: Lịch thi cuối học phần (`ExamDate`, `StartTime`, `EndTime`, `Room`, `Location`, `ExamForm`).
9. `Announcements`: Thông báo chính thức của trường (`Title`, `Content`, `Issuer`, `Priority`, `DeadlineAt`, `IsActive`).
10. `AnnouncementTargets`: Đối tượng nhận thông báo (`All`, `Faculty`, `Cohort`).
11. `Events`: Sự kiện UEH (`StartDateTime`, `EndDateTime`, `RegistrationDeadline`, `MaxParticipants`).
12. `EventRegistrations`: Sinh viên đăng ký tham gia sự kiện (ngăn trùng lặp bằng unique index).
13. `PersonalTasks`: Công việc cá nhân sinh viên thêm vào (`PlannedDate`, `PlannedTime`, `DueDate`, `IsCompleted`).
14. `StudentSavedAnnouncements`: Liên kết thông báo sinh viên lưu vào lịch cá nhân kèm ngày dự kiến theo dõi (`PlannedAt`).

---

## ⚙️ 4. Hướng dẫn Cài đặt & Chạy Dự án trong VS Code

### 4.1. Yêu cầu Môi trường
- .NET 8.0 SDK (đã kiểm tra: `8.0.426`).
- Microsoft SQL Server 2019/2022/2025 đang chạy trên `localhost`.
- Công cụ `sqlcmd` hoặc SQL Server Management Studio (SSMS).

### 4.2. Khởi tạo Cơ sở Dữ liệu (Quy trình Database-First)
Mở terminal trong thư mục dự án và chạy lần lượt 3 script:

```powershell
# 1. Tạo Database
sqlcmd -S localhost -E -C -i "database\01_CreateDatabase.sql"

# 2. Tạo 14 bảng, ràng buộc và Index
sqlcmd -S localhost -E -C -i "database\02_CreateSchema.sql"

# 3. Nạp dữ liệu minh họa học kỳ cuối 2026-2027
sqlcmd -S localhost -E -C -i "database\03_SeedDemoData.sql"
```

### 4.3. Scaffold Entity Framework Core (Nếu muốn sinh lại)
Lệnh scaffold Database-First đã được thực thi sẵn trong dự án:

```powershell
dotnet ef dbcontext scaffold "Server=localhost;Database=UEHStudentDB;Trusted_Connection=True;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer --project src/UEHStudent.Api/UEHStudent.Api.csproj --output-dir Data/Scaffolded --context UEHStudentDbContext --context-dir Data/Scaffolded --no-onconfiguring --force
```

> [!NOTE]
> Cờ `--no-onconfiguring` đảm bảo connection string **không bị nhúng cứng** vào mã nguồn sinh tự động, mà được đọc an toàn qua Dependency Injection từ `appsettings.Development.json` hoặc User Secrets.

### 4.4. Khởi chạy Ứng dụng
```powershell
dotnet run --project src/UEHStudent.Api/UEHStudent.Api.csproj --launch-profile http
```

Sau khi chạy:
- **Cổng thông tin sinh viên & My Week:** [http://localhost:5000/](http://localhost:5000/)
- **Trang đăng nhập:** [http://localhost:5000/login.html](http://localhost:5000/login.html)
- **Trang quản trị (Admin):** [http://localhost:5000/admin.html](http://localhost:5000/admin.html)
- **Tài liệu Swagger API:** [http://localhost:5000/swagger](http://localhost:5000/swagger)

---

## 🔑 5. Tài khoản Trải nghiệm & Phân quyền

Hệ thống có sẵn tính năng **1-click điền thông tin** ngay trên trang [login.html](http://localhost:5000/login.html):

| Vai trò | Tên đăng nhập | Mật khẩu | Quyền hạn |
|---|---|---|---|
| **Sinh viên 1** | `tranvanhoang` | `Student@123` | Xem My Week, TKB (4 môn), Lịch thi, Đăng ký sự kiện, Quản lý task cá nhân, Lưu thông báo vào lịch |
| **Sinh viên 2** | `lethimaianh` | `Student@123` | Tương tự SV1 với danh sách 3 môn học độc lập, cách ly việc cá nhân hoàn toàn |
| **Quản trị viên** | `admin` | `Admin@123` | Toàn quyền CRUD Lịch học, Lịch thi, Thông báo và Sự kiện tại `/admin.html` |

---

## 🎯 6. Điểm nổi bật của Tính năng UEH My Week

1. **Chọn tuần tùy ý:**
   - Người dùng có thể chọn bất kỳ ngày nào qua bộ chọn ngày (DatePicker) hoặc nút Tuần trước/Tuần sau.
   - Hệ thống tự động xác định khoảng thứ Hai đến Chủ Nhật (`dd/MM/yyyy – dd/MM/yyyy`) và tải dữ liệu tương ứng.
   - Ví dụ: Chọn ngày `16/10/2026` ➔ Hiển thị tuần `12/10/2026 – 18/10/2026`.
   - Chọn ngày `28/11/2026` ➔ Hiển thị tuần thi `23/11/2026 – 29/11/2026` với 3 kỳ thi.
2. **Đồng bộ Lịch học & Lịch thi:**
   - Tự động mở rộng các quy tắc học kỳ (`ClassScheduleRules`) và lịch thi (`ExamSchedules`) của các lớp sinh viên theo học thành các buổi học cụ thể trong tuần.
3. **UEH Events (Hoạt động trường):**
   - Chỉ hiển thị các sự kiện diễn ra trong tuần đang xem.
   - Sinh viên bấm **Đăng ký** ➔ Sự kiện ngay lập tức được thêm vào lịch trình tuần cá nhân.
   - Sinh viên bấm **Hủy** ➔ Sự kiện được gỡ khỏi lịch tuần.
4. **Thông báo cần chú ý & Lưu vào lịch:**
   - Ưu tiên các thông báo khẩn cấp hoặc có hạn chót (Deadline Radar).
   - Sinh viên chọn ngày theo dõi ➔ Thông báo xuất hiện đúng ngày trong My Week.
5. **Personal Tasks (Việc cá nhân):**
   - Thêm, sửa, đánh dấu hoàn thành (checkbox) và xóa công việc cá nhân.
   - Dữ liệu lưu trong SQL Server và được bảo vệ theo `StudentId` của tài khoản đăng nhập.

---

## 🛡️ 7. Quản trị Dữ liệu tại Giao diện Admin (`/admin.html`)

Quản trị viên (`admin`) có thể trực tiếp:
- **Lịch học:** Thêm quy tắc lịch học (chọn lớp, thứ, giờ bắt đầu/kết thúc, phòng, giảng viên), xóa quy tắc.
- **Lịch thi:** Tạo lịch thi cuối kỳ (ngày thi, giờ thi, phòng thi, hình thức), xóa lịch thi.
- **Thông báo:** Đăng thông báo mới có hạn chót và mức độ ưu tiên; xóa thông báo an toàn (nếu đã có sinh viên lưu vào lịch, hệ thống tự động chuyển sang *Ngừng hiển thị* để bảo toàn toàn vẹn dữ liệu).
- **Sự kiện:** Tạo workshop/career fair với giới hạn số lượng và hạn đăng ký.

Mọi thay đổi từ Admin lập tức phản ánh ngay trên các trang Lịch học, Lịch thi, Thông báo và UEH My Week của sinh viên mà không cần khởi động lại server.

---

## 🧪 8. Báo cáo Kết quả Kiểm thử Tích hợp (12/12 PASS)

Đã chạy kiểm thử tự động toàn diện qua PowerShell với kết quả:

```text
[TEST-01] PASS: Sinh viên đăng nhập và xem My Week (Tuần 12/10 - 18/10/2026: 5 buổi học)
[TEST-02] PASS: Chuyển đến một tuần bất kỳ xa hiện tại (18/01/2027)
[TEST-03] PASS: Lịch học & Lịch thi đồng bộ trong My Week (Tuần học: 5 buổi; Tuần thi: 3 môn)
[TEST-04] PASS: Sinh viên xem sự kiện chưa đăng ký (Hiện 4 sự kiện hợp lệ)
[TEST-05] PASS: Đăng ký sự kiện và tự động cập nhật vào dòng thời gian tuần
[TEST-06] PASS: Thêm thông báo vào lịch cá nhân với ngày nhắc nhở
[TEST-07] PASS: Tạo, đánh dấu hoàn thành và xóa việc cá nhân
[TEST-08] PASS: Admin tạo thông báo mới có phân quyền và xóa an toàn
[TEST-09] PASS: Admin tạo quy tắc lịch học mới cho sinh viên
[TEST-10] PASS: Sinh viên bị chặn khi gọi API Admin (Trả về HTTP 403 Forbidden)
[TEST-11] PASS: Cách ly dữ liệu — Sinh viên 1 bị từ chối khi thao tác lên việc cá nhân của Sinh viên 2
[TEST-12] PASS: Xử lý tuần không có lịch học (Giao diện hiển thị empty-state an toàn, không phát sinh lỗi)

KẾT QUẢ TOÀN DIỆN: 12 / 12 BÀI TEST ĐẠT KẾT QUẢ PASS 100%!
```
