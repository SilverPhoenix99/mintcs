@echo off

setlocal

set SOLUTION_DIR=%~dp0
set OUTPUT_DIR=%SOLUTION_DIR%.vs\mint\OpenCover

OpenCover.Console -hideskipped:All -log:Off -register:user -target:nunit3-console.exe -targetargs:"\"%SOLUTION_DIR%UnitTests\bin\Debug\Mint.UnitTests.dll\" --work=\"%OUTPUT_DIR%\" --noresult --noheader --nocolor" -output:"%OUTPUT_DIR%\CoverResults.xml"
ReportGenerator -reporttypes:Html -verbosity:Error -assemblyfilters:+Mint.VM;+Mint.Compiler -classfilters:-Mint.*Error -reports:"%OUTPUT_DIR%\CoverResults.xml" -targetdir:"%OUTPUT_DIR%\Report"

endlocal