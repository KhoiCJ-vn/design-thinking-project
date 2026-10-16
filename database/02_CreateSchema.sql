/*
=============================================
  UEH Student My Week — Create Schema
  File: 02_CreateSchema.sql
  Purpose: Tao tat ca bang, khoa chinh, khoa ngoai,
           unique constraints va indexes.
           
  HUONG DAN:
  - Chay sau khi da thuc thi 01_CreateDatabase.sql.
  - Chay tren database UEHStudentDB.
  
  CAC BANG (14):
  1.  AcademicTerms         - Nam hoc & hoc ky
  2.  Courses               - Danh muc hoc phan
  3.  ClassSections         - Lop hoc phan
  4.  Students              - Ho so sinh vien
  5.  AppUsers              - Tai khoan dang nhap
  6.  StudentCourseEnrollments - Dang ky lop hoc phan
  7.  ClassScheduleRules    - Quy tac lich hoc dinh ky
  8.  ExamSchedules         - Lich thi cu the
  9.  Announcements         - Thong bao chung
  10. AnnouncementTargets   - Doi tuong nhan thong bao
  11. Events                - Su kien UEH
  12. EventRegistrations    - Dang ky su kien
  13. PersonalTasks         - Viec ca nhan sinh vien
  14. StudentSavedAnnouncements - Thong bao duoc luu vao lich

  LUU Y: Khong chua lenh DROP TABLE hoac TRUNCATE.
=============================================
*/

USE [UEHStudentDB];
GO

-- =============================================
-- 1. AcademicTerms — Nam hoc va hoc ky
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AcademicTerms]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[AcademicTerms]
    (
        [TermId]        INT IDENTITY(1,1) NOT NULL,
        [AcademicYear]  NVARCHAR(20)      NOT NULL,  -- VD: '2026-2027'
        [Semester]      NVARCHAR(30)      NOT NULL,  -- VD: 'Hoc ky cuoi', 'Hoc ky dau'
        [StartDate]     DATE              NOT NULL,
        [EndDate]       DATE              NOT NULL,
        [IsActive]      BIT               NOT NULL DEFAULT 1,
        [CreatedAt]     DATETIME2(7)      NOT NULL DEFAULT SYSUTCDATETIME(),

        CONSTRAINT [PK_AcademicTerms] PRIMARY KEY CLUSTERED ([TermId]),
        CONSTRAINT [UQ_AcademicTerms_YearSemester] UNIQUE ([AcademicYear], [Semester])
    );
    PRINT N'Table [AcademicTerms] created.';
END
GO

-- =============================================
-- 2. Courses — Danh muc hoc phan
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Courses]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[Courses]
    (
        [CourseId]      INT IDENTITY(1,1) NOT NULL,
        [CourseCode]    NVARCHAR(30)      NOT NULL,  -- VD: 'MAT58013B'
        [CourseName]    NVARCHAR(200)     NOT NULL,  -- VD: 'Tinh toan thong ke'
        [Credits]       INT               NOT NULL DEFAULT 3,
        [IsActive]      BIT               NOT NULL DEFAULT 1,
        [CreatedAt]     DATETIME2(7)      NOT NULL DEFAULT SYSUTCDATETIME(),

        CONSTRAINT [PK_Courses] PRIMARY KEY CLUSTERED ([CourseId]),
        CONSTRAINT [UQ_Courses_CourseCode] UNIQUE ([CourseCode])
    );
    PRINT N'Table [Courses] created.';
END
GO

