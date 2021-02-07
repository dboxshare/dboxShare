@echo off

set path=%~dp1

start /wait "" "C:\Program Files\LibreOffice\program\soffice.exe" --headless --invisible --convert-to pdf "%1" --outdir "%path:~0,-1%"

taskkill /f /t /im soffice.exe

ren "%~dp1%~n1.pdf" "%~nx1.pdf"

exit