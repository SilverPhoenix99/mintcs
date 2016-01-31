@echo off

setlocal

set MINT_DIR=%~dp0
set MINT_PARSER_DIR=%MINT_DIR%Parser\
set MINT_GEN_DIR=%MINT_DIR%gen\

if not exist "%MINT_GEN_DIR%" mkdir "%MINT_GEN_DIR%"

ragel -A -L -F1 --error-format=msvc "%MINT_PARSER_DIR%Lexer_exec.csrl" -o "%MINT_GEN_DIR%Lexer_exec.cs"
ragel -A -L -F1 --error-format=msvc "%MINT_PARSER_DIR%Lexer_data.csrl" -o "%MINT_GEN_DIR%Lexer_data.cs"

endlocal