-- =============================================
-- 3. ClassSections — Lop hoc phan
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ClassSections]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[ClassSections]
    (
        [ClassSectionId]    INT IDENTITY(1,1) NOT NULL,
        [ClassSectionCode]  NVARCHAR(50)      NOT NULL,  -- VD: '28C1MAT58013B01'
        [CourseId]          INT               NOT NULL,
        [TermId]            INT               NOT NULL,
        [GroupName]         NVARCHAR(20)      NULL,       -- VD: 'LHP 01', 'N2'
        [MaxStudents]       INT               NOT NULL DEFAULT 50,
        [IsActive]          BIT               NOT NULL DEFAULT 1,
        [CreatedAt]         DATETIME2(7)      NOT NULL DEFAULT SYSUTCDATETIME(),

        CONSTRAINT [PK_ClassSections] PRIMARY KEY CLUSTERED ([ClassSectionId]),
        CONSTRAINT [UQ_ClassSections_Code] UNIQUE ([ClassSectionCode]),
        CONSTRAINT [FK_ClassSections_Courses] FOREIGN KEY ([CourseId]) 
            REFERENCES [dbo].[Courses]([CourseId]),
        CONSTRAINT [FK_ClassSections_AcademicTerms] FOREIGN KEY ([TermId]) 
            REFERENCES [dbo].[AcademicTerms]([TermId])
    );
    PRINT N'Table [ClassSections] created.';
END
GO

-- =============================================
-- 4. Students — Ho so sinh vien
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Students]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[Students]
    (
        [StudentId]     INT IDENTITY(1,1)   NOT NULL,
        [StudentCode]   NVARCHAR(20)        NOT NULL,  -- VD: '31251000001'
        [FullName]      NVARCHAR(100)       NOT NULL,
        [Faculty]       NVARCHAR(100)       NULL,      -- VD: 'Khoa Cong nghe thong tin kinh doanh'
        [Major]         NVARCHAR(100)       NULL,      -- VD: 'He thong thong tin quan ly'
        [CohortYear]    INT                 NULL,      -- VD: 2026
        [IsActive]      BIT                 NOT NULL DEFAULT 1,
        [CreatedAt]     DATETIME2(7)        NOT NULL DEFAULT SYSUTCDATETIME(),

        CONSTRAINT [PK_Students] PRIMARY KEY CLUSTERED ([StudentId]),
        CONSTRAINT [UQ_Students_StudentCode] UNIQUE ([StudentCode])
    );
    PRINT N'Table [Students] created.';
END
GO

-- =============================================
-- 5. AppUsers — Tai khoan dang nhap
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AppUsers]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[AppUsers]
    (
        [UserId]        INT IDENTITY(1,1)   NOT NULL,
        [Username]      NVARCHAR(50)        NOT NULL,
        [PasswordHash]  NVARCHAR(500)       NOT NULL,
        [RoleName]      NVARCHAR(20)        NOT NULL DEFAULT 'Student',  -- 'Admin' hoac 'Student'
        [StudentId]     INT                 NULL,   -- NULL neu la Admin
        [IsActive]      BIT                 NOT NULL DEFAULT 1,
        [CreatedAt]     DATETIME2(7)        NOT NULL DEFAULT SYSUTCDATETIME(),
        [LastLoginAt]   DATETIME2(7)        NULL,

        CONSTRAINT [PK_AppUsers] PRIMARY KEY CLUSTERED ([UserId]),
        CONSTRAINT [UQ_AppUsers_Username] UNIQUE ([Username]),
        CONSTRAINT [FK_AppUsers_Students] FOREIGN KEY ([StudentId]) 
            REFERENCES [dbo].[Students]([StudentId]),
        CONSTRAINT [CK_AppUsers_RoleName] CHECK ([RoleName] IN ('Admin', 'Student'))
    );
    PRINT N'Table [AppUsers] created.';
END
GO

