﻿using GameLauncherSimplified.App.Classes.SystemPlatform.Linux;
using GameLauncherSimplified.App.Classes.SystemPlatform.Windows;
using System.IO;

namespace GameLauncherSimplified.App.Classes.LauncherCore.FileReadWrite
{
    class FileSettingsSave
    {
        public static IniFile settingFile = new IniFile("Settings.ini");

        public static string GameInstallation = !string.IsNullOrEmpty(settingFile.Read("InstallationDirectory")) ? settingFile.Read("InstallationDirectory") : string.Empty;

        public static string CDN = !string.IsNullOrEmpty(settingFile.Read("CDN")) ? settingFile.Read("CDN") : "http://localhost/";

        public static string Lang = !string.IsNullOrEmpty(settingFile.Read("Language")) ? settingFile.Read("Language") : "EN";

        public static string Proxy = !string.IsNullOrEmpty(settingFile.Read("DisableProxy")) ? settingFile.Read("DisableProxy") : "0";

        public static string RPC = !string.IsNullOrEmpty(settingFile.Read("DisableRPC")) ? settingFile.Read("DisableRPC") : "0";

        public static string IgnoreVersion = !string.IsNullOrEmpty(settingFile.Read("IgnoreUpdateVersion")) ? settingFile.Read("IgnoreUpdateVersion") : string.Empty;

        public static string FirewallLauncherStatus = !string.IsNullOrEmpty(settingFile.Read("FirewallLauncher")) ? settingFile.Read("FirewallLauncher") : "Unknown";

        public static string FirewallGameStatus = !string.IsNullOrEmpty(settingFile.Read("FirewallGame")) ? settingFile.Read("FirewallGame") : "Unknown";

        public static string WindowsDefenderStatus = !string.IsNullOrEmpty(settingFile.Read("WindowsDefender")) ? settingFile.Read("WindowsDefender") : "Unknown";

        public static string Win7UpdatePatches = !string.IsNullOrEmpty(settingFile.Read("PatchesApplied")) ? settingFile.Read("PatchesApplied") : string.Empty;

        public static string FilePermissionStatus = !string.IsNullOrEmpty(settingFile.Read("FilePermission")) ? settingFile.Read("FilePermission") : "Not Set";

        public static void NullSafeSettings()
        {
            /* Migrate old Key Entries */
            if (settingFile.KeyExists("Server"))
            {
                FileAccountSave.ChoosenGameServer = settingFile.Read("Server");
                settingFile.DeleteKey("Server");
                FileAccountSave.SaveAccount();
            }

            if (settingFile.KeyExists("AccountEmail"))
            {
                FileAccountSave.UserRawEmail = settingFile.Read("AccountEmail");
                settingFile.DeleteKey("AccountEmail");
                FileAccountSave.SaveAccount();
            }

            if (settingFile.KeyExists("Password"))
            {
                FileAccountSave.UserHashedPassword = settingFile.Read("Password");
                settingFile.DeleteKey("Password");
                FileAccountSave.SaveAccount();
            }

            if (settingFile.KeyExists("Firewall"))
            {
                FirewallLauncherStatus = settingFile.Read("Firewall");
                settingFile.DeleteKey("Firewall");
            }

            /* Check if any Entries are missing */

            if (DetectLinux.LinuxDetected() && !settingFile.KeyExists("InstallationDirectory"))
            {
                settingFile.Write("InstallationDirectory", "GameFiles");
            }
            else if (!settingFile.KeyExists("InstallationDirectory"))
            {
                settingFile.Write("InstallationDirectory", GameInstallation);
            }
            else if (!File.Exists(GameInstallation) && !string.IsNullOrEmpty(GameInstallation))
            {
                Directory.CreateDirectory(GameInstallation);
            }

            if (!settingFile.KeyExists("CDN"))
            {
                settingFile.Write("CDN", CDN);
            }
            else if (settingFile.KeyExists("CDN"))
            {
                if (CDN.EndsWith("/"))
                {
                    char[] charsToTrim = { '/' };
                    string FinalCDNURL = CDN.TrimEnd(charsToTrim);

                    settingFile.Write("CDN", FinalCDNURL);
                }
            }

            if (!settingFile.KeyExists("Language"))
            {
                settingFile.Write("Language", Lang);
            }

            if (!settingFile.KeyExists("DisableProxy"))
            {
                settingFile.Write("DisableProxy", Proxy);
            }

            if (!settingFile.KeyExists("DisableRPC"))
            {
                settingFile.Write("DisableRPC", RPC);
            }

            if (!settingFile.KeyExists("IgnoreUpdateVersion"))
            {
                settingFile.Write("IgnoreUpdateVersion", IgnoreVersion);
            }

            if (!settingFile.KeyExists("FilePermission") && !DetectLinux.LinuxDetected())
            {
                settingFile.Write("FilePermission", FilePermissionStatus);
            }
            else if (settingFile.KeyExists("FilePermission") && DetectLinux.LinuxDetected())
            {
                settingFile.DeleteKey("FilePermission");
            }

            if (!DetectLinux.LinuxDetected())
            {
                if (!settingFile.KeyExists("FirewallLauncher"))
                {
                    settingFile.Write("FirewallLauncher", FirewallLauncherStatus);
                }

                if (FirewallLauncherStatus != "Unknown")
                {
                    FirewallGameStatus = FirewallLauncherStatus;
                }
                else if (!settingFile.KeyExists("FirewallGame"))
                {
                    settingFile.Write("FirewallGame", FirewallGameStatus);
                }

                if (WindowsProductVersion.GetWindowsNumber() >= 10.0)
                {
                    if (!settingFile.KeyExists("WindowsDefender"))
                    {
                        settingFile.Write("WindowsDefender", WindowsDefenderStatus);
                    }
                }
                else if (WindowsProductVersion.GetWindowsNumber() < 10.0)
                {
                    if (settingFile.KeyExists("WindowsDefender") || !string.IsNullOrEmpty(settingFile.Read("WindowsDefender")))
                    {
                        settingFile.DeleteKey("WindowsDefender");
                    }
                }

                if (WindowsProductVersion.GetWindowsNumber() == 6.1 && !settingFile.KeyExists("PatchesApplied"))
                {
                    settingFile.Write("PatchesApplied", Win7UpdatePatches);
                }
                else if (WindowsProductVersion.GetWindowsNumber() != 6.1 && settingFile.KeyExists("PatchesApplied"))
                {
                    settingFile.DeleteKey("PatchesApplied");
                }
            }

            /* Key Entries to Remove (No Longer Needed) */

            if (settingFile.KeyExists("LauncherPosX"))
            {
                settingFile.DeleteKey("LauncherPosX");
            }

            if (settingFile.KeyExists("LauncherPosY"))
            {
                settingFile.DeleteKey("LauncherPosY");
            }

            if (settingFile.KeyExists("DisableVerifyHash"))
            {
                settingFile.DeleteKey("DisableVerifyHash");
            }

            if (settingFile.KeyExists("TracksHigh"))
            {
                settingFile.DeleteKey("TracksHigh");
            }

            if (settingFile.KeyExists("ModNetDisabled"))
            {
                settingFile.DeleteKey("ModNetDisabled");
            }

            settingFile = new IniFile("Settings.ini");
        }

