@echo off
setlocal
set "PROJECT=%~dp0VoyageGame"

echo ========================================
echo RTS Voyage Game - Build
echo ========================================
echo.

cd /d "%PROJECT%"
dotnet build VoyageGame.csproj

if errorlevel 1 (
    echo.
    echo [ERROR] Build failed.
    pause
    exit /b 1
)

echo.
echo Build succeeded.
pause
endlocal
