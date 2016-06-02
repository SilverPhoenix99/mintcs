@echo off

setlocal

set MINT_DIR=%~dp0
set MINT_LEXER_DIR=%MINT_DIR%Lexing\
set MINT_GEN_DIR=%MINT_DIR%gen\

if not exist "%MINT_GEN_DIR%" mkdir "%MINT_GEN_DIR%"

::set RAGEL_OPTIONS=-F1
set RAGEL_OPTIONS=-T0

ragel -L -A %RAGEL_OPTIONS% --error-format=msvc "%MINT_LEXER_DIR%Lexer_exec.csrl" -o "%MINT_GEN_DIR%Lexer_exec.cs"
ragel -L -A %RAGEL_OPTIONS% --error-format=msvc "%MINT_LEXER_DIR%Lexer_data.csrl" -o "%MINT_GEN_DIR%Lexer_data.cs"

endlocal
