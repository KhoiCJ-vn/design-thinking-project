/*
=============================================
  UEH Student My Week — Create Database
  File: 01_CreateDatabase.sql
  Purpose: Tao database UEHStudentDB neu chua ton tai.
  
  HUONG DAN:
  - Mo SQL Server Management Studio hoac Azure Data Studio.
  - Ket noi den SQL Server (localhost).
  - Chay script nay truoc tien.
  - Sau do chay 02_CreateSchema.sql va 03_SeedDemoData.sql.
  
  LUU Y: Khong chua lenh DROP DATABASE.
=============================================
*/

USE [master];
GO

-- Kiem tra database da ton tai chua
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'UEHStudentDB')
BEGIN
    CREATE DATABASE [UEHStudentDB];
    PRINT N'Database [UEHStudentDB] da duoc tao thanh cong.';
END
ELSE
BEGIN
    PRINT N'Database [UEHStudentDB] da ton tai. Khong tao lai.';
END
GO
