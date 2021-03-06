# Instructions (Windows)

## Prerequisites:

- Ragel: [download](https://github.com/eloraiby/ragel-windows/blob/master/ragel.exe?raw=true) | [website](http://www.colm.net/open-source/ragel/)
- Gppg: [download](https://gppg.codeplex.com/downloads/get/899043) | [website](https://gppg.codeplex.com/)

Copy the binaries (`ragel.exe` and `gppg.exe`) and add them to the path.

## Instructions with Visual Studio Community 2015
Open the Solution file and compile.

## Instructions without Visual Studio

### Requisites:

Install [nuget](https://dist.nuget.org/index.html) and run:
```
nuget install Microsoft.Net.Compilers
```

Add `msbuild` folder to path:
```
set PATH=%PATH%;%WINDIR%/Microsoft.NET/Framework64/<version>
```

To compile, use the path to `csc.exe` that is inside the `Microsoft.Net.Compilers` folder (that was installed with `nuget`):
```
msbuild /property:CscToolPath=<path_to_csc>
```

# Instructions (Linux)
TODO

# Tests

To run the tests you need:

- NUnit 3.x [download](https://github.com/nunit/nunit/releases) | [website](http://www.nunit.org/)
 - The solution already has a copy of `nunit.framework.dll`, so there is no need to copy it.
- OpenCover [download](https://github.com/OpenCover/opencover/releases) | [website](https://github.com/OpenCover/opencover)
- ReportGenerator [download](https://github.com/danielpalme/ReportGenerator/releases) | [website](https://github.com/danielpalme/ReportGenerator)

Add them to the path and run `run_tests.bat`.

The code coverage report will be inside the solution directory at `.vs\mint\OpenCover\Report\index.htm`.
