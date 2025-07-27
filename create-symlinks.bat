@echo off
setlocal enabledelayedexpansion

:: Check for administrator privileges
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo This script requires administrator privileges to create symlinks.
    echo Please run as administrator.
    pause
    exit /b 1
)

:: Set config file path (can be passed as parameter or use default)
set "CONFIG_FILE=%~1"
if "%CONFIG_FILE%"=="" set "CONFIG_FILE=symlink_config.txt"

:: Check if config file exists
if not exist "%CONFIG_FILE%" (
    echo Configuration file not found: %CONFIG_FILE%
    echo Please create a config file with the following format:
    echo SOURCE_PATH^|DESTINATION_PATH^|TYPE
    echo Where TYPE is either FILE or DIR
    pause
    exit /b 1
)

echo ========================================
echo Starting Symlink Creation
echo Config File: %CONFIG_FILE%
echo ========================================
echo.

:: Initialize counters
set /a SUCCESS_COUNT=0
set /a ERROR_COUNT=0

:: Read config file line by line
for /f "usebackq tokens=1-3 delims=|" %%a in ("%CONFIG_FILE%") do (
    :: Skip empty lines and comments
    set "LINE=%%a"
    if not "!LINE!"=="" if not "!LINE:~0,1!"=="#" (
        set "SOURCE=%%a"
        set "DEST=%%b"
        set "TYPE=%%c"
        
        :: Trim spaces
        for /f "tokens=* delims= " %%x in ("!SOURCE!") do set "SOURCE=%%x"
        for /f "tokens=* delims= " %%x in ("!DEST!") do set "DEST=%%x"
        for /f "tokens=* delims= " %%x in ("!TYPE!") do set "TYPE=%%x"
        
        :: Convert TYPE to uppercase
        for %%i in (A B C D E F G H I J K L M N O P Q R S T U V W X Y Z) do (
            set "TYPE=!TYPE:%%i=%%i!"
        )
        
        :: Process based on type
        if "!TYPE!"=="DIR" (
            echo Creating directory symlink:
            echo   Source: !SOURCE!
            echo   Dest:   !DEST!
            
            :: Check if source exists
            if exist "!SOURCE!\" (
                :: Check if destination already exists
                if exist "!DEST!" (
                    echo   [WARNING] Destination already exists, skipping...
                    set /a ERROR_COUNT+=1
                ) else (
                    :: Create parent directory if it doesn't exist
                    for %%i in ("!DEST!") do set "PARENT_DIR=%%~dpi"
                    if not exist "!PARENT_DIR!" mkdir "!PARENT_DIR!"
                    
                    :: Create directory symlink
                    mklink /D "!DEST!" "!SOURCE!" >nul 2>&1
                    if !errorlevel! equ 0 (
                        echo   [SUCCESS] Directory symlink created
                        set /a SUCCESS_COUNT+=1
                    ) else (
                        echo   [ERROR] Failed to create directory symlink
                        set /a ERROR_COUNT+=1
                    )
                )
            ) else (
                echo   [ERROR] Source directory does not exist
                set /a ERROR_COUNT+=1
            )
            echo.
            
        ) else if "!TYPE!"=="FILE" (
            echo Creating file symlink:
            echo   Source: !SOURCE!
            echo   Dest:   !DEST!
            
            :: Check if source exists
            if exist "!SOURCE!" (
                :: Check if destination already exists
                if exist "!DEST!" (
                    echo   [WARNING] Destination already exists, skipping...
                    set /a ERROR_COUNT+=1
                ) else (
                    :: Create parent directory if it doesn't exist
                    for %%i in ("!DEST!") do set "PARENT_DIR=%%~dpi"
                    if not exist "!PARENT_DIR!" mkdir "!PARENT_DIR!"
                    
                    :: Create file symlink
                    mklink "!DEST!" "!SOURCE!" >nul 2>&1
                    if !errorlevel! equ 0 (
                        echo   [SUCCESS] File symlink created
                        set /a SUCCESS_COUNT+=1
                    ) else (
                        echo   [ERROR] Failed to create file symlink
                        set /a ERROR_COUNT+=1
                    )
                )
            ) else (
                echo   [ERROR] Source file does not exist
                set /a ERROR_COUNT+=1
            )
            echo.
            
        ) else (
            echo [ERROR] Invalid type: !TYPE! (must be FILE or DIR)
            echo   Source: !SOURCE!
            echo   Dest:   !DEST!
            set /a ERROR_COUNT+=1
            echo.
        )
    )
)

echo ========================================
echo Symlink Creation Complete
echo Success: %SUCCESS_COUNT%
echo Errors:  %ERROR_COUNT%
echo ========================================

pause