-- =============================================
-- 6. StudentCourseEnrollments — Dang ky lop hoc phan
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StudentCourseEnrollments]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[StudentCourseEnrollments]
    (
        [EnrollmentId]      INT IDENTITY(1,1) NOT NULL,
        [StudentId]         INT               NOT NULL,
        [ClassSectionId]    INT               NOT NULL,
        [EnrolledAt]        DATETIME2(7)      NOT NULL DEFAULT SYSUTCDATETIME(),
        [Status]            NVARCHAR(20)      NOT NULL DEFAULT 'Active',  -- Active, Withdrawn, Completed
        [IsActive]          BIT               NOT NULL DEFAULT 1,

        CONSTRAINT [PK_StudentCourseEnrollments] PRIMARY KEY CLUSTERED ([EnrollmentId]),
        CONSTRAINT [UQ_StudentCourseEnrollments] UNIQUE ([StudentId], [ClassSectionId]),
        CONSTRAINT [FK_SCE_Students] FOREIGN KEY ([StudentId]) 
            REFERENCES [dbo].[Students]([StudentId]),
        CONSTRAINT [FK_SCE_ClassSections] FOREIGN KEY ([ClassSectionId]) 
            REFERENCES [dbo].[ClassSections]([ClassSectionId])
    );
    PRINT N'Table [StudentCourseEnrollments] created.';
END
GO

-- =============================================
-- 7. ClassScheduleRules — Quy tac lich hoc dinh ky
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ClassScheduleRules]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[ClassScheduleRules]
    (
        [ScheduleRuleId]    INT IDENTITY(1,1) NOT NULL,
        [ClassSectionId]    INT               NOT NULL,
        [DayOfWeek]         TINYINT           NOT NULL,  -- 1=Mon, 2=Tue,..., 7=Sun
        [StartTime]         TIME(0)           NOT NULL,  -- VD: '07:30'
        [EndTime]           TIME(0)           NOT NULL,  -- VD: '11:30'
        [Room]              NVARCHAR(50)      NULL,      -- VD: 'N2-508'
        [Instructor]        NVARCHAR(100)     NULL,
        [EffectiveStartDate] DATE             NOT NULL,  -- Ngay bat dau ap dung
        [EffectiveEndDate]  DATE              NOT NULL,  -- Ngay ket thuc ap dung
        [WeekPattern]       NVARCHAR(20)      NULL DEFAULT 'Every',  -- Every, Odd, Even
        [IsActive]          BIT               NOT NULL DEFAULT 1,
        [CreatedAt]         DATETIME2(7)      NOT NULL DEFAULT SYSUTCDATETIME(),

        CONSTRAINT [PK_ClassScheduleRules] PRIMARY KEY CLUSTERED ([ScheduleRuleId]),
        CONSTRAINT [FK_CSR_ClassSections] FOREIGN KEY ([ClassSectionId]) 
            REFERENCES [dbo].[ClassSections]([ClassSectionId]),
        CONSTRAINT [CK_CSR_DayOfWeek] CHECK ([DayOfWeek] BETWEEN 1 AND 7),
        CONSTRAINT [CK_CSR_TimeRange] CHECK ([StartTime] < [EndTime]),
        CONSTRAINT [CK_CSR_DateRange] CHECK ([EffectiveStartDate] <= [EffectiveEndDate])
    );
    PRINT N'Table [ClassScheduleRules] created.';
END
GO

-- =============================================
-- 8. ExamSchedules — Lich thi cu the
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ExamSchedules]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[ExamSchedules]
    (
        [ExamScheduleId]    INT IDENTITY(1,1) NOT NULL,
        [ClassSectionId]    INT               NOT NULL,
        [ExamDate]          DATE              NOT NULL,
        [StartTime]         TIME(0)           NOT NULL,
        [EndTime]           TIME(0)           NULL,
        [Room]              NVARCHAR(50)      NULL,
        [Location]          NVARCHAR(200)     NULL,
        [ExamType]          NVARCHAR(50)      NOT NULL DEFAULT N'Thi cuoi hoc phan',
        [ExamForm]          NVARCHAR(100)     NULL,    -- VD: 'Du an', 'Trac nghiem va tu luan'
        [Notes]             NVARCHAR(500)     NULL,
        [IsActive]          BIT               NOT NULL DEFAULT 1,
        [CreatedAt]         DATETIME2(7)      NOT NULL DEFAULT SYSUTCDATETIME(),

        CONSTRAINT [PK_ExamSchedules] PRIMARY KEY CLUSTERED ([ExamScheduleId]),
        CONSTRAINT [FK_ES_ClassSections] FOREIGN KEY ([ClassSectionId]) 
            REFERENCES [dbo].[ClassSections]([ClassSectionId])
    );
    PRINT N'Table [ExamSchedules] created.';
