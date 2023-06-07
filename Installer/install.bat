@echo off
:SCRIPT_SELF_WRAP
if "%script_self_wrapped%" equ "true" goto script_setup_env
set "script_self_wrapped=true"
%comspec% /s /c ""%~0" %*"
set "script_self_wrapped=false"
exit /b

:func_script_exit
echo [INFO]
echo [INFO] Press any key to exit...
pause >nul 2>&1
echo [INFO] Exiting...
exit

:func_screen_base
cls
echo [INFO] ---------------------
echo [INFO]   CMDHere Installer
echo [INFO] ---------------------
echo [INFO]
echo [INFO] Made by vlOd
echo [INFO]
exit /b

:script_setup_env
pushd %~dp0
setlocal ENABLEDELAYEDEXPANSION
set "script_install_path=%systemdrive%\CMDHere"
goto screen_main

:screen_main
call :func_screen_base
echo [INFO] This script will install the shell extension CMDHere
echo [INFO] This shellex allows you to open a CMD at the current location
echo [INFO]
echo [INFO] Installation details:
echo [INFO] - Install DIR: "%script_install_path%"
echo [INFO] - Register: true
echo [INFO] - Register ContextMenuHandler: true

:prompt_continue_after_main
echo [INFO]
choice.exe /N "[INFO] Are you sure you want to continue? [Y,N] "
if "%errorlevel%" equ "1" goto :screen_installing
call :func_script_exit

:screen_installing
call :func_screen_base

echo [INFO] Creating the installation directory...
MKDIR "%script_install_path%" > nul 2>&1

echo [INFO] Copying files...
copy /b /y "CMDHere.dll" "%script_install_path%\CMDHere.dll" > nul 2>&1
copy /b /y "unregister.reg" "%script_install_path%\unregister.reg" > nul 2>&1
copy /b /y "unregister_as_context_menu_handler.reg" "%script_install_path%\unregister_as_context_menu_handler.reg" > nul 2>&1
copy /b /y "uninstall.bat" "%script_install_path%\uninstall.bat" > nul 2>&1
copy /b /y "choice.exe" "%script_install_path%\choice.exe" > nul 2>&1

echo [INFO] Registering...
reg import "register.reg" > nul 2>&1

echo [INFO] Registering ContextMenuHandler...
reg import "register_as_context_menu_handler.reg" > nul 2>&1

echo [INFO]
echo [INFO] Press any key to continue...
pause >nul 2>&1

goto screen_installation_finished

:screen_installation_finished
call :func_screen_base
echo [INFO] The installation was completed successfully.
call :func_script_exit