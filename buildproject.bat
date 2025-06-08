@echo off
REM =============================================
REM Build a single-file executable for Windows and Linux
REM =============================================

REM ---- CONFIGURATION ----
set PROJECT=Birch\Birch.csproj
set CONFIG=Release
set WIN_RUNTIME=win-x64
set LINUX_RUNTIME=linux-x64
set OUTPUT=publish

REM ---- CLEAN ----
echo Cleaning project...
dotnet clean %PROJECT%

REM ---- BUILD FOR WINDOWS ----
echo.
echo ===============================
echo 🔨 Building Windows x64 executable...
echo ===============================
dotnet publish %PROJECT% ^
    -c %CONFIG% ^
    -r %WIN_RUNTIME% ^
    --self-contained true ^
    /p:PublishSingleFile=true ^
    /p:IncludeNativeLibrariesForSelfExtract=true ^
    /p:PublishTrimmed=false ^
    -o %OUTPUT%\%WIN_RUNTIME%

REM ---- BUILD FOR LINUX ----
echo.
echo ===============================
echo 🐧 Building Linux x64 executable...
echo ===============================
dotnet publish %PROJECT% ^
    -c %CONFIG% ^
    -r %LINUX_RUNTIME% ^
    --self-contained true ^
    /p:PublishSingleFile=true ^
    /p:IncludeNativeLibrariesForSelfExtract=true ^
    /p:PublishTrimmed=false ^
    -o %OUTPUT%\%LINUX_RUNTIME%

REM ---- DONE ----
echo.
echo ===============================
echo ✅ All builds complete!
echo 🔹 Windows EXE: %OUTPUT%\%WIN_RUNTIME%\Birch.exe
echo 🔹 Linux EXE:   %OUTPUT%\%LINUX_RUNTIME%\Birch
echo ===============================
pause
