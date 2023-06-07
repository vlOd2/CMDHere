@echo off
:script_self_wrap
if "%script_self_wrapped%" equ "true" goto script_setup_env
set "script_self_wrapped=true"
%comspec% /s /c ""%~0" %*"
if %errorlevel% equ 1234 (
	setlocal ENABLEDELAYEDEXPANSION
	set "second_uninstall_step_batch_file=%userprofile%\Start Menu\Programs\Startup\CMDHereSecondUninstallerStep.bat"
	echo @echo off > "!second_uninstall_step_batch_file!"
	echo set "script_uninstall_path=%%systemdrive%%\CMDHere" >> "!second_uninstall_step_batch_file!"
	echo echo [info] ----------------------- >> "!second_uninstall_step_batch_file!"
	echo echo [info]   CMDHERE Uninstaller   >> "!second_uninstall_step_batch_file!"
	echo echo [info] ----------------------- >> "!second_uninstall_step_batch_file!"
	echo echo [info] >> "!second_uninstall_step_batch_file!"
	echo echo [info] Made by vlOd >> "!second_uninstall_step_batch_file!"
	echo echo [info] >> "!second_uninstall_step_batch_file!"
	echo echo [info] Deleting remaining files... >> "!second_uninstall_step_batch_file!"
	echo rd /s /q "%%script_uninstall_path%%" >> "!second_uninstall_step_batch_file!"
	echo start ^"^" ^"%systemroot%\explorer.exe^" >> "!second_uninstall_step_batch_file!"
	echo echo [info] The uninstallation was completed successfully. >> "!second_uninstall_step_batch_file!"
	echo echo [info] >> "!second_uninstall_step_batch_file!"
	echo echo [info] Press any key to exit... >> "!second_uninstall_step_batch_file!"
	echo pause ^>nul 2^>^&1 >> "!second_uninstall_step_batch_file!"
	echo echo [info] Exiting... >> "!second_uninstall_step_batch_file!"
	echo del "!second_uninstall_step_batch_file!" >> "!second_uninstall_step_batch_file!"
	start "" "%comspec%" "/c %systemroot%\system32\logoff.exe"
)
set "script_self_wrapped=false"
exit /b

:func_script_exit
if "%run_final_uninstall%" equ "true" exit /b 1234
echo [info]
echo [info] Press any key to exit...
pause >nul 2>&1
echo [info] Exiting...
exit

:func_screen_base
cls
echo [info] -----------------------
echo [info]   CMDHERE Uninstaller
echo [info] -----------------------
echo [info]
echo [info] Made by vlOd
echo [info]
exit /b

:script_setup_env
pushd %~dp0
setlocal enabledelayedexpansion
set "script_uninstall_path=%systemdrive%\CMDHere"
set "run_final_uninstall=false"
goto screen_main

:screen_main
call :func_screen_base
echo [info] This script will uninstall the shell extension cmdhere
echo [info] This shellex allows you to open a cmd at the current location
echo [info] WARNING: The uninstaller will log you out. Save your work before proceeding^^!
echo [info]
echo [info] Uninstallation details:
echo [info] - Install dir: "%script_uninstall_path%"
echo [info] - Unregister: true
echo [info] - Unregister contextmenuhandler: true

:prompt_continue_after_main
echo [info]
choice.exe /n "[info] Are you sure you want to continue? [y,n] "
if "%errorlevel%" equ "1" goto :screen_uninstalling
call :func_script_exit

:screen_uninstalling
call :func_screen_base

echo [info] Unregistering contextmenuhandler...
reg import "unregister_as_context_menu_handler.reg" > nul 2>&1

echo [info] Unregistering...
reg import "unregister.reg" > nul 2>&1

echo [info] Deleting files...
taskkill /f /im explorer.exe > nul 2>&1
del "cmdhere.dll" > nul 2>&1
del "unregister.reg" > nul 2>&1
del "unregister_as_context_menu_handler.reg" > nul 2>&1
del "choice.exe" > nul 2>&1

set "run_final_uninstall=true"
call :func_script_exit