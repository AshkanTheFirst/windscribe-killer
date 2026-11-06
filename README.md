# regdust

A tiny Windows housekeeping utility. It backs up and removes one registry key
under `HKCU` that the **Windscribe** desktop client writes on the machine:

```
HKCU\Software\Microsoft\Windows\CurrentVersion\NetControlPort
```

After the key is removed, the client keeps no local bookkeeping about accounts
it has seen on this machine and starts fresh. The client may recreate the key
on its own at any time.

> [!IMPORTANT]
> regdust is a **local-only** tool. It never talks to any server, it does not
> modify any account, and it does not create, grant, extend, or restore any
> subscription. It only backs up and deletes that one `HKCU` key.

## What's inside

| File | What it is |
|------|------------|
| `app/ObligationKiller.exe` | One-tap GUI - no install, no admin rights |
| `app/ObligationKiller.cs` | GUI source (C#, builds with the compiler built into Windows) |
| `clear-account-record.ps1` | Original PowerShell version |
| `clear-account-record.bat` | Double-click launcher for the script |

## Usage

### GUI (recommended)

Double-click `app\ObligationKiller.exe`:

1. The status dot shows whether the key is currently present (green = clean).
2. Tap the big red button - the key is backed up to `app\backups\` and removed.
3. Start the client and continue with the account of your choice.

Any backup can be restored later from the dropdown inside the app.

### PowerShell script

```powershell
# default - backup, then remove
.\clear-account-record.ps1

# close the client first so the key is not rewritten right away
.\clear-account-record.ps1 -KillClient

# without a backup
.\clear-account-record.ps1 -NoBackup

# custom backup folder
.\clear-account-record.ps1 -BackupDir D:\backups
```

or double-click `clear-account-record.bat` - it runs the script with
`-ExecutionPolicy Bypass` and UTF-8 output; arguments work there too:

```
clear-account-record.bat -KillClient
```

### Building the GUI from source

Double-click `app\build.bat` - it uses the C# compiler that ships with Windows
(`%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\csc.exe`). No Visual Studio
and no .NET SDK required.

## Notes

- Everything happens under `HKCU`; admin rights are not required.
- Removing the key is reversible - double-click any `.reg` backup (or use the
  **Restore** button in the app) to put it back.
- The client recreates the key by itself in some situations; that is normal
  and this tool can simply be run again.

## Credits

Made with ❤️ by **AshkanTheFirst** - based on the open-source
*Account Record Cleaner* project (MIT).

## License

MIT - see [LICENSE](LICENSE).
