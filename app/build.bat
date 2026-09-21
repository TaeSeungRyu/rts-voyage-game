@echo off
setlocal

set "GODOT=D:\LOCAL-WORK-STATION\Godot_v4.7.2-stable_win64\Godot_v4.7.2-stable_mono_win64.exe"
set "PROJECT=%~dp0VoyageGame"

echo ========================================
echo RTS Voyage Game - Build
echo ========================================
echo.

echo [1/2] Importing Godot resources...

"%GODOT%" --headless --editor --path "%PROJECT%" --quit

if errorlevel 1 (
    echo.
    echo [ERROR] Godot resource import failed.
    pause
    exit /b %errorlevel%
)

echo.
echo [2/2] Building C# project...

cd /d "%PROJECT%"

dotnet build VoyageGame.csproj

if errorlevel 1 (
    echo.
    echo [ERROR] C# build failed.
    pause
    exit /b %errorlevel%
)

echo.
echo ========================================
echo Build succeeded.
echo ========================================
echo.

pause
endlocal