@echo off

mshta vbscript:CreateObject("WScript.Shell").Run("%~dp0dboxShare.Processer.exe",0)(window.close)

exit