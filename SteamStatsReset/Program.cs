using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Steamworks;

namespace SteamStatsReset
{
    class Program
    {
        static Callback<UserStatsReceived_t> statsReceivedCallback;

        static void Main(string[] args)
        {
            if (SteamAPI.Init())
            {
                bool run = true;
                AppId_t currentAppId = SteamUtils.GetAppID();
                CSteamID currentUserId = SteamUser.GetSteamID();
                statsReceivedCallback = Callback<UserStatsReceived_t>.Create(p =>
                {
                    if (p.m_nGameID == currentAppId.m_AppId && p.m_steamIDUser == currentUserId)
                    {
                        if (SteamUserStats.ResetAllStats(false) && SteamUserStats.StoreStats())
                        {
                            Console.WriteLine("Stats reset.");
                        }
                        else
                        {
                            Console.WriteLine("Failed to reset stats");
                        }
                        run = false;
                    }
                });
                if (SteamUserStats.RequestCurrentStats())
                {
                    while (run)
                    {
                        SteamAPI.RunCallbacks();
                        System.Threading.Thread.Sleep(100);
                    }
                }
                else
                {
                    Console.WriteLine("Failed to request stats.");
                }
                SteamAPI.Shutdown();
            }
            else
            {
                Console.WriteLine("Cannot initialize Steamworks API. Please make sure you have a steam_appid.txt " +
                    "file with the target app ID in the same directory as this executable.");
            }
        }
    }
}
