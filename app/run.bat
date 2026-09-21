@echo off
setlocal
set "PROJECT=%~dp0VoyageGame"

REM Change this path if your Godot executable is elsewhere.
set "GODOT=D:\LOCAL-WORK-STATION\Godot_v4.7.2-stable_win64\Godot_v4.7.2-stable_mono_win64.exe"

cd /d "%PROJECT%"

echo [1/2] Building...
dotnet build VoyageGame.csproj
if errorlevel 1 (
    echo.
    echo [ERROR] Build failed.
    pause
    exit /b 1
)

echo.
echo [2/2] Starting Godot...
if not exist "%GODOT%" (
    echo [ERROR] Godot executable not found:
    echo %GODOT%
    echo Edit GODOT in run.bat to match your installation.
    pause
    exit /b 1
)

"%GODOT%" --path "%PROJECT%"
endlocal
