/*
=============================================
  UEH Student My Week — Update Students and Courses
  File: 04_UpdateStudentsAndCourses.sql
  Purpose: 
  - Bo mon Tinh toan dong ke
  - Cap nhat mon Tu duy thiet ke cho giang vien ThS. Nguyen Quoc Khanh
  - Dong bo giang vien cac mon con lai
  - Cap nhat duy nhat 7 sinh vien Khoa Toan thong ke, nganh Thong ke kinh doanh
  - User la MSSV va Password la 4 so cuoi MSSV
=============================================
*/

USE [UEHStudentDB];
GO
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

-- =============================================
-- 1. XOA MON TINH TOAN DONG KE (CourseId = 3, ClassSectionId = 3)
-- =============================================
DELETE FROM [dbo].[StudentCourseEnrollments] WHERE [ClassSectionId] = 3;
DELETE FROM [dbo].[ClassScheduleRules] WHERE [ClassSectionId] = 3;
DELETE FROM [dbo].[ExamSchedules] WHERE [ClassSectionId] = 3;
DELETE FROM [dbo].[ClassSections] WHERE [ClassSectionId] = 3;
DELETE FROM [dbo].[Courses] WHERE [CourseId] = 3;
GO

-- =============================================
-- 2. CAP NHAT TEN MON VA GIANG VIEN CAC MON CON LAI
-- =============================================
UPDATE [dbo].[Courses] SET [CourseName] = N'Tư duy thiết kế' WHERE [CourseId] = 1;
UPDATE [dbo].[Courses] SET [CourseName] = N'Tính toán thống kê' WHERE [CourseId] = 2;
UPDATE [dbo].[Courses] SET [CourseName] = N'Nguyên lý kế toán' WHERE [CourseId] = 4;
UPDATE [dbo].[Courses] SET [CourseName] = N'Quản trị học' WHERE [CourseId] = 5;

-- Mon Tu duy thiet ke (ClassSectionId 1): ThS. Nguyen Quoc Khanh
UPDATE [dbo].[ClassScheduleRules]
SET [Instructor] = N'ThS. Nguyễn Quốc Khánh',
    [Room] = N'N2-508'
WHERE [ClassSectionId] = 1;

-- Mon Tinh toan thong ke (ClassSectionId 2): TS. Le Thanh Trung
UPDATE [dbo].[ClassScheduleRules]
SET [Instructor] = N'TS. Lê Thanh Trung',
    [Room] = N'B1-205'
WHERE [ClassSectionId] = 2;

-- Mon Nguyen ly ke toan (ClassSectionId 4): ThS. Tran Thi Thuong
UPDATE [dbo].[ClassScheduleRules]
SET [Instructor] = N'ThS. Trần Thị Thương'
WHERE [ClassSectionId] = 4;

-- Mon Quan tri hoc (ClassSectionId 5): PGS.TS. Hoang Van Binh
UPDATE [dbo].[ClassScheduleRules]
SET [Instructor] = N'PGS.TS. Hoàng Văn Bình'
WHERE [ClassSectionId] = 5;
GO

-- =============================================
-- 3. XOA DANG KY MON CU & DU LIEU PHU CUA SINH VIEN
-- =============================================
DELETE FROM [dbo].[EventRegistrations];
DELETE FROM [dbo].[StudentSavedAnnouncements];
DELETE FROM [dbo].[PersonalTasks];
DELETE FROM [dbo].[StudentCourseEnrollments];
DELETE FROM [dbo].[AppUsers] WHERE [RoleName] = 'Student';
DELETE FROM [dbo].[Students];
GO

-- =============================================
-- 4. THEM DUY NHAT 7 SINH VIEN (Khoa Toan thong ke - Nganh Thong ke kinh doanh)
-- =============================================
SET IDENTITY_INSERT [dbo].[Students] ON;

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
-- 5. THEM TAI KHOAN DANG NHAP (User: MSSV, Password: 4 so cuoi MSSV)
-- =============================================
SET IDENTITY_INSERT [dbo].[AppUsers] ON;

-- Dam bao co tai khoan Admin
IF NOT EXISTS (SELECT 1 FROM [dbo].[AppUsers] WHERE [Username] = 'admin')
BEGIN
    INSERT INTO [dbo].[AppUsers] ([UserId], [Username], [PasswordHash], [RoleName], [StudentId], [IsActive])
    VALUES (1, N'admin', N'Admin@123', 'Admin', NULL, 1);
END

