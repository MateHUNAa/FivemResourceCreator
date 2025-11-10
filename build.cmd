@echo off
REM ==============================
REM Build Fivem Resource Creator
REM ==============================

REM Set variables
SET PROJECT_DIR=%~dp0
SET PUBLISH_DIR=%PROJECT_DIR%bin\Release\net8.0\win-x64\publish
SET TARGET_DIR=C:\Tools\fvm

REM Clean previous publish
IF EXIST "%PUBLISH_DIR%" (
    rmdir /s /q "%PUBLISH_DIR%"
)

REM Build and publish self-contained single file
dotnet publish "%PROJECT_DIR%" -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeAllContentForSelfExtract=true

IF ERRORLEVEL 1 (
    echo Build failed!
    pause
    exit /b 1
)

REM Create target folder
IF NOT EXIST "%TARGET_DIR%" mkdir "%TARGET_DIR%"

REM Copy everything from publish folder to target
xcopy "%PUBLISH_DIR%\*" "%TARGET_DIR%\" /E /I /Y

echo.
echo ==============================
echo Build completed successfully!
echo Files copied to %TARGET_DIR%
echo ==============================
pause
