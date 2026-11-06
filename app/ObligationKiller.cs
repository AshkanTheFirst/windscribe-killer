// ObligationKiller - one-tap cleanup of the Windscribe expired-account record.
// Wraps the logic of clear-account-record.ps1 in a small WinForms app:
// closes the client (optional), backs up and deletes
//   HKCU\Software\Microsoft\Windows\CurrentVersion\NetControlPort
// so another free account can be added. No admin rights required.

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using System.Windows.Forms;

namespace ObligationKiller
{
    public class MainForm : Form
    {
        const string KeyPath = "Software\\Microsoft\\Windows\\CurrentVersion\\NetControlPort";
        const string RegPath = "HKCU\\" + KeyPath;

        readonly string backupDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "backups");

        Label statusDot;
        Label statusText;
        CheckBox chkKillClient;
        CheckBox chkRelaunch;
        Button btnKill;
        Button btnOpenWindscribe;
        ComboBox cmbBackups;
        Button btnRestore;
        Button btnOpenFolder;
        TextBox txtLog;

        public MainForm()
        {
            Text = "Windscribe Obligation Killer";
            Font = new Font("Segoe UI", 9.75F);
            AutoScaleMode = AutoScaleMode.Font;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(440, 560);

            var title = new Label
            {
                Text = "Windscribe Obligation Killer",
                Font = new Font("Segoe UI", 15F, FontStyle.Bold),
                Location = new Point(16, 10),
                AutoSize = true
            };
            var subtitle = new Label
            {
                Text = "One-tap cleanup of the local expired-account record:",
                ForeColor = Color.DimGray,
                Location = new Point(18, 46),
                AutoSize = true
            };
            var regHint = new Label
            {
                Text = RegPath,
                Font = new Font("Segoe UI", 8.25F),
                ForeColor = Color.DimGray,
                Location = new Point(18, 68),
                AutoSize = true
            };

            var grpStatus = new GroupBox { Text = "Status", Location = new Point(16, 90), Size = new Size(408, 62) };
            statusDot = new Label { Text = "\u25CF", Font = new Font("Segoe UI", 13F, FontStyle.Bold), Location = new Point(12, 21), AutoSize = true };
            statusText = new Label { Text = "...", Location = new Point(34, 25), AutoSize = true };
            var btnRefresh = new Button { Text = "Refresh", Location = new Point(318, 20), Size = new Size(78, 28) };
            btnRefresh.Click += delegate { RefreshStatus(); RefreshBackups(); };
            grpStatus.Controls.Add(statusDot);
            grpStatus.Controls.Add(statusText);
            grpStatus.Controls.Add(btnRefresh);

            var grpOptions = new GroupBox { Text = "Options", Location = new Point(16, 160), Size = new Size(408, 66) };
            chkKillClient = new CheckBox { Text = "Close Windscribe client first (recommended)", Location = new Point(12, 19), AutoSize = true, Checked = true };
            chkRelaunch = new CheckBox { Text = "Open Windscribe automatically after clearing", Location = new Point(12, 41), AutoSize = true };
            grpOptions.Controls.Add(chkKillClient);
            grpOptions.Controls.Add(chkRelaunch);

            btnKill = new Button
            {
                Text = "\u2620  KILL THE OBLIGATION",
                Font = new Font("Segoe UI", 12.5F, FontStyle.Bold),
                BackColor = Color.Firebrick,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(16, 236),
                Size = new Size(408, 52)
            };
            btnKill.FlatAppearance.BorderSize = 0;
            btnKill.Click += delegate { KillObligation(); };

            btnOpenWindscribe = new Button { Text = "Open Windscribe  \u2192  add a new free account", Location = new Point(16, 296), Size = new Size(408, 34) };
            btnOpenWindscribe.Click += delegate { OpenWindscribe(); };

            var grpBackups = new GroupBox { Text = "Backups (created automatically on every clear)", Location = new Point(16, 340), Size = new Size(408, 62) };
            cmbBackups = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(12, 22), Size = new Size(218, 26) };
            btnRestore = new Button { Text = "Restore", Location = new Point(238, 21), Size = new Size(80, 28) };
            btnRestore.Click += delegate { RestoreBackup(); };
            btnOpenFolder = new Button { Text = "Folder", Location = new Point(324, 21), Size = new Size(72, 28) };
            btnOpenFolder.Click += delegate { OpenBackupsFolder(); };
            grpBackups.Controls.Add(cmbBackups);
            grpBackups.Controls.Add(btnRestore);
            grpBackups.Controls.Add(btnOpenFolder);

