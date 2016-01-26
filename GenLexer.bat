@echo off

setlocal

set MINT_DIR=%~dp0
set MINT_CMP_DIR=%MINT_DIR%Compiler\
set MINT_GEN_DIR=%MINT_DIR%gen\

if not exist "%MINT_GEN_DIR%" mkdir "%MINT_GEN_DIR%"

ragel -A -L -F0 --error-format=msvc "%MINT_CMP_DIR%Lexer_exec.csrl" -o "%MINT_GEN_DIR%Lexer_exec.cs"
ragel -A -L -F0 --error-format=msvc "%MINT_CMP_DIR%Lexer_data.csrl" -o "%MINT_GEN_DIR%Lexer_data.cs"

endlocal