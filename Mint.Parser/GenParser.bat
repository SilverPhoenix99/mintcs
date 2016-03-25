@echo off

setlocal

set MINT_DIR=%~dp0
set MINT_GEN_DIR=%MINT_DIR%gen\
set MINT_PARSER_DIR=%MINT_DIR%Parse\

if not exist "%MINT_GEN_DIR%" mkdir "%MINT_GEN_DIR%"

gppg /no-info /no-lines "/out:%MINT_GEN_DIR%Parser.cs" "%MINT_PARSER_DIR%Parser.y"

if %errorlevel% neq 0 (pause)