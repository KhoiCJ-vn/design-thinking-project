/*
=============================================
  UEH Student My Week — Seed Demo Data
  File: 03_SeedDemoData.sql
  Purpose: Chen du lieu mau phuc vu trinh dien.
  
  HUONG DAN:
  - Chay sau khi da thuc thi 01_CreateDatabase.sql va 02_CreateSchema.sql.
  - Du lieu nay la du lieu MINH HOA, khong phai du lieu thuc cua UEH.
  - Mat khau duoc luu duoi dang placeholder hash.
    Backend se tao hash that khi khoi tao tai khoan.
    
  DU LIEU MAU:
  - 1 AcademicTerm (HK cuoi 2026-2027)
  - 5 Courses (hoc phan)
  - 5 ClassSections (lop hoc phan)
  - 2 Students (sinh vien mau)
  - 3 AppUsers (1 Admin + 2 Student)
  - Enrollments cho sinh vien 1 (4 lop) va sinh vien 2 (3 lop)
  - ClassScheduleRules (quy tac lich hoc nhieu tuan)
  - ExamSchedules (lich thi cuoi ky)
  - Announcements (nhieu loai thong bao)
  - AnnouncementTargets
  - Events (su kien UEH)
  - 1 EventRegistration mau
  - 1 PersonalTask mau
  - 1 StudentSavedAnnouncement mau
  
  LUU Y:
  - PasswordHash la placeholder. Backend phai tao hash thuc te.
  - Ngay thang duoc thiet ke quanh thang 10-11/2026 de kiem thu
    viec chon tuan bat ky.
  - Khong sao chep du lieu ca nhan tu anh tham chieu.
=============================================
*/

USE [UEHStudentDB];
GO
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

-- =============================================
-- 1. AcademicTerms
-- =============================================
SET IDENTITY_INSERT [dbo].[AcademicTerms] ON;

IF NOT EXISTS (SELECT 1 FROM [dbo].[AcademicTerms] WHERE [TermId] = 1)
INSERT INTO [dbo].[AcademicTerms] ([TermId], [AcademicYear], [Semester], [StartDate], [EndDate])
VALUES (1, N'2026-2027', N'Hoc ky cuoi', '2026-09-07', '2027-01-17');

SET IDENTITY_INSERT [dbo].[AcademicTerms] OFF;
GO

-- =============================================
-- 2. Courses (Khong co mon Tinh toan dong ke)
-- =============================================
SET IDENTITY_INSERT [dbo].[Courses] ON;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Courses] WHERE [CourseId] = 1)
INSERT INTO [dbo].[Courses] ([CourseId], [CourseCode], [CourseName], [Credits])
VALUES
    (1, N'TEC5500183B', N'Tư duy thiết kế', 3),
    (2, N'MAT58013B',   N'Tính toán thống kê', 3),
    (4, N'PUF604103B',  N'Nguyên lý kế toán', 3),
    (5, N'BUS3012B',    N'Quản trị học', 3);

SET IDENTITY_INSERT [dbo].[Courses] OFF;
GO

-- =============================================
-- 3. ClassSections
-- =============================================
SET IDENTITY_INSERT [dbo].[ClassSections] ON;

IF NOT EXISTS (SELECT 1 FROM [dbo].[ClassSections] WHERE [ClassSectionId] = 1)
INSERT INTO [dbo].[ClassSections] ([ClassSectionId], [ClassSectionCode], [CourseId], [TermId], [GroupName])
VALUES
    (1, N'28C1TEC5500183B01', 1, 1, N'LHP'),
    (2, N'28C1MAT5801360101', 2, 1, N'LHP'),
    (4, N'28C1PUF6041030101', 4, 1, N'LHP'),
    (5, N'28C1BUS30120001',   5, 1, N'LHP');

SET IDENTITY_INSERT [dbo].[ClassSections] OFF;
GO