END
GO

-- =============================================
-- 9. Announcements — Thong bao chung
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Announcements]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[Announcements]
    (
        [AnnouncementId]    INT IDENTITY(1,1) NOT NULL,
        [Title]             NVARCHAR(500)     NOT NULL,
        [Content]           NVARCHAR(MAX)     NULL,
        [Summary]           NVARCHAR(500)     NULL,
        [Issuer]            NVARCHAR(100)     NOT NULL,
        [Category]          NVARCHAR(50)      NOT NULL DEFAULT N'Thong bao chung',
        [PublishedAt]       DATETIME2(7)      NOT NULL DEFAULT SYSUTCDATETIME(),
        [ExpiresAt]         DATETIME2(7)      NULL,
        [DeadlineAt]        DATETIME2(7)      NULL,
        [Priority]          NVARCHAR(20)      NOT NULL DEFAULT 'Normal',  -- Low, Normal, High, Urgent
        [IsActive]          BIT               NOT NULL DEFAULT 1,
        [CreatedAt]         DATETIME2(7)      NOT NULL DEFAULT SYSUTCDATETIME(),

        CONSTRAINT [PK_Announcements] PRIMARY KEY CLUSTERED ([AnnouncementId]),
        CONSTRAINT [CK_Ann_Priority] CHECK ([Priority] IN ('Low', 'Normal', 'High', 'Urgent'))
    );
    PRINT N'Table [Announcements] created.';
END
GO

-- =============================================
-- 10. AnnouncementTargets — Doi tuong nhan thong bao
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AnnouncementTargets]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[AnnouncementTargets]
    (
        [TargetId]          INT IDENTITY(1,1) NOT NULL,
        [AnnouncementId]    INT               NOT NULL,
        [TargetType]        NVARCHAR(30)      NOT NULL DEFAULT 'All',
            -- 'All', 'Faculty', 'Major', 'Cohort', 'Student'
        [TargetValue]       NVARCHAR(100)     NULL,
            -- NULL cho 'All', ten khoa cho 'Faculty', 
            -- ma sv cho 'Student', v.v.
        
        CONSTRAINT [PK_AnnouncementTargets] PRIMARY KEY CLUSTERED ([TargetId]),
        CONSTRAINT [FK_AT_Announcements] FOREIGN KEY ([AnnouncementId]) 
            REFERENCES [dbo].[Announcements]([AnnouncementId])
    );
    PRINT N'Table [AnnouncementTargets] created.';
END
GO

-- =============================================
-- 11. Events — Su kien UEH
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Events]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[Events]
    (
        [EventId]               INT IDENTITY(1,1) NOT NULL,
        [Title]                 NVARCHAR(300)     NOT NULL,
        [Description]           NVARCHAR(MAX)     NULL,
        [Category]              NVARCHAR(50)      NOT NULL DEFAULT N'Hoat dong',
        [StartDateTime]         DATETIME2(7)      NOT NULL,
        [EndDateTime]           DATETIME2(7)      NOT NULL,
        [Location]              NVARCHAR(200)     NULL,
        [RegistrationDeadline]  DATETIME2(7)      NULL,
        [MaxParticipants]       INT               NULL,
        [TargetAudience]        NVARCHAR(100)     NULL DEFAULT N'Toan truong',
        [IsActive]              BIT               NOT NULL DEFAULT 1,
        [CreatedAt]             DATETIME2(7)      NOT NULL DEFAULT SYSUTCDATETIME(),

        CONSTRAINT [PK_Events] PRIMARY KEY CLUSTERED ([EventId]),
        CONSTRAINT [CK_Events_DateRange] CHECK ([StartDateTime] < [EndDateTime])
    );
    PRINT N'Table [Events] created.';
