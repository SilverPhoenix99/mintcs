@echo off

ruby --disable=gems,did_you_mean "%~dp0\module_printer.rb" %*
