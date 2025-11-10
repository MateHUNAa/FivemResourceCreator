@echo off
REM ==============================
REM Build Fivem Resource Creator
REM ==============================

REM Set variables
SET "PROJECT_DIR=%~dp0app"
SET "PROJECT_FILE=%PROJECT_DIR%\fvm.csproj"
SET "PUBLISH_DIR=%PROJECT_DIR%\bin\Release\net8.0\win-x64\publish"
SET "TARGET_DIR=C:\Tools\fvm"

REM Clean previous publish
IF EXIST "%PUBLISH_DIR%" (
    rmdir /s /q "%PUBLISH_DIR%"
)

REM Build and publish self-contained single file
dotnet publish "%PROJECT_FILE%" -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeAllContentForSelfExtract=true
IF ERRORLEVEL 1 (
    echo [ERROR] Build failed!
    pause
    exit /b 1
)

REM Create target folder if it doesn't exist
IF NOT EXIST "%TARGET_DIR%" mkdir "%TARGET_DIR%"

REM Copy publish output to target
xcopy "%PUBLISH_DIR%\*" "%TARGET_DIR%\" /E /I /Y

REM ==============================
REM Add TARGET_DIR to user PATH safely
REM ==============================

REM Read current user PATH from registry
FOR /F "tokens=2*" %%A IN ('reg query "HKCU\Environment" /v Path 2^>nul ^| find "Path"') DO SET "CURPATH=%%B"

REM Check if TARGET_DIR is already in PATH
echo %CURPATH% | find /I "%TARGET_DIR%" >nul
IF ERRORLEVEL 1 (
    REM Not found, add it
    setx PATH "%CURPATH%;%TARGET_DIR%"
    echo Added %TARGET_DIR% to user PATH
) ELSE (
    echo %TARGET_DIR% is already in PATH
)

echo.
echo ==============================
echo Build completed successfully!
echo Files copied to %TARGET_DIR%
echo ==============================
pause
