@echo off
setlocal

set "GODOT=D:\LOCAL-WORK-STATION\Godot_v4.7.2-stable_win64\Godot_v4.7.2-stable_mono_win64.exe"
set "PROJECT=%~dp0VoyageGame"

echo ========================================
echo RTS Voyage Game - Run
echo ========================================
echo.

echo [1/2] Building project...

call "%~dp0build.bat"

if errorlevel 1 (
    echo.
    echo [ERROR] Build failed. Game will not start.
    pause
    exit /b %errorlevel%
)

echo.
echo [2/2] Starting game...
echo.

"%GODOT%" --path "%PROJECT%"

endlocal