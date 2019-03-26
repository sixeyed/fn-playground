using System;
using Mono.Unix;

namespace echoapi
{
    //variables set by Fn runtime    
    // see https://github.com/fnproject/docs/blob/master/fn/develop/fn-format.md

    public static class FnHelper
    {
        public static string Format
        {
            get
            {
                return Environment.GetEnvironmentVariable("FN_FORMAT");
            }
        }

        public static string Listener
        {
            get
            {
                return Environment.GetEnvironmentVariable("FN_LISTENER");
            }
        }

        public static string ListenerSocketPath
        {
            get
            {
                var fnListener = Environment.GetEnvironmentVariable("FN_LISTENER");
                if (fnListener?.StartsWith("unix:") == true)
                {
                    return fnListener?.Substring("unix:".Length);
                }
                return null;
            }
        }

        public static void EnsureListenerSocket()
        {
            var socketPath = ListenerSocketPath;
            Logger.Log.Info($"Changing permissions on: {socketPath}");
            try
            {
                var socketInfo = new UnixFileInfo(socketPath);
                socketInfo.FileAccessPermissions =
                  FileAccessPermissions.UserRead | FileAccessPermissions.UserWrite |
                  FileAccessPermissions.GroupRead | FileAccessPermissions.GroupWrite |
                  FileAccessPermissions.OtherRead | FileAccessPermissions.OtherWrite;
                Logger.Log.Info("Set socket permissions to 0666");
            }
            catch (Exception ex)
            {
                Logger.Log.Fatal($"Unable to change socket permissions, exception: {ex}");
                Environment.Exit(-1);
            }
        }
    }
}