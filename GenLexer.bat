@echo off

set MINT_GEN_DIR=%~dp0

if not exist "%MINT_GEN_DIR%gen" mkdir "%MINT_GEN_DIR%gen"

ragel -A -L -G0 "%MINT_GEN_DIR%Compiler\Lexer_exec.csrl" -o "%MINT_GEN_DIR%gen\Lexer_exec.cs"
ragel -A -L -G0 "%MINT_GEN_DIR%Compiler\Lexer_data.csrl" -o "%MINT_GEN_DIR%gen\Lexer_data.cs"

if %errorlevel% neq 0 (pause)

set MINT_GEN_DIR=