<div align="center">

# 🪓 Windscribe Killer

**One tap, and the Windscribe client forgets your machine ever saw an expired account.**

**Windscribe Killer** is the unofficial **Windscribe account-record cleaner**
for Windows. A tiny, zero-install utility that removes the local record the
Windscribe desktop client keeps about used-up free accounts — so the client
treats your machine as brand new and lets you add another free account.

[![Windows](https://img.shields.io/badge/Windows-10%20%7C%2011-0078d6?logo=windows11&logoColor=white)](#-why-this-exists-the-windscribe-device-limit)
[![Admin rights](https://img.shields.io/badge/admin%20rights-not%20required-2ea44f)](#-safety--reversibility)
[![Dependencies](https://img.shields.io/badge/dependencies-zero-orange)](#-project-layout)
[![EXE size](https://img.shields.io/badge/EXE-~18%20KB-8a2be2)](#-project-layout)
[![License](https://img.shields.io/badge/license-MIT-green)](LICENSE)

[Why](#-why-this-exists-the-windscribe-device-limit) · [Features](#-features) · [Quick start](#-quick-start) · [CLI](#️-cli-usage) · [FAQ](#-faq)

</div>

> [!TIP]
> **Found this through a search?** You probably typed one of these:
> *windscribe free account limit* · *windscribe already used on this device* ·
> *windscribe expired account* · *windscribe netcontrolport* ·
> *reset windscribe on this pc* · *windscribe registry cleaner* —
> this tool is the fix, and it works in one tap.

```text
┌─────────────────────────────────────────────┐
│  Windscribe Obligation Killer               │
│                                             │
│  ● Record PRESENT — your account is used up │
│                                             │
│  [x] Close Windscribe client first          │
│  [ ] Open Windscribe automatically after    │
│                                             │
│  ╔═══════════════════════════════════════╗  │
│  ║        KILL THE OBLIGATION            ║  │
│  ╚═══════════════════════════════════════╝  │
│                                             │
│  Backups  netcontrolport-20261106-235924.reg│
│           [ Restore ]   [ Folder ]          │
│                                             │
│  log  record deleted. run Windscribe now    │
└─────────────────────────────────────────────┘
```

*The whole app. Nothing to install, nothing to configure.*

## 🤔 Why this exists: the Windscribe device limit

Windscribe's free plan is limited per **device**, not just per account. When a
free account runs out, the Windscribe client doesn't simply forget it — it
writes a small **encrypted record** into the Windows registry that marks this
machine as "already used". From then on, signing up with a brand-new free
account changes nothing: the client reads the record and blocks the **device**,
not the account.

That Windscribe record lives here:

```
HKCU\Software\Microsoft\Windows\CurrentVersion\NetControlPort
```

Windscribe Killer removes exactly that key — with a backup, every single time —
and the Windscribe client starts fresh. Nothing else is touched.

> [!IMPORTANT]
> **Local-only by design.** The tool never talks to Windscribe's servers, never
> modifies any account, and cannot create, grant, extend, or restore any
> Windscribe subscription. It backs up and deletes one `HKCU` key. That's all
> it does.

## ✨ Features

- **One-tap Windscribe cleanup** — no installer, no admin rights, no PowerShell
  policy prompts
- **Live status dot** — 🟢 clean / 🔴 record present, refreshed every 3 seconds
- **Backup-first** — exports a timestamped `.reg` backup before every delete;
  if the backup fails, nothing is deleted
- **One-click restore** — any backup can be restored from the dropdown, or by
  double-clicking the `.reg` file
- **Windscribe-client-aware** — optionally closes `Windscribe.exe` before
  cleaning and relaunches it afterwards (finds it in standard paths like
  `Program Files\Windscribe` and `Windscribe2`, uninstall entries, or via file
  picker)
- **Two flavors** — a GUI (`app/ObligationKiller.exe`) and a CLI script
  (`clear-account-record.ps1`) with the same behavior

The flow is always: **close Windscribe *(optional)* → export backup → delete the record → relaunch Windscribe *(optional)***.

## 🚀 Quick start

1. Double-click `app\ObligationKiller.exe`.
2. The status dot tells you where you stand: 🔴 = Windscribe has a record
   about this machine.
3. Tap **KILL THE OBLIGATION** — the key is backed up to `app\backups\` and removed.
4. Start Windscribe and add another free account.

That's it. The EXE is a single ~18 KB WinForms binary built against the
.NET Framework that already ships with Windows 10 and 11 — no runtime to
install, no admin prompt, and it works from any folder (e.g. a USB stick).

## ️⌨️ CLI usage

Prefer the terminal, or want to automate it? Use `clear-account-record.ps1`:

```powershell
.\clear-account-record.ps1                # backup, then remove
.\clear-account-record.ps1 -KillClient    # close the Windscribe client first
.\clear-account-record.ps1 -NoBackup      # skip the backup (not recommended)
.\clear-account-record.ps1 -BackupDir D:\backups
```

| Parameter | Effect |
|-----------|--------|
| *(none)* | Export a timestamped `.reg` backup to `backups\`, then delete the key |
| `-KillClient` | Stop running `Windscribe.exe` processes first, so the key isn't rewritten right away |
| `-NoBackup` | Delete without exporting a backup |
| `-BackupDir <path>` | Store backups in a custom folder |

No patience for execution policies? Double-click `clear-account-record.bat` —
it runs the script with `-ExecutionPolicy Bypass` and UTF-8 output, and
forwards any arguments:

```
clear-account-record.bat -KillClient
```

## 🛠️ Building from source

No Visual Studio, no .NET SDK — the C# compiler ships with Windows itself.
Just double-click:

```
app\build.bat
```

It compiles `app\ObligationKiller.cs` with the in-box compiler
(`%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\csc.exe`) and refreshes
`ObligationKiller.exe` in place. Five seconds, zero setup.

## 🔒 Safety & reversibility

- Everything happens under `HKCU` — the current user's own registry area — so
  **admin rights are never required**.
- Every delete is preceded by a timestamped `.reg` export. **Backup failed →
  delete never happens.**
- Restore anytime: double-click a backup file, or pick it in the app and hit
  **Restore**.
- The Windscribe client recreates the key on its own in some situations —
  that's normal. The status dot tells you when it's back; run the tool again.

## ❓ FAQ

<details>
<summary><b>Windscribe says the free plan was already used on this device — does this fix that?</b></summary>

Yes, that's exactly what this tool is for. That block comes from the local
`NetControlPort` registry record, not from your account. Removing the record
makes the Windscribe client treat the machine as fresh.
</details>

<details>
<summary><b>Is this a Windscribe crack, keygen, or premium-account generator?</b></summary>

No. It's a registry cleaner for one specific key. It cannot touch Windscribe's
servers, your account, or anyone else's — it only changes what the Windscribe
client <i>remembers locally</i> about this machine.
</details>

<details>
<summary><b>Why does the record come back?</b></summary>

The Windscribe client recreates the key by itself when it sees an expired
account again. That's expected — just run the tool again. The status dot shows
the current state at all times.
</details>

<details>
<summary><b>Does it work while Windscribe is running?</b></summary>

Yes, but the client may rewrite the key immediately after cleaning. Tick
<b>Close Windscribe client first</b> in the GUI (or use <code>-KillClient</code>)
to avoid that.
</details>

<details>
<summary><b>Is it portable?</b></summary>

Fully. One `.exe` with no dependencies, no installation, no registry entries of
its own. Run it from anywhere — including a USB stick.
</details>

<details>
<summary><b>Why should I trust the binary?</b></summary>

You don't have to. The full source is in this repo and builds in five seconds
with <code>app\build.bat</code> using only what Windows already ships with.
</details>

## ⚖️ Fair play

Windscribe Killer is an independent tool and is **not affiliated with or
endorsed by Windscribe**. It changes nothing on Windscribe's side. Whether and
how you use it is your call — keep in mind it may conflict with Windscribe's
Terms of Service, and your account remains subject to them.

## 🧩 Project layout

| Path | What it is |
|------|------------|
| `app/ObligationKiller.exe` | One-tap GUI — a single ~18 KB WinForms executable |
| `app/ObligationKiller.cs` | GUI source (C#, WinForms) |
| `app/build.bat` | Rebuilds the EXE with the compiler built into Windows |
| `clear-account-record.ps1` | The original PowerShell version |
| `clear-account-record.bat` | Double-click launcher for the script |
| `backups/` | Timestamped `.reg` backups land here |

## Credits

Made with ❤️ by **AshkanTheFirst** — based on the open-source
*Account Record Cleaner* project (MIT).

## License

[MIT](LICENSE) — do whatever, just keep the notice.

## 🔎 Keywords

`windscribe` · `windscribe free account limit` · `windscribe device already used` ·
`windscribe expired account` · `windscribe netcontrolport` ·
`reset windscribe on this pc` · `windscribe registry cleaner` ·
`windscribe account record cleaner` · `windscribe windows 10` · `windscribe windows 11` ·
`windscribe killer` · `obligation killer` · `regdust`
