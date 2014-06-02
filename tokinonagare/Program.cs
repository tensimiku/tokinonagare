using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Configuration;
using System.IO;
using LinqToTwitter;


namespace tokinonagare
{
    class Program : parser
    {
        public static bool splitsend = true;
        public static bool mute = false;
        static void Main()
        {
            try
            {
                Task AsyncTask = StartAsyncTask();
                AsyncTask.Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Console.Write("\nPress any key to close console window...");
            Console.ReadKey(true);
        }

        static async Task StartAsyncTask()
        {
            string oauthpath = System.IO.Directory.GetCurrentDirectory() + "//oauth.txt";
            string oauthtoken = "";
            string oauthtokensecret = "";
            if (File.Exists(oauthpath))
            {
                StreamReader oauthreader = new StreamReader(oauthpath);
                oauthtoken = oauthreader.ReadLine();
                oauthtokensecret = oauthreader.ReadLine();
                oauthreader.Close();
            }
            IAuthorizer auth;
            if (oauthtoken != "" && oauthtoken != "")
            {
                auth = DoSingleUserAuth(oauthtoken, oauthtokensecret);
                await auth.AuthorizeAsync();
            }
            else
            {
                auth = DoPinOAuth();
                await auth.AuthorizeAsync();

                var credentials = auth.CredentialStore;
                string oauthToken = credentials.OAuthToken;
                string oauthTokenSecret = credentials.OAuthTokenSecret;
                string oauth = oauthToken + "/n";
                oauth += oauthTokenSecret;
                oauth = oauth.Replace("/n", System.Environment.NewLine);
                Console.WriteLine("Storing oauth : " + "\n" + oauthToken + "\n" + oauthTokenSecret);
                using (FileStream fs = File.Create(oauthpath))
                {
                    Byte[] txt = new UTF8Encoding(true).GetBytes(oauth);
                    fs.Write(txt, 0, txt.Length);
                }
            }


            

            var twitterCtx = new TwitterContext(auth);
            //await ReplyAsync(twitterCtx, 0, DateTime.Now + "에 켜졌습니다.");
            await DoUserStreamAsync(twitterCtx);

            Console.ReadKey();
        }
        static async Task SplitSend(TwitterContext twitterCtx, ulong tweetid, string statues)
        {
            List<string> arr = SplitString(statues);
            Console.WriteLine("Split System Triggered");
            foreach (string str in arr)
            {
                await ReplyAsync(twitterCtx, tweetid, str);
            }
        }
        static async Task UpdateTwit(TwitterContext twitterCtx,string statues)
        {
            ulong tweetid = tweetidparser(statues);
            string mesg = messageparser(statues);
            string chkstr = mesg.ToLower();
            if (mute)
            {
                if (chkstr.IndexOf("말") > 0)
                {
                    mute = false;
                    await ReplyAsync(twitterCtx, tweetid, "트윗을 합니다.");
                }
            }
            else
            {
                Console.WriteLine("Converted : " + mesg);
                if (chkstr.IndexOf("조용") > 0)
                {
                    mute = true;
                    mesg = "조용히 합니다. 말하기 전까지 모든 명령을 무시합니다.";
                }
                if (chkstr.IndexOf("꺼져") > 0)
                {
                    await ReplyAsync(twitterCtx, tweetid, "꺼집니다.");
                    System.Environment.Exit(0);
                }
                if (chkstr.IndexOf("분할") > 0)
                {
                    if (chkstr.IndexOf("켜") > 0)
                    {
                        splitsend = true;
                        mesg = "트윗 분할을 켭니다.";
                    }
                    else
                    {
                        splitsend = false;
                        mesg = "트윗 분할을 끕니다.";
                    }
                }
                if (chkstr.IndexOf("ip") > 0)
                {
                    mesg = DateTime.Now + "의 ipv4주소:" + ipv4parser(cmdparser("ipconfig"));
                }
                else if (chkstr.IndexOf("cmd:") > 0)
                {
                    mesg = System.Text.RegularExpressions.Regex.Matches(mesg, "\"(.*)\"", System.Text.RegularExpressions.RegexOptions.None)[0].Groups[1].Value;
                    mesg = cmdparser(mesg);
                    Console.WriteLine("Command-line prompt Triggerd");
                }
                if (splitsend && (mesg.Length > 120))
                {
                    await SplitSend(twitterCtx, tweetid, mesg);
                }
                else
                {
                    await ReplyAsync(twitterCtx, tweetid, mesg);
                }
            }
        }
        static async Task DoUserStreamAsync(TwitterContext twitterCtx)
        {
            Console.WriteLine("\nStreamed Content: \n");
            //int count = 0;

            await
                (from strm in twitterCtx.Streaming
                 where strm.Type == StreamingType.User
                 select strm)
                .StartAsync(async strm =>
                {
                    string message =
                        string.IsNullOrEmpty(strm.Content) ?
                            "Keep-Alive" : strm.Content;
                    if (chkengine(message))
                    {
                        Console.WriteLine("Update Task Run");
                        UpdateTwit(twitterCtx, message);
                    }
                    /*
                    Console.WriteLine(
                        (count + 1).ToString() +
                        ". " + DateTime.Now +
                        ": " + message + "\n");
                    if (count++ == 5)
                        strm.CloseStream();
                    */

                });
        }
        static async Task ReplyAsync(TwitterContext twitterCtx, ulong tweetid,string status)
        {
            var tweet = await twitterCtx.ReplyAsync(tweetid, "@changeityours "+status);
                if (tweet != null)
                    Console.WriteLine(
                        "Status returned: " +
                        "(" + tweet.StatusID + ")" +
                        tweet.User.Name + ", " + tweetid + ", " +
                        tweet.Text + "\n");
        }
        static List<String> SplitString(String text)
        {
            List<String> output = new List<String>();
            int overlenth = (text.Length/120)+1;
            string temp = String.Empty;
            for(int i=0;i<overlenth;i++){
                if ((i * 120 + 120) > text.Length)
                {
                    temp = text.Substring(i * 120);
                }
                else
                    temp = text.Substring(i*120,120);
                output.Add(temp);
                temp = String.Empty;
            }
            return output;
        }
        static bool chkengine(string msg)
        {
            string mentchk = mentidparser(msg);
            if (mentchk.ToLower() == "mentioncheckstring")
            {
                string idchk = idparser(msg);
                if (idchk.ToLower() == "idcheckstring")
                {
                    return true;
                }
            }
            return false;
        }
        static IAuthorizer ChooseAuthenticationStrategy()
        {
            IAuthorizer auth = null;

                auth = DoPinOAuth();

            return auth;
        }
        static IAuthorizer DoPinOAuth()
        {
            var auth = new PinAuthorizer()
            {
                CredentialStore = new InMemoryCredentialStore
                {
                    ConsumerKey = ConfigurationManager.AppSettings["consumerKey"],
                    ConsumerSecret = ConfigurationManager.AppSettings["consumerSecret"]
                },
                GoToTwitterAuthorization = pageLink => Process.Start(pageLink),
                GetPin = () =>
                {
                    Console.WriteLine(
                        "\nAfter authorizing this application, Twitter " +
                        "will give you a 7-digit PIN Number.\n");
                    Console.Write("Enter the PIN number here: ");
                    return Console.ReadLine();
                }
            };
            return auth;
        }
        static IAuthorizer DoSingleUserAuth(string oauth, string oauthsecret)
        {

            var auth = new SingleUserAuthorizer
            {
                CredentialStore = new SingleUserInMemoryCredentialStore
                {
                    ConsumerKey = ConfigurationManager.AppSettings["consumerKey"],
                    ConsumerSecret = ConfigurationManager.AppSettings["consumerSecret"],
                    AccessToken = oauth,
                    AccessTokenSecret = oauthsecret
                }
            };
            return auth;
        }
    }
}
