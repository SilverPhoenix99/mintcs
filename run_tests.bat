@echo off

setlocal

set SOLUTION_DIR=%~dp0
set OUTPUT_DIR=%SOLUTION_DIR%.vs\mint\OpenCover

nunit3-console.exe --noresult --noheader --nocolor "--work=%OUTPUT_DIR%" "%SOLUTION_DIR%UnitTests\bin\Debug\Mint.UnitTests.dll"

endlocal
