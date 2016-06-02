@echo off

setlocal

set FILENAME=%1

if "%FILENAME%" == "" goto:eof

set MINT_DIR=%~dp0
set MINT_LEXER_DIR=Lex\States
set MINT_GEN_DIR=gen\%MINT_LEXER_DIR%
set INPUT_FILENAME=%FILENAME%.csrl
set OUTPUT_FILENAME=%FILENAME%.cs

cd "%MINT_DIR%"

if not exist "%MINT_GEN_DIR%" mkdir "%MINT_GEN_DIR%"

::set RAGEL_OPTIONS=-F1
set RAGEL_OPTIONS=-T0

ragel -A -L --error-format=msvc %RAGEL_OPTIONS% "%MINT_LEXER_DIR%\%INPUT_FILENAME%" -o "%MINT_GEN_DIR%\%OUTPUT_FILENAME%"

endlocal