-- =============================================
-- 4. Students
-- 7 sinh vien Khoa Toan thong ke, nganh Thong ke kinh doanh
-- =============================================
SET IDENTITY_INSERT [dbo].[Students] ON;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Students] WHERE [StudentId] = 1)
INSERT INTO [dbo].[Students] ([StudentId], [StudentCode], [FullName], [Faculty], [Major], [CohortYear], [IsActive])
VALUES
    (1, N'31251022130', N'Nguyễn Minh Khôi',       N'Khoa Toán thống kê', N'Thống kê kinh doanh', 2025, 1),
    (2, N'31251023860', N'Nguyễn Nhựt Thắng',      N'Khoa Toán thống kê', N'Thống kê kinh doanh', 2025, 1),
    (3, N'31251025093', N'Phạm Trần Tuấn Khang',   N'Khoa Toán thống kê', N'Thống kê kinh doanh', 2025, 1),
    (4, N'31251026911', N'Trà Anh Thư',            N'Khoa Toán thống kê', N'Thống kê kinh doanh', 2025, 1),
    (5, N'31251023365', N'Nguyễn Thị Thanh Tâm',    N'Khoa Toán thống kê', N'Thống kê kinh doanh', 2025, 1),
    (6, N'31251023149', N'Nguyễn Ngọc Anh Thư',    N'Khoa Toán thống kê', N'Thống kê kinh doanh', 2025, 1),
    (7, N'31251026534', N'Nguyễn Hữu Toàn',        N'Khoa Toán thống kê', N'Thống kê kinh doanh', 2025, 1);

SET IDENTITY_INSERT [dbo].[Students] OFF;
GO

-- =============================================
-- 5. AppUsers
-- User la MSSV, Password la 4 so cuoi MSSV
-- =============================================
SET IDENTITY_INSERT [dbo].[AppUsers] ON;

IF NOT EXISTS (SELECT 1 FROM [dbo].[AppUsers] WHERE [UserId] = 1)
INSERT INTO [dbo].[AppUsers] ([UserId], [Username], [PasswordHash], [RoleName], [StudentId], [IsActive])
VALUES
    (1, N'admin',       N'Admin@123', 'Admin',   NULL, 1),
    (2, N'31251022130', N'2130',      'Student', 1,    1),
    (3, N'31251023860', N'3860',      'Student', 2,    1),
    (4, N'31251025093', N'5093',      'Student', 3,    1),
    (5, N'31251026911', N'6911',      'Student', 4,    1),
    (6, N'31251023365', N'3365',      'Student', 5,    1),
    (7, N'31251023149', N'3149',      'Student', 6,    1),
    (8, N'31251026534', N'6534',      'Student', 7,    1);

SET IDENTITY_INSERT [dbo].[AppUsers] OFF;
GO

-- =============================================
-- 6. StudentCourseEnrollments
-- Dang ky 4 mon hoc cho ca 7 sinh vien
-- =============================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[StudentCourseEnrollments] WHERE [StudentId] = 1 AND [ClassSectionId] = 1)
INSERT INTO [dbo].[StudentCourseEnrollments] ([StudentId], [ClassSectionId], [Status], [IsActive])
VALUES
    (1, 1, 'Active', 1), (1, 2, 'Active', 1), (1, 4, 'Active', 1), (1, 5, 'Active', 1),
    (2, 1, 'Active', 1), (2, 2, 'Active', 1), (2, 4, 'Active', 1), (2, 5, 'Active', 1),
    (3, 1, 'Active', 1), (3, 2, 'Active', 1), (3, 4, 'Active', 1), (3, 5, 'Active', 1),
    (4, 1, 'Active', 1), (4, 2, 'Active', 1), (4, 4, 'Active', 1), (4, 5, 'Active', 1),
    (5, 1, 'Active', 1), (5, 2, 'Active', 1), (5, 4, 'Active', 1), (5, 5, 'Active', 1),
    (6, 1, 'Active', 1), (6, 2, 'Active', 1), (6, 4, 'Active', 1), (6, 5, 'Active', 1),
    (7, 1, 'Active', 1), (7, 2, 'Active', 1), (7, 4, 'Active', 1), (7, 5, 'Active', 1);
GO

-- =============================================
-- 7. ClassScheduleRules
-- Quy tac lich hoc dinh ky
-- =============================================

