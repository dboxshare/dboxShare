@echo off

%1 mshta vbscript:CreateObject("Shell.Application").ShellExecute("cmd.exe","/c %~s0 ::","","runas",1)(window.close)&&exit

reg delete "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Run" /v dboxSyncer /f

taskkill /f /t /im dboxSyncer.exe
taskkill /f /t /im dboxSyncer.Process.exe

pause
exit