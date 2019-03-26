using System;
using System.IO;
using System.Reflection;
using System.Xml;
using log4net;

namespace echoapi
{
    public static class Logger
    {
        public static readonly ILog Log;

        static Logger()
        {
            var log4netConfig = new XmlDocument();
            log4netConfig.Load(File.OpenRead("log4net.config"));

            var repo = LogManager.CreateRepository(
                Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));

            log4net.Config.XmlConfigurator.Configure(repo, log4netConfig["log4net"]);

            Log = LogManager.GetLogger(typeof(Program));
        }
    }
}