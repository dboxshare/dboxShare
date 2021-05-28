@echo off

%1 mshta vbscript:CreateObject("Shell.Application").ShellExecute("cmd.exe","/c %~s0 ::","","runas",1)(window.close)&&exit

set jdk_path=%~dp0

if not exist "%jdk_path%bin" (
echo jdk directory does not exist
) else if "%JAVA_HOME%" neq "" (
echo configured
) else (
setx /M JAVA_HOME "%jdk_path:~0,-1%"
setx /M CLASSPATH ".;%%JAVA_HOME%%\lib;"
setx /M PATH "%PATH%;%%JAVA_HOME%%\bin;%%JAVA_HOME%%\jre\bin"

java -version
java
javac
)

pause
exit