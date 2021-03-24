using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SteamKit2;
using SteamKit2.GC;
using SteamKit2.GC.CSGO.Internal;
using SteamKit2.Internal;
using Fishware_v5.DataEntry;

namespace Fishware_v5.DataEntry
{
    class BanCheck
    {
        public class AccountDetails
        {
            public string profile_url = string.Empty;
            public string profile_img_url = string.Empty;

            public string public_name = string.Empty;

            public string login = string.Empty;
            public ulong steam64id;
            public int penalty_reason = -1;
            public int penalty_seconds = -1;
            public int wins = -1;
            public int rank = -1;
        }

        private const int SLEEP = 2000;

        private SteamGameCoordinator SteamGameCoordinator;
        private SteamClient steamClient;
        private SteamUser steamUser;
        private CallbackManager manager;

        private static List<AccountDetails> accounts = new List<AccountDetails>();

        private bool isRunning = false;

        private string username = string.Empty;
        private string password = string.Empty;
        private ulong steamid;

        private bool AcknowledgedPenalty = false;

        private Stopwatch sw = new Stopwatch();

        public BanCheck(string login, string pass)
        {
            username = login;
            password = pass;

            steamClient = new SteamClient();
            manager = new CallbackManager(steamClient);

            steamUser = steamClient.GetHandler<SteamUser>();
            SteamGameCoordinator = steamClient.GetHandler<SteamGameCoordinator>();

            manager.Subscribe<SteamClient.ConnectedCallback>(OnConnected);
            manager.Subscribe<SteamClient.DisconnectedCallback>(OnDisconnected);

            manager.Subscribe<SteamApps.VACStatusCallback>(OnVACStatus);

            manager.Subscribe<SteamUser.LoggedOnCallback>(OnLoggedOn);
            manager.Subscribe<SteamUser.LoggedOffCallback>(OnLoggedOff);

            manager.Subscribe<SteamGameCoordinator.MessageCallback>(OnMessageCall);

        }

        public void Run()
        {
            isRunning = true;

            steamClient.Connect();

            while (isRunning)
            {
                manager.RunWaitAllCallbacks(TimeSpan.FromSeconds(5));
                if (sw.Elapsed.Seconds > 5)
                {
                    Console.WriteLine("Resending CMsgClientHello");

                    var ClientHello = new ClientGCMsgProtobuf<CMsgClientHello>((uint)EGCBaseClientMsg.k_EMsgGCClientHello);
                    SteamGameCoordinator.Send(ClientHello, 730);

                    sw.Restart();

                }
            }
        }

        private void OnConnected(SteamClient.ConnectedCallback callback)
        {
            Console.WriteLine($"Connected to Steam, logging in as {username}");

            steamUser.LogOn(new SteamUser.LogOnDetails
            {
                Username = username,
                Password = password,
            });
        }

        private void OnDisconnected(SteamClient.DisconnectedCallback callback)
        {
            Console.WriteLine($"{username}: Disconnected from Steam");
            isRunning = false;
        }

        private void OnVACStatus(SteamApps.VACStatusCallback callback)
        {
            if (callback.BannedApps.Contains(730))
            {
                Console.WriteLine($"Banned: {username}");
                steamUser.LogOff();
            }
        }

        private void OnLoggedOn(SteamUser.LoggedOnCallback callback)
        {
            if (callback.Result != EResult.OK)
            {
                if (callback.Result == EResult.AccountLogonDenied)
                {
                    Console.WriteLine($"{username}: Unable to logon to Steam: This account is SteamGuard protected");

                    isRunning = false;
                    return;
                }

                Console.WriteLine($"{username}: Unable to logon to Steam: {callback.Result} / {callback.ExtendedResult}");

                isRunning = false;
                return;
            }

            Console.WriteLine("Successfully logged on!");
            steamid = callback.ClientSteamID.ConvertToUInt64();

            var Play = new ClientMsgProtobuf<CMsgClientGamesPlayed>(EMsg.ClientGamesPlayed);
            Play.Body.games_played.Add(new CMsgClientGamesPlayed.GamePlayed { game_id = new GameID(730), });
            steamClient.Send(Play);

            Thread.Sleep(SLEEP);

            var ClientHello = new ClientGCMsgProtobuf<CMsgClientHello>((uint)EGCBaseClientMsg.k_EMsgGCClientHello);
            SteamGameCoordinator.Send(ClientHello, 730);

            sw.Start();
        }

