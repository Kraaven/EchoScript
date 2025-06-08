@echo off
REM =============================================
REM Build a single-file executable for Windows and Linux with custom names
REM =============================================

REM ---- CONFIGURATION ----
set PROJECT=Birch\Birch.csproj
set CONFIG=Release
set OUTPUT=publish
set BUILD_NAME=BirchInterpreter

set WIN_RUNTIME=win-x64
set LINUX_RUNTIME=linux-x64

REM ---- CLEAN ----
echo Cleaning project...
dotnet clean %PROJECT%

REM ---- CREATE OUTPUT FOLDER ----

if exist %OUTPUT% (
    rmdir /S /Q %OUTPUT%
)
mkdir %OUTPUT%


REM ===============================
REM  Build Windows x64
REM ===============================
echo.
echo Building Windows...
dotnet publish %PROJECT% ^
    -c %CONFIG% ^
    -r %WIN_RUNTIME% ^
    --self-contained true ^
    /p:PublishSingleFile=true ^
    /p:IncludeNativeLibrariesForSelfExtract=true ^
    /p:PublishTrimmed=false ^
    /p:DebugType=None ^
    -o %OUTPUT%\temp_win

REM Rename the Windows output
for %%f in (%OUTPUT%\temp_win\*.exe) do (
    move /Y "%%f" "%OUTPUT%\%BUILD_NAME%_windows.exe"
)

REM Cleanup temp folder
rmdir /S /Q %OUTPUT%\temp_win

REM ===============================
REM  Build Linux x64
REM ===============================
echo.
echo Building Linux...
dotnet publish %PROJECT% ^
    -c %CONFIG% ^
    -r %LINUX_RUNTIME% ^
    --self-contained true ^
    /p:PublishSingleFile=true ^
    /p:IncludeNativeLibrariesForSelfExtract=true ^
    /p:PublishTrimmed=false ^
    /p:DebugType=None ^
    -o %OUTPUT%\temp_linux

REM Find the output file (it will match Birch because it uses the project name)
for %%f in (%OUTPUT%\temp_linux\Birch) do (
    move /Y "%%f" "%OUTPUT%\%BUILD_NAME%_linux"
)

REM Cleanup temp folder
rmdir /S /Q %OUTPUT%\temp_linux

REM ===============================
REM  DONE
REM ===============================
echo.
echo ===============================
echo  All builds complete!
echo 🔹 Windows EXE: %OUTPUT%\%BUILD_NAME%_windows.exe
echo 🔹 Linux EXE:   %OUTPUT%\%BUILD_NAME%_linux
echo ===============================
pause
