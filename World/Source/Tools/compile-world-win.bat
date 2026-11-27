@echo off
REM
REM -- RunUO compile script for .NET 4.0 --
REM
REM The full .NET framework needs to be installed for this script.
REM i.e. not the "Client Profile", as it is missing several required DLLs.
REM
REM Optional arguments (used by the in-game [UpdateServer command):
REM   %1  output path for World.exe (default: ..\..\World.exe)
REM   %2  --nopause to skip the "Press any key" prompt (any %1 implies it)
REM
set "targetfile=World.exe"
set "OUTFILE=..\..\World.exe"
set "NOPAUSE="

if not "%~1"=="" (
	set "OUTFILE=%~1"
	set "NOPAUSE=1"
)
if /I "%~2"=="--nopause" set "NOPAUSE=1"

if exist "%targetfile%" (
	echo Deleting binary...
	del "%targetfile%" 1>NUL 2>NUL
	
	if exist "%targetfile%" (
		echo Failed!
		echo.
		echo Is "%targetfile%" in use?
		echo.
		if not defined NOPAUSE pause
		exit /b 1
	) else (
		echo Success.
		echo.
	)
)

echo Recompiling...
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc /optimize /unsafe /t:exe /out:"%OUTFILE%" /win32icon:..\System\icon.ico /d:NEWTIMERS /d:NEWPARENT /recurse:..\System\*.cs /main:Server.Core
if errorlevel 1 (
	echo.
	echo Build failed.
	if not defined NOPAUSE pause
	exit /b 1
)

if not defined NOPAUSE pause
exit /b 0
