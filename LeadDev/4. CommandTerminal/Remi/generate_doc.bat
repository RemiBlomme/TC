@echo off

for %%I in (.) do set DirName=%%~nxI

if not exist %cd%\%DirName%.sln (goto nosln)
if not exist %cd%\docfx.json (goto init) else (goto generate)

:nosln
color 0C
echo ERROR: There is no .sln file in this folder
pause
exit

:init
docfx init -y

:replace source file
setlocal enabledelayedexpansion
set "docfxFile=%cd%\docfx.json"
powershell -Command "(Get-Content '%docfxFile%' -Raw) -replace '../src', '' | Set-Content '%docfxFile%'"

:generate
docfx %cd%\docfx.json --serve