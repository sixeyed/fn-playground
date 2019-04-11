using System;
using log4net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace echoapi
{
    public class Program
    {       

        public static void Main(string[] args)
        {
            Logger.Log.Info("Echo Api starting...");            
            if (FnHelper.Format?.ToLower() != "http-stream")
            {
                Logger.Log.Fatal("FN_FORMAT=http-stream required");
                Environment.Exit(-1);
            }            
            if (FnHelper.Listener?.StartsWith("unix:") == false)
            {
                Logger.Log.Fatal("FN_LISTENER=unix:/... required");
                Environment.Exit(-1);
            }            
            var socketPath = FnHelper.InternalSocketPath;
            Logger.Log.Info($"Listening on unix socket: {socketPath}");
            CreateWebHostBuilder(socketPath, args).Build().Run();
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

