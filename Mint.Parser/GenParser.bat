@echo off

setlocal

set MINT_DIR=%~dp0
set MINT_GEN_DIR=%MINT_DIR%gen\Parse\
set MINT_PARSER_DIR=%MINT_DIR%Parse\

gppg /no-info /no-lines "/out:%MINT_GEN_DIR%Parser.cs" "%MINT_PARSER_DIR%Parser.y"