END
GO

-- =============================================
-- 12. EventRegistrations — Dang ky su kien
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EventRegistrations]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[EventRegistrations]
    (
        [RegistrationId]    INT IDENTITY(1,1) NOT NULL,
        [StudentId]         INT               NOT NULL,
        [EventId]           INT               NOT NULL,
        [RegisteredAt]      DATETIME2(7)      NOT NULL DEFAULT SYSUTCDATETIME(),
        [Status]            NVARCHAR(20)      NOT NULL DEFAULT 'Registered',
            -- Registered, Cancelled, Attended

        CONSTRAINT [PK_EventRegistrations] PRIMARY KEY CLUSTERED ([RegistrationId]),
        CONSTRAINT [UQ_EventRegistrations] UNIQUE ([StudentId], [EventId]),
        CONSTRAINT [FK_ER_Students] FOREIGN KEY ([StudentId]) 
            REFERENCES [dbo].[Students]([StudentId]),
        CONSTRAINT [FK_ER_Events] FOREIGN KEY ([EventId]) 
            REFERENCES [dbo].[Events]([EventId])
    );
    PRINT N'Table [EventRegistrations] created.';
END
GO

-- =============================================
-- 13. PersonalTasks — Viec ca nhan sinh vien
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PersonalTasks]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[PersonalTasks]
    (
        [TaskId]            INT IDENTITY(1,1) NOT NULL,
        [StudentId]         INT               NOT NULL,
        [Title]             NVARCHAR(200)     NOT NULL,
        [Description]       NVARCHAR(500)     NULL,
        [PlannedDate]       DATE              NOT NULL,
        [PlannedTime]       TIME(0)           NULL,
        [DueDate]           DATE              NULL,
        [IsCompleted]       BIT               NOT NULL DEFAULT 0,
        [CompletedAt]       DATETIME2(7)      NULL,
        [CreatedAt]         DATETIME2(7)      NOT NULL DEFAULT SYSUTCDATETIME(),
        [UpdatedAt]         DATETIME2(7)      NULL,

        CONSTRAINT [PK_PersonalTasks] PRIMARY KEY CLUSTERED ([TaskId]),
        CONSTRAINT [FK_PT_Students] FOREIGN KEY ([StudentId]) 
            REFERENCES [dbo].[Students]([StudentId])
    );
    PRINT N'Table [PersonalTasks] created.';
END
GO

-- =============================================
-- 14. StudentSavedAnnouncements — Thong bao duoc luu vao lich
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StudentSavedAnnouncements]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[StudentSavedAnnouncements]
    (
        [SavedAnnouncementId]   INT IDENTITY(1,1) NOT NULL,
        [StudentId]             INT               NOT NULL,
        [AnnouncementId]        INT               NOT NULL,
        [PlannedAt]             DATE              NOT NULL,  -- Ngay sinh vien chon de theo doi
        [PersonalNote]          NVARCHAR(300)     NULL,
        [CreatedAt]             DATETIME2(7)      NOT NULL DEFAULT SYSUTCDATETIME(),

        CONSTRAINT [PK_StudentSavedAnnouncements] PRIMARY KEY CLUSTERED ([SavedAnnouncementId]),
        CONSTRAINT [UQ_SSA_StudentAnnouncement] UNIQUE ([StudentId], [AnnouncementId]),
        CONSTRAINT [FK_SSA_Students] FOREIGN KEY ([StudentId]) 
            REFERENCES [dbo].[Students]([StudentId]),
        CONSTRAINT [FK_SSA_Announcements] FOREIGN KEY ([AnnouncementId]) 
            REFERENCES [dbo].[Announcements]([AnnouncementId])
    );
    PRINT N'Table [StudentSavedAnnouncements] created.';
