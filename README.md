
# Instructions (Windows)

## Prerequisites:

- [ragel](https://github.com/eloraiby/ragel-windows/blob/master/ragel.exe?raw=true)
- [gppg](https://gppg.codeplex.com/downloads/get/899043)

copy the binaries (ragel.exe and gppg.exe) and add them to the path
## Instructions with Visual Studio Community 2015
Open the Solution file and compile.
## Instructions without Visual Studio
### Requisites:

### [nuget](https://dist.nuget.org/index.html)

```
nuget install Microsoft.Net.Compilers
```

add msbuild folder to path:
```
set path=%path%;%windir%/Microsoft.NET/Framework64/<version>
```
to compile use the path to csc.exe that is inside the Microsoft.Net.Compilers folder:
```
msbuild /property:CscToolPath=<path_to_csc>
```
# Instructions (Linux)
TODO
