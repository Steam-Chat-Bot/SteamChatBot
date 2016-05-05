using System;
using System.IO;

namespace SteamChatBot_Mono
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Log Log;
            string username;
            string password;
            string logFile;
            string sentryFile;
            string displayName;
            string cll;
            string fll;
            string ss;
            UserInfo _data;

            Console.WriteLine("Username: ");
            username = Console.ReadLine();
            Bot.username = username;

            if(File.Exists(username + "/login.json"))
            {
                _data = Bot.ReadData(username);
                logFile = _data.logFile;
                sentryFile = _data.sentryFile;
                username = _data.username;
                password = _data.password;
                displayName = _data.displayName;
                cll = _data.cll;
                fll = _data.fll;
                ss = _data.sharedSecret;

                Log = Log.CreateInstance(logFile, username, (Log.LogLevel)Enum.Parse(typeof(Log.LogLevel), cll, true),
                    (Log.LogLevel)Enum.Parse(typeof(Log.LogLevel), fll, true));
                Log.Instance.Silly("Initialized logger.");
                Log.Instance.Silly("Read login data from file " + username + "/login.json");

                Bot.Start(username, password, cll, fll, logFile, displayName, sentryFile, ss);
            }
            else
            {
                Console.WriteLine("Password: ");
                password = Console.ReadLine();

                Console.WriteLine("Display Name: ");
                displayName = Console.ReadLine();

                Console.WriteLine("Log File (optional): ");
                logFile = Console.ReadLine();

                Console.WriteLine("Sentry File (optional): ");
                sentryFile = Console.ReadLine();

                Console.WriteLine("Console Log Level (Silly, Debug, Verbose, Info, Warn, Error) (optional): ");
                cll = Console.ReadLine();

                Console.WriteLine("File Log Level (Silly, Debug, Verbose, Info, Warn, Error) (optional): ");
                fll = Console.ReadLine();

                Console.WriteLine("Shared secret (mobile auth) (optional): ");
                ss = Console.ReadLine();

                Log = Log.CreateInstance((logFile == "" ? username + ".log" : logFile), username,
                    (cll == "" ? (Log.LogLevel)Enum.Parse(typeof(Log.LogLevel), "Verbose", true) :
                        (Log.LogLevel)Enum.Parse(typeof(Log.LogLevel), cll.ToString(), true)),
                    (fll == "" ? (Log.LogLevel)Enum.Parse(typeof(Log.LogLevel), "Verbose", true) :
                        (Log.LogLevel)Enum.Parse(typeof(Log.LogLevel), fll.ToString(), true)));
                
                Bot.Start(
                          username,
                          password,
                          (cll == "" ? "Verbose" : cll), 
                          (fll == "" ? "Verbose" : fll),
                          (logFile == "" ? username + ".log" : logFile),
                          displayName, 
                          (sentryFile == "" ? username + ".sentry" : sentryFile),
                          ss);
            }
        }
    }
}
