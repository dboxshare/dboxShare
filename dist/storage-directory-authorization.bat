@echo off

%1 mshta vbscript:CreateObject("Shell.Application").ShellExecute("cmd.exe","/c %~s0 ::","","runas",1)(window.close)&&exit

set www_path=%~dp0

if not exist "%www_path%storage" (
echo storage directory does not exist
) else (
cmd /c "icacls "%www_path%storage" /grant Administrator:(OI)(CI)(M,R,W,D) /T"
cmd /c "icacls "%www_path%storage" /grant IIS_IUSRS:(OI)(CI)(M,R,W,D) /T"
)

pause
exit