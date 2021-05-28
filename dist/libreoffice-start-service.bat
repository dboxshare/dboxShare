@echo off

%1 mshta vbscript:CreateObject("Shell.Application").ShellExecute("cmd.exe","/c %~s0 ::","","runas",1)(window.close)&&exit

set libreoffice_path=%~dp0

if not exist "%libreoffice_path%program" (
echo libreoffice directory does not exist
) else (
cd /d "%libreoffice_path%program"

start soffice.exe --headless --accept="socket,host=127.0.0.1,port=8100;urp;" --nofirststartwizard
)

pause
exit