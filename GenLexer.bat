@echo off

set MINT_CUR_DIR=%~dp0

ragel -A -L -G0 "%MINT_CUR_DIR%Compiler\Lexer_exec.csrl" -o "%MINT_CUR_DIR%gen\Lexer_exec.cs"
ragel -A -L -G0 "%MINT_CUR_DIR%Compiler\Lexer_data.csrl" -o "%MINT_CUR_DIR%gen\Lexer_data.cs"

if %errorlevel% neq 0 (pause)

set MINT_CUR_DIR=