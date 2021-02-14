using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncherReborn;

namespace GameLauncher.App.Classes.LauncherCore.APICheckers
{
    class VisualsAPIChecker
    {
        public static bool UnitedAPI = true;

        public static bool CarbonAPI = true;

        public static bool CarbonAPITwo = true;

        public static bool WOPLAPI = true;

        public static void PingAPIStatus()
        {
            switch (APIStatusChecker.CheckStatus(URLs.mainserver + "/serverlist.json"))
            {
                case APIStatus.Online:
                    break;
                default:
                    UnitedAPI = false;
                    break;
            }

            if (UnitedAPI == false)
            {
                switch (APIStatusChecker.CheckStatus(URLs.staticapiserver + "/serverlist.json"))
                {
                    case APIStatus.Online:
                        break;
                    default:
                        CarbonAPI = false;
                        break;
                }
            }

            if (CarbonAPI == false)
            {
                switch (APIStatusChecker.CheckStatus(URLs.secondstaticapiserver + "/serverlist.json"))
                {
                    case APIStatus.Online:
                        break;
                    default:
                        CarbonAPITwo = false;
                        break;
                }
            }

            if (CarbonAPITwo == false)
            {
                switch (APIStatusChecker.CheckStatus(URLs.woplserver + "/serverlist.json"))
                {
                    case APIStatus.Online:
                        break;
                    default:
                        WOPLAPI = false;
                        break;
                }
            }

            FunctionStatus.IsVisualAPIsChecked = true;
        }
    }
}
