using System;
using System.IO;
using Mono.Unix;

namespace echoapi
{
    //variables set by Fn runtime    
    // see https://github.com/fnproject/docs/blob/master/fn/develop/fn-format.md

    public static class FnHelper
    {
        private static UnixSymbolicLinkInfo _SymLink;

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

        public static string InternalSocketPath
        {
            get; private set;
        }

        static FnHelper()
        {
            var fileName = $"dotnet-fdk-{Guid.NewGuid().ToString().Substring(0,6)}.sock";
            var listenerDirectory = Path.GetDirectoryName(ListenerSocketPath);            
            InternalSocketPath = Path.Combine(listenerDirectory, fileName);
        }

        public static void EnsureListenerSocket()
        {
            var socketPath = InternalSocketPath;
            Logger.Log.Info($"Configuring socket, internal: {InternalSocketPath}; listener: {ListenerSocketPath}");
            try
            {
                var socketInfo = new UnixFileInfo(socketPath);
                _SymLink = socketInfo.CreateSymbolicLink(ListenerSocketPath);
                Logger.Log.Info($"Created symlink, target: {InternalSocketPath}, source: {ListenerSocketPath}");

                socketInfo.FileAccessPermissions =
                  FileAccessPermissions.UserRead | FileAccessPermissions.UserWrite |
                  FileAccessPermissions.GroupRead | FileAccessPermissions.GroupWrite |
                  FileAccessPermissions.OtherRead | FileAccessPermissions.OtherWrite;
                Logger.Log.Info("Set socket permissions to 0666");                
            }
            catch (Exception ex)
            {
                Logger.Log.Fatal($"Unable to configure socket, exception: {ex}");
                Environment.Exit(-1);
            }
        }

        public static void RemoveListenerSocket()
        {
            var socketPath = InternalSocketPath;
            Logger.Log.Info($"Deleting socket: {socketPath}");
            try
            {
                if (_SymLink != null)
                {
                    _SymLink.Delete();
                }
                File.Delete(socketPath);
                Logger.Log.Info("Socket deleted");
            }
            catch (Exception ex)
            {
                Logger.Log.Fatal($"Unable to delete socket, exception: {ex}");
                Environment.Exit(-1);
            }
        }
    }
}