-- Tu duy thiet ke (ClassSection 1): Thu 2, Tiet 2-5 (07:30-11:30) - ThS. Nguyen Quoc Khanh
IF NOT EXISTS (SELECT 1 FROM [dbo].[ClassScheduleRules] WHERE [ClassSectionId] = 1 AND [DayOfWeek] = 1)
INSERT INTO [dbo].[ClassScheduleRules] 
    ([ClassSectionId], [DayOfWeek], [StartTime], [EndTime], [Room], [Instructor], [EffectiveStartDate], [EffectiveEndDate])
VALUES
    (1, 1, '07:30', '11:30', N'N2-508',  N'ThS. Nguyễn Quốc Khánh', '2026-09-07', '2027-01-17');

-- Tinh toan thong ke (ClassSection 2): Thu 2, Tiet 8-12 (12:30-16:30) - TS. Le Thanh Trung
IF NOT EXISTS (SELECT 1 FROM [dbo].[ClassScheduleRules] WHERE [ClassSectionId] = 2 AND [DayOfWeek] = 1)
INSERT INTO [dbo].[ClassScheduleRules]
    ([ClassSectionId], [DayOfWeek], [StartTime], [EndTime], [Room], [Instructor], [EffectiveStartDate], [EffectiveEndDate])
VALUES
    (2, 1, '12:30', '16:30', N'B1-205',  N'TS. Lê Thanh Trung', '2026-09-07', '2027-01-17');

-- Nguyen ly ke toan (ClassSection 4): Thu 4, Tiet 2-5 (07:30-11:30) - ThS. Tran Thi Thuong
IF NOT EXISTS (SELECT 1 FROM [dbo].[ClassScheduleRules] WHERE [ClassSectionId] = 4 AND [DayOfWeek] = 3)
INSERT INTO [dbo].[ClassScheduleRules]
    ([ClassSectionId], [DayOfWeek], [StartTime], [EndTime], [Room], [Instructor], [EffectiveStartDate], [EffectiveEndDate])
VALUES
    (4, 3, '07:30', '11:30', N'N2-507',  N'ThS. Trần Thị Thương', '2026-09-07', '2027-01-17');

-- Nguyen ly ke toan (ClassSection 4): Thu 5, Tiet 2-3 (07:30-09:30) - ThS. Tran Thi Thuong
IF NOT EXISTS (SELECT 1 FROM [dbo].[ClassScheduleRules] WHERE [ClassSectionId] = 4 AND [DayOfWeek] = 4)
INSERT INTO [dbo].[ClassScheduleRules]
    ([ClassSectionId], [DayOfWeek], [StartTime], [EndTime], [Room], [Instructor], [EffectiveStartDate], [EffectiveEndDate])
VALUES
    (4, 4, '07:30', '09:30', N'N2-507',  N'ThS. Trần Thị Thương', '2026-09-07', '2027-01-17');

-- Quan tri hoc (ClassSection 5): Thu 6, Tiet 2-5 (07:30-11:30) - PGS.TS. Hoang Van Binh
IF NOT EXISTS (SELECT 1 FROM [dbo].[ClassScheduleRules] WHERE [ClassSectionId] = 5 AND [DayOfWeek] = 5)
INSERT INTO [dbo].[ClassScheduleRules]
    ([ClassSectionId], [DayOfWeek], [StartTime], [EndTime], [Room], [Instructor], [EffectiveStartDate], [EffectiveEndDate])
VALUES
    (5, 5, '07:30', '11:30', N'N3-201',  N'PGS.TS. Hoàng Văn Bình', '2026-09-07', '2027-01-17');
GO

-- =============================================
-- 8. ExamSchedules
-- Lich thi cuoi hoc ky, cuoi thang 11 - dau thang 12/2026
-- =============================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[ExamSchedules] WHERE [ClassSectionId] = 1)
INSERT INTO [dbo].[ExamSchedules] 
    ([ClassSectionId], [ExamDate], [StartTime], [EndTime], [Room], [Location], [ExamType], [ExamForm], [Notes])
