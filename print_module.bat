@echo off

setlocal

ruby -r./module_printer -e "ModulePrinter.print %1"

endlocal