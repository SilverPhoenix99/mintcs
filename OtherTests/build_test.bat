
@set OLD_PATH=%PATH%
@set PATH=%MINGW_HOME%;%MSYS_HOME%;%PATH%

gcc -c -Wall -fpic -o obj\Debug\test.o test.c
gcc -shared -o bin\Debug\test.so obj\Debug\test.o

@set PATH=%OLD_PATH%
@set OLD_PATH=

pause