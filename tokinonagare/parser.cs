using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Configuration;
using LinqToTwitter;

namespace tokinonagare
{
    class parser
    {
        public static ulong tweetidparser(string msg)
        {
            ulong tweetid = 0;
            int a = msg.IndexOf("\"id_str\":\"");
            if (a > 0)
            {
                a = a + 10;
                int b = msg.IndexOf("\"", a);
                msg = msg.Substring(a, b - a);
                if (msg.Length != 0)
                {
                    tweetid = Convert.ToUInt64(msg);
                }
            }
            return tweetid;
        }
        public static string cmdparser(string msg)
        {
            ProcessStartInfo cmd = new ProcessStartInfo();
            Process process = new Process();
            cmd.FileName = @"cmd";
            cmd.CreateNoWindow = true;
            cmd.UseShellExecute = false;

            cmd.RedirectStandardOutput = true;
            cmd.RedirectStandardInput = true;
            cmd.RedirectStandardError = true;
            process.StartInfo = cmd;
            process.Start();
            process.StandardInput.Write(msg + Environment.NewLine);
            process.StandardInput.Close();
            string cmdline = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            process.Close();
            cmdline = (new System.Text.RegularExpressions.Regex(@">.*?([\r\n](.*)){1,}"))
                             .Matches(cmdline)[0].ToString();
            return cmdline;
        }
        public static string ipv4parser(string msg)
        {
            msg = (new System.Text.RegularExpressions.Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}"))
                             .Matches(msg)[0].ToString();
            return msg;
        }
        public static string mentidparser(string msg)
        {
            int a = msg.IndexOf("\"user_mentions\":[{\"screen_name\":\"");
            if (a > 0)
            {
                a = a + 33;
                int b = msg.IndexOf("\"", a);
                msg = msg.Substring(a, b - a);
            }
            return msg;
        }
        public static string idparser(string msg)
        {
            int a = msg.IndexOf("\"screen_name\":\"");
            if (a > 0)
            {
                a = a + 15;
                int b = msg.IndexOf("\"", a);
                msg = msg.Substring(a, b - a);
            }
            return msg;
        }
        public static string messageparser(string msg)
        {
            
            int a = msg.IndexOf("\"text\":");
            msg = msg.Substring(a, msg.IndexOf("\"source\":") - a);

            msg = System.Text.RegularExpressions.Regex.Matches(msg, "\"text\":\"(.*)\",", System.Text.RegularExpressions.RegexOptions.None)[0].Groups[1].Value;
            msg = System.Text.RegularExpressions.Regex.Unescape(msg);
            return msg;
        }
    }
}