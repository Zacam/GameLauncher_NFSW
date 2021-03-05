using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using GameLauncher.App.Classes.LauncherCore.Client.Web;
using GameLauncher.App.Classes.LauncherCore.Hashes;
using GameLauncher.App.Classes.LauncherCore.Lists;
using GameLauncher.App.Classes.LauncherCore.ModNet;
using GameLauncher.App.Classes.LauncherCore.Client;
using GameLauncher.App.Classes.LauncherCore.Proxy;
using GameLauncher.App.Classes.LauncherCore.RPC;
using GameLauncher.App.Classes.LauncherCore.FileReadWrite;

namespace GameLauncher
{
    static class Program 
    {
        /// <summary>
        /// Główny punkt wejścia dla aplikacji.
        /// </summary>
        [STAThread]
        static void Main() 
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (NFSW.IsNFSWRunning())
            {
                if (NFSW.DetectGameProcess())
                {
                    MessageBox.Show(null, "An instance of Need for Speed: World is already running", UserAgent.AgentAltName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    MessageBox.Show(null, "An instance of SBRW Launcher is already running.", UserAgent.AgentAltName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                Process.GetProcessById(Process.GetCurrentProcess().Id).Kill();
            }
            else
            {
                var mutex = new Mutex(false, UserAgent.AgentName);
                try
                {
                    if (mutex.WaitOne(0, false))
                    {
                        if (!File.Exists(FileSettingsSave.GameInstallation + "\\nfsw.exe"))
                        {
                            MessageBox.Show("nfsw.exe not found! Please put this launcher in the game directory. " +
                                "If you don't have the game installed, Use the Vanilla Launcher to install it (visit https://soapboxrace.world/)",
                                UserAgent.AgentAltName, MessageBoxButtons.OK, MessageBoxIcon.Error);

                            Process.GetProcessById(Process.GetCurrentProcess().Id).Kill();
                        }
                        else
                        {
                            if (!CanAccesGameData())
                            {
                                MessageBox.Show("This application requires admin priviledge. Restarting...");
                                RunAsAdmin();
                                return;
                            }

                            if (SHA.HashFile(FileSettingsSave.GameInstallation + "\\nfsw.exe") != "7C0D6EE08EB1EDA67D5E5087DDA3762182CDE4AC")
                            {
                                MessageBox.Show("Invalid file was detected, please restore original nfsw.exe", UserAgent.AgentAltName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                if (File.Exists(FileSettingsSave.GameInstallation + "\\.links"))
                                {
                                    var linksPath = Path.Combine(FileSettingsSave.GameInstallation + "\\.links");
                                    ModNetLinksCleanup.CleanLinks(linksPath);
                                }

                                ServicePointManager.Expect100Continue = true;
                                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                                ServerListUpdater.GetList();
                                DiscordLauncherPresense.Start("Start Up", "540651192179752970");
                                //ProxyServer.Start();

                                Application.Run(new ScreenLogin());
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show(null, "An instance of Launcher is already running.", UserAgent.AgentAltName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                finally
                {
                    mutex.Close();
                }
            }
        }

        static bool CanAccesGameData()
        {
            try
            {
                using (var test = File.OpenRead(FileSettingsSave.GameInstallation + "\\nfsw.exe"))
                {
                    
                }
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }

            return true;
        }

        public static void RunAsAdmin()
        {
            string[] args = Environment.GetCommandLineArgs();

            ProcessStartInfo processStartInfo = new ProcessStartInfo()
            {
                Verb = "runas",
                FileName = Application.ExecutablePath
            };

            if ((int)args.Length > 0)
            {
                processStartInfo.Arguments = args[0];
            }

            try
            {
                Process.Start(processStartInfo);
            }
            catch (Exception exception1)
            {
                MessageBox.Show("Failed to self-run as admin: " + exception1.Message);
            }
        }
    }
}
