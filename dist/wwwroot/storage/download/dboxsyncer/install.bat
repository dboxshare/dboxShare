@echo off

%1 mshta vbscript:CreateObject("Shell.Application").ShellExecute("cmd.exe","/c %~s0 ::","","runas",1)(window.close)&&exit

reg add "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Run" /v dboxSyncer /t REG_SZ /d "%~dp0dboxSyncer.exe" /f

start "" "%~dp0dboxSyncer.exe"

pause
exit