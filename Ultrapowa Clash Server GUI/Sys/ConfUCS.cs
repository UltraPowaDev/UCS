using System;
using System.Net;
using CRS.Core;

namespace CRS.Sys
{
    public class ConfUCS
    {
        public static Version thisAppVer = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

        public static string Language = "en-US"; // Translation coming soon.

        public static bool DebugMode = true;
        public static bool EnableRemoteControl = false;
        public static bool IsConsoleMode = true; // Will start console first, overridable by config overridable by args.
        public static bool IsDefaultMode = false;
        public static bool IsLogEnabled = true;
        public static string LogDirectory = System.IO.Directory.GetCurrentDirectory() + @"\logs\";
        public static readonly string VersionUCS = string.Empty + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        public static string UnivTitle = "Clash Royale Spain Server " + VersionUCS +  " | " + "OFFLINE";
        public static string LogPath = "NONE";
        public static bool IsPreloaded = false;
        public static bool IsConsoleFirst = false;

        public static bool _IsServerOnline = false;
        public static bool IsServerOnline
        {
            get { return _IsServerOnline; }
            set
            {
                _IsServerOnline = value;
                ControlTimer.StartUpTimer();
                OnServerOnlineChange();
            }
        }
        public static bool AutoStartServer = true;


        // Updater section
        public static bool IsUpdateAvailable = false;
        public static Version NewVer = new Version();
        public static string UrlXML = "https://raw.githubusercontent.com/UltraPowaDev/UCS/refs/heads/main/system.xml"; // og url was: http://www.google.com
        public static string UrlPage = "https://github.com/UltraPowaDev/UCS/"; // og url was: http://www.ultrapowa.com
        public static string Changelog = "Error, changelog not downloaded...";

        public static string GetIP()
        {
            string HostName = Dns.GetHostName();
            return Dns.GetHostByName(HostName).AddressList[0].ToString();
        }

        public static event EventHandler OnServerOnlineEvent;

        public static void OnServerOnlineChange()
        {
            if (OnServerOnlineEvent != null) OnServerOnlineEvent(typeof(ConfUCS), EventArgs.Empty);
        }

        public void PreloadConfig()
        {

            // Here will preload and check for config errors.
        }

    }
}