VALUES
    (1, '2026-11-30', '08:00', NULL,    NULL,     N'UEH Nguyễn Văn Linh',  N'Thi cuoi hoc phan', N'Du an',                        N'Thi het thuc hoc phan'),
    (2, '2026-11-29', '08:30', '10:00', N'N2-N24', N'UEH Nguyen Van Linh',  N'Thi cuoi hoc phan', N'Trac nghiem va tu luan tu trung', N'Thi het thuc hoc phan'),
    (4, '2026-11-27', '13:30', '15:30', N'N2-301', N'UEH Nguyen Van Linh',  N'Thi cuoi hoc phan', N'Trac nghiem',                NULL);

IF NOT EXISTS (SELECT 1 FROM [dbo].[ExamSchedules] WHERE [ClassSectionId] = 5)
INSERT INTO [dbo].[ExamSchedules]
    ([ClassSectionId], [ExamDate], [StartTime], [EndTime], [Room], [Location], [ExamType], [ExamForm])
VALUES
    (5, '2026-12-01', '08:30', '10:30', N'N3-101', N'UEH Nguyen Van Linh', N'Thi cuoi hoc phan', N'Tu luan');
GO

-- =============================================
-- 9. Announcements
-- Nhieu loai thong bao, nhieu muc do uu tien
-- =============================================
SET IDENTITY_INSERT [dbo].[Announcements] ON;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Announcements] WHERE [AnnouncementId] = 1)
INSERT INTO [dbo].[Announcements] 
    ([AnnouncementId], [Title], [Content], [Summary], [Issuer], [Category], [PublishedAt], [ExpiresAt], [DeadlineAt], [Priority])
VALUES
    (1,
     N'THONG BAO DANG KY HOC PHAN HOC KY DAU NAM 2027',
     N'Sinh vien toan truong dang ky hoc phan cho Hoc ky dau nam hoc 2027 tu ngay 01/11/2026 den 15/11/2026. Chi tiet xem tai cong thong tin sinh vien.',
     N'Dang ky hoc phan HK dau 2027 tu 01/11 den 15/11/2026.',
     N'Cao Thi Xuan Tam',
     N'Dang ky hoc phan',
     '2026-10-14 08:00:00',
     '2026-11-16 00:00:00',
     '2026-11-15 23:59:59',
     'High'),
    
    (2,
     N'Thong bao ve viec nop chung chi tieng Anh quoc te dot 4 nam 2026',
     N'Sinh vien nop chung chi tieng Anh quoc te xet chuan dau ra theo quy dinh. Han nop: 30/10/2026.',
     N'Nop chung chi tieng Anh dot 4 nam 2026. Han: 30/10/2026.',
     N'Ngo Thi Lan',
     N'Nop chung chi',
     '2026-10-07 09:00:00',
     '2026-10-31 00:00:00',
     '2026-10-30 23:59:59',
     'High'),
    
    (3,
     N'Thong bao lich thi hoc ky cuoi 2026 - EHCQ',
     N'Lich thi hoc ky cuoi 2026 da duoc cong bo. Sinh vien kiem tra lich thi tren cong thong tin.',
     N'Lich thi HK cuoi 2026 da cong bo.',
     N'Thai Ngoc Huong',
     N'Lich thi',
     '2026-10-05 10:00:00',
     '2026-12-15 00:00:00',
     NULL,
     'Normal'),
    
    (4,
     N'Ke hoach dang ky hoc phan hoc ky cuoi khoa 52 DHCQ',
     N'Ke hoach dang ky hoc phan ap dung cho sinh vien chinh quy khoa 52.',
     N'Ke hoach DKHP cho khoa 52.',
     N'Thai Ngoc Huong',
     N'Dang ky hoc phan',
     '2026-09-02 08:00:00',
     '2026-10-30 00:00:00',
     NULL,
     'Normal'),
    
    (5,
     N'HOC KY CUOI 2026 - THONG BAO LICH SINH HOAT CONG DAN',
     N'Sinh vien tham gia sinh hoat cong dan dau hoc ky theo lich cu the cua tung khoa.',
     N'Sinh hoat cong dan dau HK cuoi 2026.',
     N'Truong Nhat Uyen',
     N'Hoat dong sinh vien',
     '2026-09-18 08:00:00',
     '2026-10-31 00:00:00',
     '2026-10-25 23:59:59',
     'Normal'),
    
    (6,
     N'Thong bao dieu chinh lich hoc tuan 17 (19/10 - 25/10/2026)',
     N'Do su kien toan truong, mot so lop hoc phan se duoc dieu chinh lich. Vui long kiem tra lich hoc cap nhat.',
     N'Dieu chinh lich hoc tuan 17 do su kien toan truong.',
     N'Cao Thi Xuan Tam',
     N'Lich hoc',
     '2026-10-15 14:00:00',
     '2026-10-25 00:00:00',
     NULL,
     'Urgent'),
    
    (7,
     N'Thong bao ve thoi han hoan thanh thu tuc hoc vu',
     N'Sinh vien can hoan thanh cac thu tuc hoc vu con ton dong truoc ngay 20/10/2026.',
     N'Han hoan thanh thu tuc hoc vu: 20/10/2026.',
     N'Truong Nhat Uyen',
     N'Hoc vu va thu tuc',
     '2026-10-10 08:00:00',
     '2026-10-21 00:00:00',
     '2026-10-20 23:59:59',
     'High'),
    
    (8,
     N'Chuong trinh kham suc khoe sinh vien nam 2026',
     N'Chuong trinh kham suc khoe dinh ky cho sinh vien nam 2026 dien ra tu ngay 20/10 den 31/10/2026.',
     N'Kham suc khoe dinh ky 20/10 - 31/10/2026.',
     N'Nguyen Phuong Thao',
     N'Thong bao chung',
     '2026-10-08 09:00:00',
     '2026-11-01 00:00:00',
     '2026-10-31 23:59:59',
     'Normal');