INSERT INTO [dbo].[AppUsers] ([UserId], [Username], [PasswordHash], [RoleName], [StudentId], [IsActive])
VALUES
    (2, N'31251022130', N'2130', 'Student', 1, 1),
    (3, N'31251023860', N'3860', 'Student', 2, 1),
    (4, N'31251025093', N'5093', 'Student', 3, 1),
    (5, N'31251026911', N'6911', 'Student', 4, 1),
    (6, N'31251023365', N'3365', 'Student', 5, 1),
    (7, N'31251023149', N'3149', 'Student', 6, 1),
    (8, N'31251026534', N'6534', 'Student', 7, 1);

SET IDENTITY_INSERT [dbo].[AppUsers] OFF;
GO

-- =============================================
-- 6. DANG KY MON HOC CHO 7 SINH VIEN
-- Ca 7 sinh vien deu hoc 4 mon: Tu duy thiet ke, Tinh toan thong ke, Nguyen ly ke toan, Quan tri hoc
-- =============================================
INSERT INTO [dbo].[StudentCourseEnrollments] ([StudentId], [ClassSectionId], [Status], [IsActive])
VALUES
    -- Sinh vien 1: Nguyen Minh Khoi
    (1, 1, 'Active', 1), (1, 2, 'Active', 1), (1, 4, 'Active', 1), (1, 5, 'Active', 1),
    -- Sinh vien 2: Nguyen Nhut Thang
    (2, 1, 'Active', 1), (2, 2, 'Active', 1), (2, 4, 'Active', 1), (2, 5, 'Active', 1),
    -- Sinh vien 3: Pham Tran Tuan Khang
    (3, 1, 'Active', 1), (3, 2, 'Active', 1), (3, 4, 'Active', 1), (3, 5, 'Active', 1),
    -- Sinh vien 4: Tra Anh Thu
    (4, 1, 'Active', 1), (4, 2, 'Active', 1), (4, 4, 'Active', 1), (4, 5, 'Active', 1),
    -- Sinh vien 5: Nguyen Thi Thanh Tam
    (5, 1, 'Active', 1), (5, 2, 'Active', 1), (5, 4, 'Active', 1), (5, 5, 'Active', 1),
    -- Sinh vien 6: Nguyen Ngoc Anh Thu
    (6, 1, 'Active', 1), (6, 2, 'Active', 1), (6, 4, 'Active', 1), (6, 5, 'Active', 1),
    -- Sinh vien 7: Nguyen Huu Toan
    (7, 1, 'Active', 1), (7, 2, 'Active', 1), (7, 4, 'Active', 1), (7, 5, 'Active', 1);
GO

-- =============================================
-- 7. TAO DU LIEU MAU MY WEEK (PersonalTasks, EventRegistration, SavedAnnouncement)
-- =============================================
INSERT INTO [dbo].[PersonalTasks] ([StudentId], [Title], [Description], [PlannedDate], [PlannedTime], [DueDate], [IsCompleted])
VALUES
    (1, N'Làm bài tập nhóm môn Tư duy thiết kế', N'Hoàn thành slide thuyết trình cho ThS. Nguyễn Quốc Khánh.', '2026-10-16', '14:00', '2026-10-18', 0),
    (1, N'Ôn tập chương 5 Tính toán thống kê', N'Đọc bài giảng và giải bài tập thực hành R/Python.', '2026-10-17', '19:00', '2026-10-19', 0),
    (2, N'Chuẩn bị case study môn Quản trị học', N'Họp nhóm online thảo luận case study.', '2026-10-16', '15:30', '2026-10-17', 0);

-- Dang ky su kien cho Sinh vien 1
INSERT INTO [dbo].[EventRegistrations] ([StudentId], [EventId], [RegisteredAt], [Status])
VALUES (1, 1, GETDATE(), 'Confirmed');

-- Luu thong bao vao lich cho Sinh vien 1
INSERT INTO [dbo].[StudentSavedAnnouncements] ([StudentId], [AnnouncementId], [PlannedAt], [PersonalNote])
VALUES (1, 1, '2026-11-01', N'Nhắc nhở hạn đăng ký học phần đợt 1');
GO

-- =============================================
-- 8. KIEM TRA KET QUA
-- =============================================
SELECT u.UserId, u.Username, u.RoleName, s.StudentId, s.StudentCode, s.FullName, s.Faculty, s.Major
FROM [dbo].[AppUsers] u
LEFT JOIN [dbo].[Students] s ON u.StudentId = s.StudentId;

SELECT c.CourseCode, c.CourseName, cs.ClassSectionCode, csr.DayOfWeek, csr.StartTime, csr.EndTime, csr.Room, csr.Instructor
FROM [dbo].[Courses] c
JOIN [dbo].[ClassSections] cs ON c.CourseId = cs.CourseId
JOIN [dbo].[ClassScheduleRules] csr ON cs.ClassSectionId = csr.ClassSectionId;
GO
