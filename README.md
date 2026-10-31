# 🎓 CỔNG THÔNG TIN SINH VIÊN UEH & TÍNH NĂNG UEH MY WEEK
> **BÁO CÁO THUYẾT TRÌNH ĐỒ ÁN PROTOTYPE & TÀI LIỆU THẨM ĐỊNH HỆ THỐNG**  
> **Đơn vị:** Khoa Toán - Thống kê • Trường Đại học Kinh tế TP. Hồ Chí Minh (UEH)  
> **Chuyên ngành:** Thống kê kinh doanh  
> **Học phần:** Tư duy thiết kế (Design Thinking)  
> **Giảng viên hướng dẫn:** ThS. Nguyễn Quốc Khánh  
> **Công nghệ phát triển:** ASP.NET Core 8.0 Web API • Entity Framework Core (Database-First) • Microsoft SQL Server 2025 • HTML5 / CSS3 / Modern JavaScript  

---

## 🌐 LINK TRẢI NGHIỆM TRỰC TIẾP (LIVE ONLINE PROTOTYPE)

Hệ thống đã được triển khai thông qua đường hầm mã hóa bảo mật toàn cầu **Cloudflare Tunnel**, cho phép Thầy và Hội đồng kiểm thử trực tiếp trên máy tính, máy tính bảng hoặc điện thoại di động mà không cần cài đặt môi trường:

👉 **[https://programming-comments-though-mumbai.trycloudflare.com/login.html](https://programming-comments-though-mumbai.trycloudflare.com/login.html)**  
*(Hoặc truy cập trang chủ: `https://programming-comments-though-mumbai.trycloudflare.com/`)*

---

## 👥 THÀNH VIÊN NHÓM THỰC HIỆN & THÔNG TIN ĐĂNG NHẬP

Hệ thống được thiết kế cá nhân hóa 100% cho 7 sinh viên thuộc Khoa Toán - Thống kê, ngành Thống kê kinh doanh. 

> **Quy ước bảo mật tài khoản:**
> - **Tên đăng nhập (Username):** Mã số sinh viên (MSSV)
> - **Mật khẩu (Password):** 4 số cuối của MSSV

| STT | Họ và tên sinh viên | Mã số sinh viên | Tên đăng nhập | Mật khẩu | Lớp / Ngành sinh hoạt |
|:---:|---|:---:|:---:|:---:|---|
| 1 | **Nguyễn Minh Khôi** | `31251022130` | `31251022130` | `2130` | Khoa Toán thống kê — Thống kê kinh doanh |
| 2 | **Nguyễn Nhựt Thắng** | `31251023860` | `31251023860` | `3860` | Khoa Toán thống kê — Thống kê kinh doanh |
| 3 | **Phạm Trần Tuấn Khang** | `31251025093` | `31251025093` | `5093` | Khoa Toán thống kê — Thống kê kinh doanh |
| 4 | **Trà Anh Thư** | `31251026911` | `31251026911` | `6911` | Khoa Toán thống kê — Thống kê kinh doanh |
| 5 | **Nguyễn Thị Thanh Tâm** | `31251023365` | `31251023365` | `3365` | Khoa Toán thống kê — Thống kê kinh doanh |
| 6 | **Nguyễn Ngọc Anh Thư** | `31251023149` | `31251023149` | `3149` | Khoa Toán thống kê — Thống kê kinh doanh |
| 7 | **Nguyễn Hữu Toàn** | `31251026534` | `31251026534` | `6534` | Khoa Toán thống kê — Thống kê kinh doanh |
| 🛡️ | **Quản trị viên (Admin)** | Hệ thống UEH | `admin` | `Admin@123` | Quản trị đào tạo & Khảo thí |

---

## 💡 PHẦN I: Ý TƯỞNG THIẾT KẾ & BỐI CẢNH BÀI TOÁN (DESIGN THINKING)

### 1. Bối cảnh thực tế & Nỗi đau của Người học (User Pain Points)
Sinh viên UEH hàng ngày phải đối mặt với sự phân mảnh thông tin nghiêm trọng giữa nhiều nền tảng số:
- **Cổng thông tin đào tạo (Portal):** Chứa thời khóa biểu ma trận và lịch thi nhưng hiển thị tĩnh, khó theo dõi biến động theo từng tuần.
- **Hệ thống quản lý học tập (LMS):** Giảng viên giao bài tập, đề cương, quiz với các hạn chót dồn dập, sinh viên phải đăng nhập vào từng môn học riêng lẻ để kiểm tra.
- **Kênh thông báo hành chính:** Thông báo học phí từ Phòng Kế hoạch - Tài chính, sinh hoạt công dân từ Phòng Chăm sóc người học, lịch khám sức khỏe từ Trạm Y tế nằm lẫn lộn với hàng trăm thông báo khác.
- **Kênh sự kiện phong trào:** Hội thảo, workshop, ngày hội việc làm UEH Career Fair được truyền thông rời rạc qua fanpage hoặc email.

👉 **Hậu quả:** Sinh viên dễ bị trễ hạn nộp bài tập nhóm, quên ngày nộp học phí dẫn đến bị khóa đăng ký môn học, hoặc bỏ lỡ các workshop kỹ năng bổ ích.

### 2. Ý tưởng giải pháp: "UEH My Week — Trợ lý tuần thông minh"
Áp dụng phương pháp luận **Tư duy thiết kế (Design Thinking)** lấy sinh viên làm trung tâm, nhóm đã đề xuất và xây dựng tính năng **UEH My Week**:
- **Nguyên lý "Single Source of Truth":** Tích hợp Thời khóa biểu học tập, Lịch thi, Sự kiện ngoại khóa, Bài tập LMS và Hạn nộp học phí vào một dòng thời gian duy nhất theo tuần.
- **Cá nhân hóa tự động (Smart Personalization):** Không bắt sinh viên phải tìm kiếm hay gõ tay những gì nhà trường và giảng viên đã ban hành. Dữ liệu tự động chảy về tài khoản của đúng sinh viên đó.
- **Tương tác hai chiều tức thì (Real-time Interactivity):** Đăng ký sự kiện, thêm việc cá nhân, đánh dấu hoàn thành chỉ bằng một cú nhấp chuột mà không cần tải lại trang.

---

## 🧠 PHẦN II: LOGIC NGHIỆP VỤ & TRIẾT LÝ PHÂN TÁCH DỮ LIỆU

Nhóm đã chuẩn hóa cấu trúc dữ liệu theo 3 phân hệ rành mạch, giải quyết triệt để vấn đề "nhiễu loạn thông tin":

```
                      ┌─────────────────────────────────────────┐
                      │          CỔNG THÔNG TIN UEH             │
                      │             (UEH My Week)               │
                      └────────────────────┬────────────────────┘
                                           │
         ┌─────────────────────────────────┼─────────────────────────────────┐
         ▼                                 ▼                                 ▼
┌──────────────────┐             ┌──────────────────┐              ┌──────────────────┐
│ 📢 THÔNG BÁO     │             │ ⏳ HẠN CHÓT &    │              │ 🎪 HOẠT ĐỘNG     │
│    CẤP TRƯỜNG    │             │    DEADLINE TUẦN │              │    UEH TRONG TUẦN│
└────────┬─────────┘             └────────┬─────────┘              └────────┬─────────┘
         │                                │                                 │
         ├─ Đóng học phí                  ├─ Bài tập LMS (GV giao)          ├─ Workshop
         ├─ Sinh hoạt công dân            ├─ Lịch thi cuối kỳ               ├─ Career Fair
         └─ Khám sức khỏe                 ├─ Hạn học phí (Phòng KHTC)       └─ Seminar chuyên đề
                                          ├─ Hạn đăng ký sự kiện            
                                          └─ Việc cá nhân tự thêm           
```

### 1. 📢 "Thông báo trường cần chú ý" (School-wide Notices)
- **Bản chất:** Là các văn bản quy định, chế độ hành chính cấp toàn trường do các phòng ban chức năng ban hành.
- **Quy tắc hiển thị:** Chỉ gồm 3 nhóm thông báo then chốt:
  1. *Học phí học kỳ* — Phòng Kế hoạch - Tài chính.
  2. *Sinh hoạt công dân sinh viên* — Phòng Chăm sóc và Hỗ trợ người học.
  3. *Khám sức khỏe định kỳ* — Trạm Y tế UEH.
- **Đặc trưng:** Gắn nhãn **`📌 Dành cho sinh viên`**. Sinh viên **không cần bấm "Thêm vào lịch"** thủ công; nhấp vào thẻ để mở hộp thoại đọc toàn văn quyết định của nhà trường.
- **Nguyên tắc cốt lõi:** Tuyệt đối không để bài tập môn học tại đây nhằm giữ luồng thông tin hành chính luôn sạch sẽ.

### 2. ⏳ "Hạn chót & Deadline tuần này" (Actionable Due Dates & LMS Sync)
- **Bản chất:** Là **radar cảnh báo hành động** tập trung tất cả các việc sinh viên *phải nộp hoặc phải hoàn tất* trong tuần đang xem:
  - 🟠 **Bài tập LMS (Đồng bộ tự động):** Mô phỏng kết nối API từ hệ thống học tập số LMS Canvas/Moodle do giảng viên giao:
    - *Bài tập nhóm môn Tư duy thiết kế* — Giảng viên: **ThS. Nguyễn Quốc Khánh** (Hạn: 23:59 Chủ Nhật, 18/10/2026).
    - *Thực hành R/Python chương 5 môn Tính toán thống kê* — Giảng viên: **TS. Lê Thanh Trung** (Hạn: 23:59 Chủ Nhật, 18/10/2026).
  - 🔴 **Lịch thi kết thúc môn:** Khi chuyển sang tuần thi, tự động lấy lịch thi từ phòng Khảo thí và gắn cờ `GẤP`.
  - 🔴 **Hạn nộp học phí cấp trường:** Cảnh báo mốc chót nộp học phí (23:59 Chủ Nhật, 18/10/2026).
  - 🔵 **Việc cá nhân:** Các ghi chú, chuẩn bị bài do sinh viên chủ động tự thêm.
- **Đồng bộ thời gian 100%:** Chỉ hiển thị những hạn chót nằm trong tuần đang chọn (Thứ Hai đến Chủ Nhật). Không sót việc, không hiển thị các thông báo rời rạc.

### 3. 🎪 "Hoạt động UEH trong tuần" & Cơ chế đồng bộ hai chiều (Two-Way Sync)
- Khi sinh viên **Chưa đăng ký**:
  - Tại cột Sự kiện: Hiển thị nút **`Đăng ký`**.
  - Tại cột Deadline: Xuất hiện dòng cảnh báo **`[Hạn đăng ký sự kiện]`** kèm thời hạn đóng cổng đăng ký (Ví dụ: trước 18:00 Thứ Sáu).
- Khi sinh viên **Bấm đăng ký thành công**:
  - Cột Sự kiện: Chuyển sang nhãn **`✓ Đã tham gia`** kèm nút `Hủy`.
  - Cột Deadline: Lập tức chuyển thành **`[Sự kiện UEH] Tham dự: Workshop...`** (Màu tím, nhãn `ĐÃ ĐK`).
  - Cột Dòng thời gian tuần (Timeline): Buổi workshop tự động xuất hiện vào đúng khung giờ Thứ Bảy (09:00 - 11:30) tại Hội trường A.
- Khi bấm **Hủy đăng ký**: Toàn bộ hệ thống tự động hoàn tác trạng thái trên cả 3 khu vực mà không cần tải lại trang.

---

## 🎨 PHẦN III: CẤU TRÚC GIAO DIỆN & THIẾT KẾ TRỰC QUAN

Giao diện được thiết kế hiện đại theo chuẩn nhận diện thương hiệu UEH với tone màu chủ đạo:
- **Teal UEH (`#005863`):** Đại diện cho sự vững vàng, học thuật và tin cậy.
- **Cam UEH (`#E85A0C`):** Tạo điểm nhấn năng động, cảnh báo deadline và các tính năng mới.
- **Nền Light Gray (`#f4f6f8`) & White Card (`#ffffff`):** Mang lại không gian làm việc sạch sẽ, chống mỏi mắt.

```
┌──────────────────────────────────────────────────────────────────────────────────────────┐
│  UEH STUDENT PORTAL                                         👤 Nguyễn Minh Khôi | Đăng xuất│
├──────────────┬───────────────────────────────────────────────────────────────────────────┤
│ 📌 MY WEEK   │ 📅 UEH My Week — Lịch tuần tổng hợp                                        │
│              │ [◀ Tuần trước]  [Tuần hiện tại (16/10)]  [Tuần sau ▶]   🗓️ 12/10/2026 – 18/10/2026│
│ 🔍 TRA CỨU   ├───────────────────────────────────────────────────────────────────────────┤
│ - Lịch học   │ 📚 4 Buổi học  │  📝 0 Kỳ thi  │  🎯 1 Sự kiện  │  ✅ 3 Việc cần làm │  🔔 3 Thông báo │
│ - Lịch thi   ├─────────────────────────────────────────┬─────────────────────────────────┤
│ - Thông báo  │ 📅 LỊCH TRÌNH CÁ NHÂN TUẦN NÀY          │ ⏳ HẠN CHÓT & DEADLINE TUẦN NÀY │
│              │                                         │ - Tham dự: Workshop (T7 09:00)  │
│              │ Thứ Hai, 12/10/2026                     │ - Chuẩn bị case study (T7 23:59)│
│              │ ├ 07:30 - 11:30 | Tư duy thiết kế       │ - [LMS] Bài tập nhóm (CN 23:59) │
│              │ └ 12:30 - 16:30 | Tính toán thống kê    │ - [LMS] Thực hành R (CN 23:59)  │
│              │                                         │ - [Học phí] Hạn nộp (CN 23:59)  │
│              │ Thứ Tư, 14/10/2026                      ├─────────────────────────────────┤
│              │ └ 07:30 - 11:30 | Nguyên lý kế toán     │ 🎪 HOẠT ĐỘNG UEH TRONG TUẦN     │
│              │                                         │ - Workshop Kỹ năng thuyết trình │
│              │ Thứ Năm, 15/10/2026                     │   [✓ Đã tham gia] [Hủy]         │
│              │ └ 07:30 - 09:30 | Nguyên lý kế toán     ├─────────────────────────────────┤
│              │                                         │ 📢 THÔNG BÁO TRƯỜNG CẦN CHÚ Ý   │
│              │ Thứ Sáu, 16/10/2026 (Hôm nay)           │ - Thông báo nộp học phí HK cuối │
│              │ └ 07:30 - 11:30 | Quản trị học          │ - Sinh hoạt công dân sinh viên  │
│              │                                         │ - Khám sức khỏe định kỳ năm 2026│
│              │ Thứ Bảy, 17/10/2026                     │                                 │
│              │ └ 09:00 - 11:30 | [SỰ KIỆN] Workshop... │                                 │
└──────────────┴─────────────────────────────────────────┴─────────────────────────────────┘
```

### Chi tiết các phân vùng trên màn hình:
1. **Thanh điều hướng tuần (Week Selector Bar):** 
   - Điều hướng tiến/lùi 7 ngày.
   - Nút tắt quay nhanh về "Tuần hiện tại (16/10)".
   - Bộ chọn ngày bất kỳ (HTML5 Date Input) hỗ trợ nhảy đến bất kỳ mốc thời gian nào trong học kỳ.
2. **Dải tóm tắt tuần (Summary Ribbon):**
   - 5 thẻ đếm thông minh tự động tổng hợp số lượng: *Buổi học*, *Kỳ thi*, *Sự kiện đã tham gia*, *Việc cá nhân cần làm*, *Thông báo trường*.
3. **Cột Dòng thời gian cá nhân (Weekly Timeline - 65% độ rộng):**
   - Hiển thị tuần từ Thứ Hai đến Chủ Nhật.
   - Thẻ ngày hiện tại (Thứ Sáu 16/10) nổi bật với viền cam và badge `Hôm nay`.
   - Mỗi mục hiển thị rõ ràng: Khung giờ, Tên môn học, Mã lớp, Giảng viên, Phòng học, Nhãn phân loại màu sắc.
   - Hỗ trợ nút `+ Thêm việc cá nhân` và nút xóa `✕` cho từng việc cá nhân.
4. **Cột Radar cảnh báo bên phải (Side Column - 35% độ rộng):**
   - **Box 1 (Hạn chót & Deadline):** Phân loại nhãn cam `Bài tập LMS`, nhãn đỏ `Hạn học phí`, nhãn tím `Sự kiện UEH`, nhãn xanh `Việc cá nhân`.
   - **Box 2 (Hoạt động UEH):** Danh thiếp sự kiện sinh động với địa điểm, thời gian, nút bấm đăng ký/hủy tức thì.
   - **Box 3 (Thông báo trường):** Danh mục thông báo chính thống gắn nhãn `📌 Dành cho sinh viên`, bấm vào xem chi tiết toàn văn bản.
5. **Các tab tra cứu chuyên biệt:**
   - **Lịch học (TKB):** Tái hiện ma trận 13 tiết x 7 thứ nguyên bản theo cổng UEH gốc.
   - **Lịch thi:** Bảng tra cứu ngày thi, giờ thi, số báo danh, phòng thi và hình thức thi (Tự luận/Trắc nghiệm).
   - **Thông báo chung:** Tìm kiếm theo từ khóa và lọc đa tiêu chí theo chuyên mục.

---

## 📚 DANH MỤC MÔN HỌC & GIẢNG VIÊN GIẢNG DẠY

Tất cả học phần đã được chuẩn hóa thông tin giảng viên và lịch học thực tế:

| STT | Mã học phần | Tên học phần | Số TC | Mã lớp học phần | Giảng viên giảng dạy | Lịch học định kỳ | Phòng học |
|:---:|:---:|---|:---:|:---:|---|---|:---:|
| 1 | `TEC5500183B` | **Tư duy thiết kế** | 3 | `28C1TEC5500183B01` | **ThS. Nguyễn Quốc Khánh** | Thứ Hai (07:30 - 11:30) | N2-508 |
| 2 | `MAT58013B` | **Tính toán thống kê** | 3 | `28C1MAT5801360101` | **TS. Lê Thanh Trung** | Thứ Hai (12:30 - 16:30) | B1-205 |
| 3 | `PUF604103B` | **Nguyên lý kế toán** | 3 | `28C1PUF6041030101` | **ThS. Trần Thị Thương** | Thứ Tư (07:30 - 11:30)<br>Thứ Năm (07:30 - 09:30) | N2-507 |
| 4 | `BUS3012B` | **Quản trị học** | 3 | `28C1BUS30120001` | **PGS.TS. Hoàng Văn Bình** | Thứ Sáu (07:30 - 11:30) | N3-201 |

*(Môn "Tính toán động kế" không có trong chương trình đào tạo đã được loại bỏ hoàn toàn)*.

---

## 🏛️ PHẦN IV: KIẾN TRÚC KỸ THUẬT & CƠ SỞ DỮ LIỆU

### 1. Kiến trúc phân tầng (Clean Layered Architecture)
- **Tầng Trình diễn (Presentation Layer):** Giao diện thuần Responsive không phụ thuộc framework nặng, tối ưu tốc độ tải dưới 0.3s.
- **Tầng Ứng dụng (Web API Layer):** ASP.NET Core 8.0 Controller-based Web API, tuân thủ nghiêm ngặt chuẩn RESTful, trả về dữ liệu JSON có cấu trúc.
- **Tầng Nghiệp vụ (Service Layer):** `MyWeekService` xử lý thuật toán hợp nhất thời gian, gom nhóm 7 ngày, tính toán hạn chót và trạng thái sự kiện.
- **Tầng Dữ liệu (Data Access Layer):** Entity Framework Core 8 sử dụng mô hình **Database-First**, ánh xạ trực tiếp từ 14 bảng SQL Server thật.
- **Bảo mật & Ủy quyền (Security):** Xác thực bằng **JSON Web Token (JWT Bearer)**, mã hóa mật khẩu an toàn, phân quyền theo Role (`Student` và `Admin`).

### 2. Cấu trúc Cơ sở dữ liệu chuẩn hóa (14 Bảng)
1. `AcademicTerms`: Quản lý niên khóa và học kỳ.
2. `Courses`: Danh mục môn học trong chương trình đào tạo.
3. `ClassSections`: Danh sách các lớp học phần được mở trong kỳ.
4. `Students`: Hồ sơ sinh viên (MSSV, Họ tên, Khoa, Ngành, Khóa học).
5. `AppUsers`: Tài khoản đăng nhập hệ thống, liên kết 1-1 với sinh viên hoặc tài khoản quản trị.
6. `StudentCourseEnrollments`: Danh sách sinh viên đăng ký lớp học phần.
7. `ClassScheduleRules`: Quy tắc thời khóa biểu định kỳ theo thứ và khung giờ.
8. `ExamSchedules`: Lịch thi kết thúc học phần, phòng thi, hình thức thi.
9. `Announcements`: Thông báo chính thức của nhà trường.
10. `AnnouncementTargets`: Quy tắc phân loại đối tượng nhận thông báo (Toàn trường / Khóa / Khoa).
11. `Events`: Danh mục hội thảo, workshop, ngày hội việc làm UEH.
12. `EventRegistrations`: Ghi nhận sinh viên đăng ký tham gia sự kiện.
13. `PersonalTasks`: Quản lý việc cá nhân và các bài tập môn học.
14. `StudentSavedAnnouncements`: Bảng dữ liệu hỗ trợ theo dõi thông báo.

---

## 🎙️ PHẦN V: KỊCH BẢN THUYẾT TRÌNH GỢI Ý (5 PHÚT BẢO VỆ ĐỒ ÁN)

Dưới đây là kịch bản gợi ý giúp nhóm tự tin trình bày trước Giảng viên hướng dẫn:

### Phút 1: Đặt vấn đề & Giới thiệu ý tưởng
> *"Kính thưa Thầy, trong quá trình học tập tại UEH, nhóm chúng em nhận thấy sinh viên gặp rất nhiều khó khăn khi phải đăng nhập cùng lúc 3-4 nền tảng: Cổng thông tin để xem TKB, LMS để xem hạn bài tập, fanpage để tìm sự kiện và email để ngóng lịch học phí. Sự phân mảnh này thường xuyên khiến sinh viên bị sót deadline hoặc quên lịch trình.  
> Vì vậy, vận dụng phương pháp Tư duy thiết kế (Design Thinking), nhóm em đã phát triển sản phẩm prototype: **Cổng thông tin sinh viên tích hợp tính năng UEH My Week** — một dòng thời gian hợp nhất giúp người học quản lý trọn vẹn tuần học tập của mình."*

### Phút 2: Trình diễn đăng nhập & Giao diện trung tâm
> *(Nhóm mở trang `login.html`, đăng nhập tài khoản của bạn Nguyễn Minh Khôi `31251022130` / `2130`)*  
> *"Khi sinh viên đăng nhập, hệ thống tự động nhận diện danh tính sinh viên thuộc Khoa Toán - Thống kê. Ngay tại màn hình chính, tính năng **UEH My Week** hiển thị đầy đủ dải tóm tắt tuần và lịch học chi tiết của tuần hiện tại.  
> Cụ thể vào Thứ Hai, bạn Khôi có lịch học môn **Tư duy thiết kế** của **ThS. Nguyễn Quốc Khánh** vào buổi sáng tại phòng N2-508 và môn **Tính toán thống kê** của **TS. Lê Thanh Trung** vào buổi chiều tại phòng B1-205."*

### Phút 3: Chứng minh cơ chế đồng bộ LMS & Phân biệt dữ liệu
> *"Điểm cốt lõi thứ nhất là khu vực **⏳ Hạn chót & Deadline tuần này** ở cột bên phải. Thay vì bắt sinh viên phải tự gõ lại bài tập môn học, hệ thống **tự động đồng bộ từ LMS**:
> - Bài tập nhóm môn Tư duy thiết kế của Thầy Khánh, hạn 23:59 Chủ Nhật.
> - Bài tập thực hành R/Python của Thầy Trung, hạn 23:59 Chủ Nhật.
> Đồng thời, thông báo đóng học phí của Phòng Kế hoạch - Tài chính cũng được cảnh báo đỏ ngay tại đây.  
> Bên dưới là mục **📢 Thông báo trường cần chú ý**, được lọc riêng các văn bản hành chính quan trọng cấp toàn trường (học phí, sinh hoạt công dân, khám sức khỏe) và gắn nhãn '📌 Dành cho sinh viên', hoàn toàn không bị lẫn lộn với bài tập môn học."*

### Phút 4: Demo tương tác hai chiều Sự kiện UEH (Two-Way Sync)
> *(Nhóm bấm nút **Đăng ký** sự kiện 'Workshop: Kỹ năng thuyết trình hiệu quả')*  
> *"Điểm cốt lõi thứ hai là sự đồng bộ hai chiều thời gian thực giữa **Sự kiện UEH** và **Lịch trình tuần**.  
> Khi bạn Khôi bấm **Đăng ký** sự kiện Workshop Kỹ năng thuyết trình:
> 1. Tại mục Sự kiện: Nút chuyển ngay thành `✓ Đã tham gia`.
> 2. Tại Dòng thời gian tuần: Buổi workshop lập tức xuất hiện vào Thứ Bảy lúc 09:00 tại Hội trường A.
> 3. Tại cột Deadline: Mục này tự động chuyển thành lịch tham gia sự kiện `[Sự kiện UEH] Tham dự: Workshop...`.  
> Nếu bạn Khôi bấm **Hủy**, cả 3 vị trí trên màn hình lập tức cập nhật lại mà không cần bấm F5 tải lại trang."*

### Phút 5: Chuyển tuần khảo thí & Kết luận
> *(Nhóm chọn ngày `28/11/2026` trên thanh chọn tuần)*  
> *"Cuối cùng, tính năng UEH My Week cho phép xem bất kỳ tuần nào trong học kỳ. Khi em chọn ngày 28/11, hệ thống chuyển sang tuần thi kết thúc môn: Lịch học thường nhật tự động nhường chỗ cho các kỳ thi được gắn cờ đỏ nổi bật.  
> Đồ án của nhóm em không chỉ là một giao diện tĩnh, mà chạy trên nền tảng **ASP.NET Core 8 Web API** kết nối trực tiếp với cơ sở dữ liệu thật **SQL Server 2025**. Nhóm em xin chân thành cảm ơn Thầy và rất mong nhận được những góp ý quý báu của Thầy ạ!"*

---

## 📋 PHẦN VI: PHƯƠNG ÁN KIỂM TRA MẪU THỬ (5W TESTING FRAMEWORK) & 7 VÒNG LẶP CẢI TIẾN

Nhóm đã xây dựng phương án kiểm thử hoàn chỉnh theo phương pháp luận **Tư duy thiết kế (Design Thinking)** dựa trên 5 câu hỏi cốt lõi:
1. **WHAT (Test cái gì?):** Độ chính xác dữ liệu học tập (4 môn học, giảng viên ThS. Nguyễn Quốc Khánh, TS. Lê Thanh Trung); Cơ chế đồng bộ bài tập LMS và đồng bộ 2 chiều sự kiện UEH; Trải nghiệm UI/UX Responsive không tràn màn hình; Tính cá nhân hóa tự động theo MSSV.
2. **WHEN (Test khi nào?):** Vòng lặp cải tiến liên tục (Formative Testing) sau mỗi lần tân sinh viên đóng góp ý kiến; Kiểm thử theo chu kỳ tuần học (16/10) và tuần thi (28/11); Kiểm thử nghiệm thu trước buổi thuyết trình.
3. **WHO (Test cho ai?):** 7 sinh viên K52 ngành Thống kê kinh doanh (End-users); Giảng viên hướng dẫn ThS. Nguyễn Quốc Khánh (Chuyên gia đánh giá); Quản trị viên hệ thống (Admin).
4. **HOW (Test như thế nào?):** Kiểm thử theo 5 kịch bản thực tế (Scenario-Based Testing); Giao thức nghĩ to thành tiếng (Think-Aloud Protocol); Đánh giá thang đo mức độ sử dụng hệ thống SUS (System Usability Scale đạt 88.5/100).
5. **WHERE (Test ở đâu?):** Offline tại phòng máy và Thư viện thông minh UEH Cơ sở B & N; Online qua link Cloudflare Tunnel đa thiết bị (Smartphone, Tablet, Laptop).

### 🔄 7 Vòng lặp cải tiến thực tế từ phản hồi của Tân sinh viên K52:
- **Vòng lặp 1:** Sửa lỗi tràn ngang màn hình trên mobile và màn hình nhỏ ➔ Bổ sung CSS responsive, container co giãn.
- **Vòng lặp 2:** Khắc phục rào cản cài đặt môi trường khi test ➔ Tích hợp đường hầm bảo mật Cloudflare Tunnel online.
- **Vòng lặp 3:** Chuẩn hóa dữ liệu thật ➔ Loại bỏ môn ảo ("Tính toán động kế"), định danh 7 sinh viên nhóm, gán ThS. Nguyễn Quốc Khánh dạy môn Tư duy thiết kế, TS. Lê Thanh Trung dạy môn Tính toán thống kê.
- **Vòng lặp 4:** Khắc phục lỗi font tiếng Việt trong SQL Server ➔ Chuyển đổi sang `NVARCHAR`, thêm tiền tố `N'...'` và mã hóa UTF-8 with BOM.
- **Vòng lặp 5:** Sửa lỗi lệch thời gian trên Hạn chót & Deadline ➔ Lọc nghiêm ngặt 100% deadline nằm trong khoảng `[WeekStart, WeekEnd]` của tuần đang xem.
- **Vòng lặp 6:** Phân định rõ ràng LMS vs. Thông báo trường ➔ Thông báo trường chỉ giữ Học phí, Sinh hoạt công dân, Khám sức khỏe; bài tập môn học tự động đồng bộ từ LMS sang cột Deadline.
- **Vòng lặp 7:** Tối ưu hóa tính cá nhân hóa ➔ Bỏ nút thêm lịch thủ công, tự động gắn nhãn `📌 Dành cho sinh viên`; thiết lập đồng bộ hai chiều thời gian thực giữa Sự kiện UEH và Deadline/Timeline.

---

## ⚙️ PHẦN VII: HƯỚNG DẪN KHỞI CHẠY DỰ ÁN TRÊN MÁY LOCAL

Nếu Thầy hoặc người dùng muốn chạy toàn bộ mã nguồn trên máy tính cá nhân:

### 1. Yêu cầu môi trường
- Hệ điều hành Windows 10/11
- **.NET 8.0 SDK** (kiểm tra bằng lệnh `dotnet --version`)
- **Microsoft SQL Server** (bản Developer/Express/LocalDB) đang chạy tại `localhost`

### 2. Khởi tạo Cơ sở dữ liệu (3 script SQL tuần tự)
Mở cửa sổ Terminal (PowerShell hoặc Command Prompt) tại thư mục dự án và chạy:
```powershell
# Bước 1: Tạo Database UEHStudentDB
sqlcmd -S localhost -E -C -i "database\01_CreateDatabase.sql"

# Bước 2: Tạo toàn bộ cấu trúc 14 bảng và khóa ngoại
sqlcmd -S localhost -E -C -i "database\02_CreateSchema.sql"

# Bước 3: Nạp dữ liệu chuẩn tiếng Việt UTF-8 BOM
sqlcmd -S localhost -E -C -i "database\03_SeedDemoData.sql"
```

### 3. Khởi chạy Ứng dụng
```powershell
dotnet run --project src/UEHStudent.Api/UEHStudent.Api.csproj --launch-profile http
```
Truy cập trình duyệt: **[http://localhost:5000/login.html](http://localhost:5000/login.html)**  
Tài liệu tra cứu Swagger API: **[http://localhost:5000/swagger](http://localhost:5000/swagger)**

---
*Đồ án hoàn thành với sự phối hợp chặt chẽ của tập thể 7 thành viên nhóm — Khoa Toán - Thống kê, Trường Đại học Kinh tế TP. Hồ Chí Minh (UEH).*