        public static void SaveSettings()
        {
            if (settingFile.Read("CDN") != CDN)
            {
                if (CDN.EndsWith("/"))
                {
                    char[] charsToTrim = { '/' };
                    string FinalCDNURL = CDN.TrimEnd(charsToTrim);

                    settingFile.Write("CDN", FinalCDNURL);
                }
                else
                {
                    settingFile.Write("CDN", CDN);
                }
            }

            if (settingFile.Read("Language") != Lang)
            {
                settingFile.Write("Language", Lang);
            }

            if (settingFile.Read("DisableProxy") != Proxy)
            {
                settingFile.Write("DisableProxy", Proxy);
            }

            if (settingFile.Read("DisableRPC") != RPC)
            {
                settingFile.Write("DisableRPC", RPC);
            }

            if (settingFile.Read("InstallationDirectory") != GameInstallation)
            {
                settingFile.Write("InstallationDirectory", GameInstallation);
            }

            if (settingFile.Read("IgnoreUpdateVersion") != IgnoreVersion)
            {
                settingFile.Write("IgnoreUpdateVersion", IgnoreVersion);
            }

            if (!DetectLinux.LinuxDetected())
            {
                if (settingFile.Read("FilePermission") != FilePermissionStatus)
                {
                    settingFile.Write("FilePermission", FilePermissionStatus);
                }

                if (settingFile.Read("FirewallLauncher") != FirewallLauncherStatus)
                {
                    settingFile.Write("FirewallLauncher", FirewallLauncherStatus);
                }

                if (settingFile.Read("FirewallGame") != FirewallGameStatus)
                {
                    settingFile.Write("FirewallGame", FirewallGameStatus);
                }

                if (WindowsProductVersion.GetWindowsNumber() >= 10.0)
                {
                    if (settingFile.Read("WindowsDefender") != WindowsDefenderStatus)
                    {
                        settingFile.Write("WindowsDefender", WindowsDefenderStatus);
                    }
                }

                if ((settingFile.Read("PatchesApplied") != Win7UpdatePatches) && WindowsProductVersion.GetWindowsNumber() == 6.1)
                {
                    settingFile.Write("PatchesApplied", Win7UpdatePatches);
                }
            }

            settingFile = new IniFile("Settings.ini");
        }
    }
}
