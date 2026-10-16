# 🎓 CỔNG THÔNG TIN SINH VIÊN UEH & TÍNH NĂNG UEH MY WEEK
> **Đồ án Prototype môn học / Báo cáo thực hành Full-Stack**  
> **Khoa / Ngành:** Khoa Toán thống kê — Ngành Thống kê kinh doanh — Đại học Kinh tế TP. Hồ Chí Minh (UEH)  
> **Công nghệ sử dụng:** ASP.NET Core 8.0 Web API • Entity Framework Core (Database-First) • Microsoft SQL Server 2025 • HTML5/CSS3/JavaScript thuần.

---

## 👨‍🏫 DÀNH CHO GIẢNG VIÊN: HƯỚNG DẪN CHẤM ĐIỂM & TEST PROTOTYPE

Kính gửi Quý Thầy/Cô, dưới đây là hướng dẫn để Thầy/Cô có thể chạy và kiểm thử toàn bộ các luồng nghiệp vụ của hệ thống.

### 1. Chuẩn bị môi trường (Nếu chạy Local)
- Máy tính có cài **.NET 8.0 SDK** (hoặc mới hơn).
- **Microsoft SQL Server** (bản Developer, Express hoặc LocalDB) đang chạy tại `localhost`.

### 2. Khởi tạo Database (Chỉ chạy 1 lần duy nhất)
Mở cửa sổ Terminal (PowerShell hoặc CMD) tại thư mục gốc của dự án và chạy các lệnh sau:

```powershell
# Bước 1: Tạo Database UEHStudentDB
sqlcmd -S localhost -E -C -i "database\01_CreateDatabase.sql"

# Bước 2: Tạo toàn bộ 14 bảng, ràng buộc khóa chính, khóa ngoại và indexes
sqlcmd -S localhost -E -C -i "database\02_CreateSchema.sql"

# Bước 3: Nạp dữ liệu mẫu chuẩn của Khoa Toán thống kê
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

## 🔑 DANH SÁCH TÀI KHOẢN ĐĂNG NHẬP

> **Quy ước đăng nhập sinh viên:**
> - **Tên đăng nhập:** Mã số sinh viên (MSSV)
> - **Mật khẩu:** 4 số cuối của MSSV

| STT | Họ và tên | Mã sinh viên | Tên đăng nhập | Mật khẩu | Khoa / Ngành |
|---|---|---|---|---|---|
| 1 | **Nguyễn Minh Khôi** | `31251022130` | `31251022130` | `2130` | Khoa Toán thống kê - Thống kê kinh doanh |
| 2 | **Nguyễn Nhựt Thắng** | `31251023860` | `31251023860` | `3860` | Khoa Toán thống kê - Thống kê kinh doanh |
| 3 | **Phạm Trần Tuấn Khang** | `31251025093` | `31251025093` | `5093` | Khoa Toán thống kê - Thống kê kinh doanh |
| 4 | **Trà Anh Thư** | `31251026911` | `31251026911` | `6911` | Khoa Toán thống kê - Thống kê kinh doanh |
| 5 | **Nguyễn Thị Thanh Tâm** | `31251023365` | `31251023365` | `3365` | Khoa Toán thống kê - Thống kê kinh doanh |
| 6 | **Nguyễn Ngọc Anh Thư** | `31251023149` | `31251023149` | `3149` | Khoa Toán thống kê - Thống kê kinh doanh |
| 7 | **Nguyễn Hữu Toàn** | `31251026534` | `31251026534` | `6534` | Khoa Toán thống kê - Thống kê kinh doanh |
| 🛡️ | **Quản trị viên (Admin)** | — | `admin` | `Admin@123` | Quản trị viên hệ thống |

---

## 📚 DANH MỤC MÔN HỌC & GIẢNG VIÊN GIẢNG DẠY

Tất cả môn học đã được đồng bộ chuẩn hóa tên giảng viên thực tế:

| STT | Mã học phần | Tên học phần | Số TC | Lớp học phần | Giảng viên giảng dạy | Lịch học định kỳ |
|---|---|---|---|---|---|---|
| 1 | `TEC5500183B` | **Tư duy thiết kế** | 3 | `28C1TEC5500183B01` | **ThS. Nguyễn Quốc Khánh** | Thứ Hai (07:30 - 11:30), Phòng N2-508 |
| 2 | `MAT58013B` | **Tính toán thống kê** | 3 | `28C1MAT5801360101` | **TS. Lê Thanh Trung** | Thứ Hai (12:30 - 16:30), Phòng B1-205 |
| 3 | `PUF604103B` | **Nguyên lý kế toán** | 3 | `28C1PUF6041030101` | **ThS. Trần Thị Thương** | Thứ Tư (07:30 - 11:30) & Thứ Năm (07:30 - 09:30), N2-507 |
| 4 | `BUS3012B` | **Quản trị học** | 3 | `28C1BUS30120001` | **PGS.TS. Hoàng Văn Bình** | Thứ Sáu (07:30 - 11:30), Phòng N3-201 |

*(Môn "Tính toán động kế" đã được loại bỏ hoàn toàn khỏi chương trình đào tạo và cơ sở dữ liệu)*.

---

## 📋 KỊCH BẢN KIỂM THỬ GỢI Ý (5 BƯỚC TEST NHANH)

### Bước 1: Đăng nhập Sinh viên
1. Tại màn hình `login.html`, nhập MSSV `31251022130` và Mật khẩu `2130` (hoặc tài khoản của bất kỳ sinh viên nào trong nhóm) ➔ Bấm **Đăng nhập hệ thống**.
2. Hệ thống chuyển vào giao diện chính mang màu sắc nhận diện Cổng thông tin UEH Student (Màu Teal đậm `#005863`, Cam UEH `#E85A0C`, nền xám trắng sạch sẽ).

