using System;
using System.IO;
using System.Reflection;
using System.Xml;
using log4net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace echoapi
{
    public class Program
    {
        static ILog _Log;

        public static void Main(string[] args)
        {
            InitializeLog4Net();

            _Log.Info("Echo Api starting...");
            //fn environment variables are needed first:
            // see https://github.com/fnproject/docs/blob/master/fn/develop/fn-format.md
            var fnFormat = Environment.GetEnvironmentVariable("FN_FORMAT");
            if (fnFormat?.ToLower() != "http-stream")
            {
                _Log.Fatal("FN_FORMAT=http-stream required");
                Environment.Exit(-1);
            }
            var fnListener = Environment.GetEnvironmentVariable("FN_LISTENER");
            if (fnListener?.StartsWith("unix:") == false)
            {
                _Log.Fatal("FN_LISTENER=unix:/... required");
                Environment.Exit(-1);
            }
            var socketPath = fnListener.Substring("unix:".Length);
            _Log.Info($"Listening on unix socket: {socketPath}");
            CreateWebHostBuilder(socketPath, args).Build().Run();
        }

        private static void InitializeLog4Net()
        {
            var log4netConfig = new XmlDocument();
            log4netConfig.Load(File.OpenRead("log4net.config"));

            var repo = LogManager.CreateRepository(
                Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));

            log4net.Config.XmlConfigurator.Configure(repo, log4netConfig["log4net"]);

            _Log = LogManager.GetLogger(typeof(Program));
        }

        public static IWebHostBuilder CreateWebHostBuilder(string socketPath, string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseLibuv()
                .UseKestrel(options =>
                {
                    options.ListenUnixSocket(socketPath);
                });
    }
}