            txtLog = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Location = new Point(16, 412),
                Size = new Size(408, 112),
                BackColor = Color.White
            };

            var credit = new Label
            {
                Text = "made with \u2764 by ashkanthefirst",
                ForeColor = Color.DimGray,
                Font = new Font("Segoe UI", 8.25F),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 532),
                Size = new Size(440, 22)
            };

            Controls.Add(title);
            Controls.Add(subtitle);
            Controls.Add(regHint);
            Controls.Add(grpStatus);
            Controls.Add(grpOptions);
            Controls.Add(btnKill);
            Controls.Add(btnOpenWindscribe);
            Controls.Add(grpBackups);
            Controls.Add(txtLog);
            Controls.Add(credit);

            Activated += delegate { RefreshStatus(); };
            var t = new Timer { Interval = 3000 };
            t.Tick += delegate { RefreshStatus(); };
            t.Start();

            RefreshStatus();
            RefreshBackups();

            Log("How to use:");
            Log("  1) When the client blocks this machine, tap KILL THE OBLIGATION.");
            Log("  2) Start the client again and continue with the account of your choice.");
            Log("     The client re-creates this record by itself sometimes.");
            Log("  3) A .reg backup is saved on every clear; restore it below.");
            Log("");
        }

        bool RecordExists()
        {
            using (RegistryKey k = Registry.CurrentUser.OpenSubKey(KeyPath)) return k != null;
        }

        void RefreshStatus()
        {
            if (RecordExists())
            {
                statusDot.ForeColor = Color.Firebrick;
                statusText.ForeColor = Color.Firebrick;
                statusText.Text = "Record PRESENT - your account is used up";
            }
            else
            {
                statusDot.ForeColor = Color.SeaGreen;
                statusText.ForeColor = Color.SeaGreen;
                statusText.Text = "No expired-account record - clean";
            }
        }

        void Log(string msg)
        {
            txtLog.AppendText(msg + Environment.NewLine);
        }

        static int RunReg(string args)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "reg.exe",
                Arguments = args,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            using (Process p = Process.Start(psi))
            {
                p.StandardOutput.ReadToEnd();
                p.StandardError.ReadToEnd();
                p.WaitForExit();
                return p.ExitCode;
            }
        }

        void KillObligation()
        {
            btnKill.Enabled = false;
            try
            {
                Log("---- " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ----");
                if (!RecordExists())
                {
                    Log("Nothing to do: no expired-account record present.");
                    return;
                }
                if (chkKillClient.Checked) CloseWindscribe();

                Directory.CreateDirectory(backupDir);
                string backupFile = Path.Combine(backupDir, "netcontrolport-" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".reg");
                int rc = RunReg("export \"" + RegPath + "\" \"" + backupFile + "\" /y");
                if (rc != 0 || !File.Exists(backupFile))
                {
                    Log("Backup FAILED (reg exit " + rc + ") - nothing was deleted.");
                    return;
                }
                Log("Backup saved: " + Path.GetFileName(backupFile));

                rc = RunReg("delete \"" + RegPath + "\" /f");
                if (RecordExists())
                {
                    Log("DELETE FAILED (reg exit " + rc + ") - key still present.");
                    return;
                }
                Log("Expired-account record deleted.");
                Log("NEXT: start Windscribe and continue with the account of your choice.");
                if (chkRelaunch.Checked) OpenWindscribe();
            }
            finally
            {
                RefreshStatus();
                RefreshBackups();
                btnKill.Enabled = true;
            }
        }

        void CloseWindscribe()
        {
            Process[] procs = Process.GetProcessesByName("Windscribe");
            if (procs.Length == 0)
            {
                Log("Windscribe client is not running.");
                return;
            }
            foreach (Process p in procs)
            {
                try
                {
                    p.Kill();
                    Log(p.WaitForExit(4000)
                        ? "Closed Windscribe (pid " + p.Id + ")."
                        : "Windscribe (pid " + p.Id + ") did not exit in time.");
                }
                catch (Exception ex)
                {
                    Log("Could not close Windscribe (pid " + p.Id + "): " + ex.Message);
                }
                finally { p.Dispose(); }
            }
        }

        void OpenWindscribe()
        {
            string exe = FindWindscribe();
            if (exe == null)
            {
                using (var dlg = new OpenFileDialog { Title = "Locate Windscribe.exe", Filter = "Windscribe.exe|Windscribe.exe" })
                {
                    if (dlg.ShowDialog(this) != DialogResult.OK) return;
                    exe = dlg.FileName;
                }
            }
            try
            {
                Process.Start(exe);
                Log("Started Windscribe: " + exe);
            }
            catch (Exception ex)
            {
                Log("Could not start Windscribe: " + ex.Message);
            }
        }

        static string FindWindscribe()
        {
            string pf = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            string pf86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            string lad = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string[] candidates =
            {
                Path.Combine(pf, "Windscribe", "Windscribe.exe"),
                Path.Combine(pf, "Windscribe2", "Windscribe.exe"),
                Path.Combine(pf86, "Windscribe", "Windscribe.exe"),
                Path.Combine(pf86, "Windscribe2", "Windscribe.exe"),
                Path.Combine(lad, "Windscribe", "Windscribe.exe"),
                Path.Combine(lad, "Windscribe2", "Windscribe.exe")
            };
            foreach (string c in candidates) if (File.Exists(c)) return c;

            string[] roots =
            {
                "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall",
                "SOFTWARE\\WOW6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall"
            };
            RegistryKey[] hives = { Registry.LocalMachine, Registry.CurrentUser };
            foreach (RegistryKey hive in hives)
            {
                foreach (string r in roots)
                {
                    using (RegistryKey k = hive.OpenSubKey(r))
                    {
                        if (k == null) continue;
                        foreach (string sub in k.GetSubKeyNames())
                        {
                            using (RegistryKey sk = k.OpenSubKey(sub))
                            {
                                if (sk == null) continue;
                                string name = sk.GetValue("DisplayName") as string;
                                if (name == null || name.IndexOf("Windscribe", StringComparison.OrdinalIgnoreCase) < 0) continue;
                                string loc = (sk.GetValue("InstallLocation") as string ?? "").Trim();
                                if (loc.Length == 0) continue;
                                string guess = Path.Combine(loc, "Windscribe.exe");
                                if (File.Exists(guess)) return guess;
                            }
                        }
                    }
                }
            }
            return null;
        }

        void RestoreBackup()
        {
            string sel = cmbBackups.SelectedItem as string;
            if (sel == null || sel.StartsWith("("))
            {
                Log("No backup selected.");
                return;
            }
            string file = Path.Combine(backupDir, sel);
            int rc = RunReg("import \"" + file + "\"");
            Log(rc == 0 && RecordExists()
                ? "Restored backup: " + sel
                : "Restore FAILED (reg exit " + rc + ").");
            RefreshStatus();
        }

        void OpenBackupsFolder()
        {
            Directory.CreateDirectory(backupDir);
            Process.Start("explorer.exe", "\"" + backupDir + "\"");
        }

        void RefreshBackups()
        {
            string keep = cmbBackups.SelectedItem as string;
            cmbBackups.Items.Clear();
            if (Directory.Exists(backupDir))
            {
                string[] files = Directory.GetFiles(backupDir, "netcontrolport-*.reg")
                    .Select(Path.GetFileName)
                    .OrderByDescending(f => f)
                    .ToArray();
                cmbBackups.Items.AddRange(files);
            }
            if (cmbBackups.Items.Count == 0) cmbBackups.Items.Add("(no backups yet)");
            int idx = (keep != null && cmbBackups.Items.Contains(keep)) ? cmbBackups.Items.IndexOf(keep) : 0;
            cmbBackups.SelectedIndex = idx;
        }
    }

    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.ThreadException += (s, e) => MessageBox.Show(e.Exception.ToString(), "ObligationKiller - unexpected error");
            AppDomain.CurrentDomain.UnhandledException += (s, e) => MessageBox.Show(e.ExceptionObject.ToString(), "ObligationKiller - fatal error");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