END
GO

-- =============================================
-- INDEXES — Tang hieu suat truy van
-- =============================================

-- Index cho truy van lich hoc theo ClassSection va ngay
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_CSR_ClassSection_Day')
    CREATE NONCLUSTERED INDEX [IX_CSR_ClassSection_Day]
    ON [dbo].[ClassScheduleRules]([ClassSectionId], [DayOfWeek])
    INCLUDE ([StartTime], [EndTime], [Room], [Instructor], [EffectiveStartDate], [EffectiveEndDate], [IsActive]);
GO

-- Index cho truy van lich hoc theo khoang ngay
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_CSR_EffectiveDates')
    CREATE NONCLUSTERED INDEX [IX_CSR_EffectiveDates]
    ON [dbo].[ClassScheduleRules]([EffectiveStartDate], [EffectiveEndDate])
    INCLUDE ([ClassSectionId], [DayOfWeek], [IsActive]);
GO

-- Index cho enrollment theo sinh vien
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SCE_StudentId')
    CREATE NONCLUSTERED INDEX [IX_SCE_StudentId]
    ON [dbo].[StudentCourseEnrollments]([StudentId])
    INCLUDE ([ClassSectionId], [Status], [IsActive]);
GO

-- Index cho lich thi theo ngay
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ES_ExamDate')
    CREATE NONCLUSTERED INDEX [IX_ES_ExamDate]
    ON [dbo].[ExamSchedules]([ExamDate])
    INCLUDE ([ClassSectionId], [StartTime], [EndTime], [Room], [IsActive]);
GO

-- Index cho thong bao theo ngay dang va trang thai
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Ann_PublishedActive')
    CREATE NONCLUSTERED INDEX [IX_Ann_PublishedActive]
    ON [dbo].[Announcements]([PublishedAt] DESC, [IsActive])
    INCLUDE ([Title], [Category], [Priority]);
GO

-- Index cho doi tuong nhan thong bao
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AT_AnnouncementId')
    CREATE NONCLUSTERED INDEX [IX_AT_AnnouncementId]
    ON [dbo].[AnnouncementTargets]([AnnouncementId])
    INCLUDE ([TargetType], [TargetValue]);
GO

-- Index cho su kien theo khoang thoi gian
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Events_DateTime')
    CREATE NONCLUSTERED INDEX [IX_Events_DateTime]
    ON [dbo].[Events]([StartDateTime], [EndDateTime])
    INCLUDE ([Title], [IsActive]);
GO

-- Index cho dang ky su kien theo sinh vien
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ER_StudentId')
    CREATE NONCLUSTERED INDEX [IX_ER_StudentId]
    ON [dbo].[EventRegistrations]([StudentId])
    INCLUDE ([EventId], [Status]);
GO

-- Index cho viec ca nhan theo sinh vien va ngay
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_PT_StudentPlannedDate')
    CREATE NONCLUSTERED INDEX [IX_PT_StudentPlannedDate]
    ON [dbo].[PersonalTasks]([StudentId], [PlannedDate])
    INCLUDE ([Title], [IsCompleted]);
GO

-- Index cho thong bao da luu theo sinh vien va ngay
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SSA_StudentPlanned')
    CREATE NONCLUSTERED INDEX [IX_SSA_StudentPlanned]
    ON [dbo].[StudentSavedAnnouncements]([StudentId], [PlannedAt])
    INCLUDE ([AnnouncementId]);
GO

-- Index cho AppUsers theo StudentId
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AppUsers_StudentId')
    CREATE NONCLUSTERED INDEX [IX_AppUsers_StudentId]
    ON [dbo].[AppUsers]([StudentId])
    WHERE [StudentId] IS NOT NULL;
GO

PRINT N'';
PRINT N'=============================================';
PRINT N'  Schema created successfully!';
PRINT N'  14 tables + indexes + constraints';
PRINT N'=============================================';
GO
