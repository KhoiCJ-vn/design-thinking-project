@echo off
chcp 65001 >nul
title KHOI DONG HE THONG UEH STUDENT & MY WEEK

echo ========================================================
echo   HE THONG CUA SO THOI KHOA BIEU & UEH MY WEEK
echo ========================================================
echo.

set "DOTNET_ROOT=%USERPROFILE%\.dotnet"
set "PATH=%USERPROFILE%\.dotnet;%USERPROFILE%\.dotnet\tools;%PATH%"

echo [1/2] Dang khoi dong Backend Web API (port 5000)...
start "UEH Backend API" cmd /k "dotnet run --project src\UEHStudent.Api\UEHStudent.Api.csproj --launch-profile http"

echo Cho 3 giay de server khoi dong hoan tat...
timeout /t 3 /nobreak >nul

echo [2/2] Dang mo trinh duyet...
start http://localhost:5000/login.html

echo.
echo ========================================================
echo   DA KHOI DONG THANH CONG!
echo   Dia chi trang web: http://localhost:5000/login.html
echo ========================================================
echo.
pause