SET IDENTITY_INSERT [dbo].[Announcements] OFF;
GO

-- =============================================
-- 10. AnnouncementTargets
-- =============================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[AnnouncementTargets] WHERE [AnnouncementId] = 1)
INSERT INTO [dbo].[AnnouncementTargets] ([AnnouncementId], [TargetType], [TargetValue])
VALUES
    (1, 'All',     NULL),   -- DKHP: toan truong
    (2, 'All',     NULL),   -- Chung chi TA: toan truong
    (3, 'All',     NULL),   -- Lich thi: toan truong
    (4, 'Cohort',  N'52'),  -- DKHP khoa 52
    (5, 'All',     NULL),   -- SHCD: toan truong
    (6, 'All',     NULL),   -- Dieu chinh lich hoc: toan truong
    (7, 'All',     NULL),   -- Thu tuc hoc vu: toan truong
    (8, 'All',     NULL);   -- Kham suc khoe: toan truong
GO

-- =============================================
-- 11. Events — Su kien UEH
-- Cac su kien nam trong khoang tuan 12/10 - 01/11/2026
-- =============================================
SET IDENTITY_INSERT [dbo].[Events] ON;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Events] WHERE [EventId] = 1)
INSERT INTO [dbo].[Events]
    ([EventId], [Title], [Description], [Category], [StartDateTime], [EndDateTime], [Location], [RegistrationDeadline], [MaxParticipants], [TargetAudience])