### Bước 2: Trải nghiệm tính năng trung tâm — UEH My Week
1. **Dòng thời gian tuần (Weekly Timeline):** Hiển thị trực quan toàn bộ các buổi học trong tuần từ Thứ Hai đến Chủ Nhật, hiển thị chính xác môn **Tư duy thiết kế** của **ThS. Nguyễn Quốc Khánh**.
2. **Chọn tuần bất kỳ:** 
   - Nhấp vào ô **"Chọn ngày bất kỳ"** và chọn ngày `28/11/2026`.
   - Hệ thống tự động chuyển sang tuần thi `23/11/2026 – 29/11/2026`, xuất hiện các kỳ thi cuối kỳ có nhãn đỏ nổi bật.
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
3. Đăng xuất và đăng nhập lại tài khoản Sinh viên: Buổi học mới vừa tạo **lập tức xuất hiện trên cả trang Thời khóa biểu và UEH My Week**, chứng minh dữ liệu được đồng bộ 100% từ cơ sở dữ liệu thật.

---

## 🌐 GIẢI THÍCH VỀ ĐƯỜNG LINK ONLINE PROTOTYPE

### 1. Đường link này từ đâu ra?
Đường link công khai dạng `https://martha-wiki-golf-penguin.trycloudflare.com` được tạo ra thông qua **Cloudflare Tunnel (`cloudflared`)**.  
- Tiện ích này thiết lập một đường ống bảo mật (Secure Tunnel) hai chiều mã hóa đầu cuối giữa máy tính của bạn và mạng lưới máy chủ toàn cầu của Cloudflare.
- Nhờ đó, bất kỳ ai (kể cả Thầy/Cô dùng điện thoại hay máy tính ngoài trường) truy cập link trên đều được Cloudflare chuyển tiếp yêu cầu (forward) trực tiếp về port 5000 (`http://localhost:5000`) trên máy tính của bạn.

### 2. Mọi cập nhật code và database có được update lên link đó không?
**CÓ, 100% CẬP NHẬT THEO THỜI GIAN THỰC!**  
Vì đường link này KHÔNG PHẢI là một bản copy tĩnh đưa lên mạng, mà nó trỏ trực tiếp vào đúng server backend ASP.NET Core và database SQL Server `UEHStudentDB` đang chạy trên máy tính của bạn:
- Khi bạn sửa code giao diện (`login.html`, `app.js`, `styles.css`), người truy cập link bấm F5 là thấy giao diện mới ngay lập tức.
- Khi bạn sửa dữ liệu trong SQL Server (thêm môn, đổi tên sinh viên), API sẽ đọc dữ liệu mới nhất từ SQL Server và trả về ngay trên link.
- **Điều kiện duy nhất:** Giữ cho máy tính mở và tiến trình ứng dụng + đường hầm Cloudflare đang chạy.

---

## 🏛️ CẤU TRÚC CƠ SỞ DỮ LIỆU ĐỒ ÁN (14 BẢNG CHUẨN HÓA)
1. `AcademicTerms`: Niên khóa và học kỳ.
2. `Courses`: Danh mục môn học.
3. `ClassSections`: Lớp học phần thực tế.
4. `Students`: Hồ sơ sinh viên (Mã SV, Họ tên, Khoa Toán thống kê, Ngành Thống kê kinh doanh).
5. `AppUsers`: Tài khoản bảo mật (User = MSSV, Mật khẩu = 4 số cuối).
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
