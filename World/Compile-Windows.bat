@echo off
REM
REM Compile-Windows.bat -- Windows "shortcut" for the Memento shard.
REM
REM Running this file (double-click) will:
REM   1. Run `git pull` from the repository base folder (the parent of this World folder).
REM   2. Run the Windows compiler in Source\Tools (compile-world-win.bat, which uses .NET csc).
REM
REM Both compilers use paths relative to World\Source\Tools, so we pushd there before
REM invoking them. If `git pull` fails we stop before compiling so we never build
REM stale/conflicting code.
setlocal

REM %~dp0 is the folder that holds this .bat (the World folder), with a trailing backslash.
set "WORLD_DIR=%~dp0"
REM Repository base folder == parent of the World folder.
for %%I in ("%WORLD_DIR%..") do set "BASE_DIR=%%~fI"
set "TOOLS_DIR=%WORLD_DIR%Source\Tools"

echo ==^> git pull  (base folder: "%BASE_DIR%")
pushd "%BASE_DIR%"
git pull
if errorlevel 1 (
    echo.
    echo !! git pull failed - aborting so we don't compile stale code.
    popd
    pause
    exit /b 1
)
popd

echo ==^> Compiling with the Windows compiler  ("%TOOLS_DIR%\compile-world-win.bat")
pushd "%TOOLS_DIR%"
call "compile-world-win.bat"
popd

endlocal
