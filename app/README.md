# ObligationKiller

One-tap GUI for the `clear-account-record.ps1` logic: backs up and removes the
registry key `HKCU\Software\Microsoft\Windows\CurrentVersion\NetControlPort`
that the Windscribe desktop client writes locally.

## Run

Double-click `ObligationKiller.exe` - no installation, no admin rights,
no PowerShell execution-policy issues.

## Features

- Live status dot (green = key absent, red = key present), refreshed every 3 s.
- Automatic `.reg` backup before every delete; aborts without deleting if the
  backup fails.
- Backup list with **Restore** and **Folder** buttons.
- Finds and starts `Windscribe.exe` (standard install paths + uninstall
  registry entries, with a file picker as fallback).

## Rebuild

Double-click `build.bat` - it uses the C# compiler that ships with Windows
(`%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\csc.exe`). No Visual Studio
or .NET SDK needed.

Same local-only limitation as the PowerShell script: it touches nothing on
any server and does not create, grant, extend, or restore any subscription.

---

Made with ❤️ by ashkanthefirst.