VALUES
    (1,
     N'Workshop: Ky nang thuyet trinh hieu qua',
     N'Workshop trang bi ky nang thuyet trinh chuyen nghiep, phu hop cho sinh vien cac nganh. Dien gia: ThS. Nguyen Minh Duc.',
     N'Workshop',
     '2026-10-17 09:00:00', '2026-10-17 11:30:00',
     N'Hoi truong A, Co so Nguyen Van Linh',
     '2026-10-16 18:00:00',
     100,
     N'Toan truong'),
    
    (2,
     N'UEH Career Fair 2026',
     N'Ngay hoi viec lam UEH 2026 voi su tham gia cua hon 50 doanh nghiep. Co hoi thuc tap va tuyen dung truc tiep.',
     N'Tuyen dung',
     '2026-10-22 08:00:00', '2026-10-22 17:00:00',
     N'Khu vuc san truong, Co so Nguyen Van Linh',
     '2026-10-20 23:59:59',
     500,
     N'Toan truong'),
    
    (3,
     N'Seminar: Chuyen doi so trong ke toan',
     N'Seminar chuyen de ve ung dung cong nghe so trong linh vuc ke toan va kiem toan.',
     N'Seminar',
     '2026-10-15 14:00:00', '2026-10-15 16:30:00',
     N'Phong N2-301, Co so Nguyen Van Linh',
     '2026-10-14 23:59:59',
     80,
     N'Khoa Ke toan'),
    
    (4,
     N'Giai chay UEH Marathon 2026',
     N'Giai chay bo thuong nien cua UEH, huong ung phong trao ren luyen suc khoe.',
     N'The thao',
     '2026-10-25 06:00:00', '2026-10-25 10:00:00',
     N'San van dong khu vuc Nguyen Van Linh',
     '2026-10-23 23:59:59',
     300,
     N'Toan truong'),
    
    (5,
     N'Workshop: Python cho phan tich du lieu',
     N'Workshop thuc hanh Python co ban cho phan tich du lieu, danh cho sinh vien chua co kinh nghiem lap trinh.',
     N'Workshop',
     '2026-10-30 09:00:00', '2026-10-30 12:00:00',
     N'Phong may tinh N4-201',
     '2026-10-28 23:59:59',
     40,
     N'Toan truong');

SET IDENTITY_INSERT [dbo].[Events] OFF;
GO

-- =============================================
-- 12. EventRegistrations
-- Sinh vien 1 (Tran Van Hoang) da dang ky su kien 1 (Workshop thuyet trinh)
-- =============================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[EventRegistrations] WHERE [StudentId] = 1 AND [EventId] = 1)
INSERT INTO [dbo].[EventRegistrations] ([StudentId], [EventId], [Status])
VALUES (1, 1, 'Registered');
GO

-- =============================================
-- 13. PersonalTasks
-- Viec ca nhan mau cua sinh vien 1
-- =============================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[PersonalTasks] WHERE [StudentId] = 1 AND [Title] = N'Lam bai tap nhom mon Tu duy thiet ke')
INSERT INTO [dbo].[PersonalTasks] 
    ([StudentId], [Title], [Description], [PlannedDate], [PlannedTime], [DueDate], [IsCompleted])
VALUES
    (1, N'Lam bai tap nhom mon Tu duy thiet ke', 
        N'Hoan thanh phan cua minh trong bai tap nhom so 3.', 
        '2026-10-16', '14:00', '2026-10-18', 0),
    (1, N'On tap chuong 5 Tinh toan thong ke', 
        N'Doc lai bai giang va lam bai tap chuong 5.', 
        '2026-10-17', '19:00', '2026-10-19', 0),
    (1, N'Nop don xin xac nhan sinh vien',
        NULL,
        '2026-10-14', '10:00', '2026-10-15', 1);
GO

-- =============================================
-- 14. StudentSavedAnnouncements
-- Sinh vien 1 da luu thong bao 1 (DKHP) vao lich ca nhan
-- =============================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[StudentSavedAnnouncements] WHERE [StudentId] = 1 AND [AnnouncementId] = 1)
INSERT INTO [dbo].[StudentSavedAnnouncements]
    ([StudentId], [AnnouncementId], [PlannedAt], [PersonalNote])
VALUES
    (1, 1, '2026-11-01', N'Bat dau dang ky hoc phan ngay dau tien'),
    (1, 2, '2026-10-25', N'Chuan bi ho so nop chung chi TA');
GO

PRINT N'';
PRINT N'=============================================';
PRINT N'  Demo data seeded successfully!';
PRINT N'  - 1 Academic Term';
PRINT N'  - 5 Courses, 5 Class Sections';
PRINT N'  - 2 Students, 3 Users (1 Admin + 2 Student)';
PRINT N'  - 7 Enrollments';
PRINT N'  - 6 Schedule Rules';
PRINT N'  - 5 Exam Schedules';
PRINT N'  - 8 Announcements';
PRINT N'  - 5 Events, 1 Event Registration';
PRINT N'  - 3 Personal Tasks';
PRINT N'  - 2 Saved Announcements';
PRINT N'=============================================';
GO
