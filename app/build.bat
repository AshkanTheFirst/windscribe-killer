@echo off
rem Rebuilds ObligationKiller.exe using the C# compiler built into Windows (.NET Framework).
setlocal
set "CSC=%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
if not exist "%CSC%" set "CSC=%WINDIR%\Microsoft.NET\Framework\v4.0.30319\csc.exe"
if not exist "%CSC%" (
    echo .NET Framework C# compiler not found.
    pause
    exit /b 1
)
cd /d "%~dp0"
"%CSC%" /nologo /target:winexe /platform:anycpu /win32manifest:app.manifest /r:System.dll /r:System.Core.dll /r:System.Drawing.dll /r:System.Windows.Forms.dll /out:ObligationKiller.exe ObligationKiller.cs
if errorlevel 1 (
    echo Build FAILED.
    pause
    exit /b 1
)
echo Built ObligationKiller.exe
pause