        private void OnLoggedOff(SteamUser.LoggedOffCallback callback)
        {
            if (callback.Result.ToString().Contains("LoggedInElsewhere"))
            {
                accounts.Add(new AccountDetails()
                {
                    login = $"{username}:{password}",
                    steam64id = steamid
                });
            }
            else
            {
                Console.WriteLine($"Logged off of Steam: {callback.Result}");
            }
        }

        private void OnMessageCall(SteamGameCoordinator.MessageCallback callback)
        {
            Console.WriteLine(callback.EMsg.ToString());

            switch (callback.EMsg)
            {
                case (uint)EGCBaseClientMsg.k_EMsgGCClientHello:
                case (uint)EGCBaseClientMsg.k_EMsgGCClientWelcome:
                    {
                        sw.Stop();

                        var ClientHello = new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchmakingGC2ClientHello>((uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchmakingClient2GCHello);
                        SteamGameCoordinator.Send(ClientHello, 730);

                        break;
                    }
                case (uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchmakingGC2ClientHello:
                    {
                        var details = new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchmakingGC2ClientHello>(callback.Message);

                        if (details.Body.vac_banned == 0)
                        {
                            var penalty_seconds = Math.Abs(details.Body.penalty_seconds);
                            if (penalty_seconds > 0 && !AcknowledgedPenalty)
                            {
                                AcknowledgedPenalty = true;

                                Console.WriteLine("k_EMsgGCCStrike15_v2_AcknowledgePenalty");

                                var AcknowledgePenalty = new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_AcknowledgePenalty>((uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_AcknowledgePenalty);
                                AcknowledgePenalty.Body.acknowledged = 1;

                                SteamGameCoordinator.Send(AcknowledgePenalty, 730);

                                Thread.Sleep(SLEEP);

                                var ClientHello = new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchmakingGC2ClientHello>((uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchmakingClient2GCHello);
                                SteamGameCoordinator.Send(ClientHello, 730);

                                return;
                            }

                            if (details.Body.penalty_reason == 18)
                            {
                                steamUser.LogOff();
                                return;
                            }

                            if (details.Body.penalty_reason == 10)
                            {
                                Console.WriteLine($"{username}: global cooldown");
                                steamUser.LogOff();
                                return;
                            }

                            if (details.Body.ranking == null || penalty_seconds > 0)
                            {

                                Console.WriteLine($"{username}: penalty_seconds > 0");
                                Console.WriteLine($"{username}: penalty_reason = {details.Body.penalty_reason}");

                                Thread.Sleep(SLEEP);

                                var ClientHello = new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchmakingGC2ClientHello>((uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchmakingClient2GCHello);
                                SteamGameCoordinator.Send(ClientHello, 730);

                                return;
                            }

                            var extra = extraData(steamid);

                            DataAccess.AddAccounts(extra.public_name, username, password, (int)details.Body.ranking.wins, details.Body.player_level, extra.profile_link, extra.profile_pic);
                            Debug.WriteLine("Account added to database");
                            accounts.Add(new AccountDetails()
                            {
                                public_name = extra.public_name,
                                profile_url = extra.profile_link,
                                profile_img_url = extra.profile_pic,
                                login = $"{username}:{password}",
                                steam64id = steamid,
                                penalty_reason = (int)details.Body.penalty_reason,
                                penalty_seconds = (int)details.Body.penalty_seconds,
                                wins = (int)details.Body.ranking.wins,
                                rank = details.Body.player_level
                            });
                        }
                        steamUser.LogOff();
                        break;
                    }
                default: break;
            }
        }

        private (string profile_link, string profile_pic, string public_name) extraData(ulong steam64)
        {
            string api_key = "NOOOOOOOOOOOOOOOOOOO";
            string api_url = new WebClient().DownloadString($"https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={api_key}&steamids={steam64}");

            dynamic jsonobject = JsonConvert.DeserializeObject<dynamic>(api_url);
            var player = jsonobject["response"]["players"][0];

            return (player["profileurl"], player["avatarfull"], player["personaname"]);
        }
    }
}
