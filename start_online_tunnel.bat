echo off
chcp 65001 >nul
title KHOI DONG HE THONG VA PHAT LINK ONLINE (CLOUDFLARE)
echo ========================================================
echo   KHOI DONG HE THONG VA PHAT LINK ONLINE
echo ========================================================
echo.
set "DOTNET_ROOT=%USERPROFILE%\.dotnet"
set "PATH=%USERPROFILE%\.dotnet;%USERPROFILE%\.dotnet\tools;%PATH%"
echo [1/2] Dang khoi dong Backend Web API (port 5000)...
start "UEH Backend API" /min cmd /k "dotnet run --project src\UEHStudent.Api\UEHStudent.Api.csproj --launch-profile http"
echo Cho 3 giay de server khoi dong hoan tat...
timeout /t 3 /nobreak >nul
echo [2/2] Dang mo duong ong Cloudflare Tunnel...
echo.
echo ========================================================
echo   CHU Y: Hay tim dong link co dang:
echo   https://xxxx.trycloudflare.com
echo   do chinh la duong link de gui cho nguoi khac truy cap!
echo ========================================================
echo.
"C:\Program Files (x86)\cloudflared\cloudflared.exe" tunnel --url http://localhost:5000