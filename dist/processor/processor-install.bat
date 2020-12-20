@echo off

%1 mshta vbscript:CreateObject("Shell.Application").ShellExecute("cmd.exe","/c %~s0 ::","","runas",1)(window.close)&&exit

reg add "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Run" /v dboxShare.Processor /t REG_SZ /d "%~dp0processor-start.bat" /f

mshta vbscript:CreateObject("WScript.Shell").Run("%~dp0dboxShare.Processer.exe",0)(window.close)

pause
exit