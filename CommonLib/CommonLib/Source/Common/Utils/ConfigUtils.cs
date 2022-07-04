using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CommonLib.Source.Common.Converters;
using CommonLib.Source.Common.Extensions;
using Microsoft.Extensions.Configuration;

namespace CommonLib.Source.Common.Utils
{
    public static class ConfigUtils
    {
        private static readonly SemaphoreSlim _syncConfigFIle = new(1, 1);
        private static string _frontendBaseUrl;
        private static string _backendBaseUrl;
        private static string _frontendDBCS;
        private static string _backendDBCS;

        public static IConfiguration GetFromAppSettings() => new ConfigurationBuilder().AddJsonFile("appsettings.json", false).Build();
        public static NetworkCredential GetRPCNetworkCredential(string rpcKey)
        {
            var jCredentials = File.ReadAllText("appsettings.json").JsonDeserialize()["RPCs"]?[rpcKey]; // using ConfigurationBUilder causing values to be cached even with 'reload on change' option
            return new NetworkCredential(jCredentials?["User"]?.ToString(), jCredentials?["Password"]?.ToString(), $"http://{jCredentials?["Address"]?.ToString().BetweenOrWhole("://", "/")}/");
        }

        public static string FrontendBaseUrl
        {
            get => _frontendBaseUrl;
            set
            {
                var oldValue = _frontendBaseUrl;
                _frontendBaseUrl = value;
                OnFrontendBaseUrlChanging(oldValue, _frontendBaseUrl);
            }
        }

        public static string BackendBaseUrl
        {
            get => _backendBaseUrl;
            set
            {
                var oldValue = _backendBaseUrl;
                _backendBaseUrl = value;
                OnBackendBaseUrlChanging(oldValue, _backendBaseUrl);
            }
        }

        public static string FrontendDBCS
        {
            get => _frontendDBCS;
            set
            {
                var oldValue = _frontendDBCS;
                _frontendDBCS = value;
                OnFrontendDBCSChanging(oldValue, _frontendDBCS);
            }
        }
        
        public static string BackendDBCS
        {
            get => _backendDBCS;
            set
            {
                var oldValue = _backendDBCS;
                _backendDBCS = value;
                OnBackendDBCSChanging(oldValue, _backendDBCS);
            }
        }

        public static string CssPath => "~/wwwroot/css/Styles.css";
        
        public static async Task SetAppSettingValueAsync(string key, string value, string appSettingsJsonFilePath = null)
        {
            if (appSettingsJsonFilePath == null)
            {
                var assemblyName = Assembly.GetEntryAssembly()?.GetName().Name;
                if (assemblyName == null)
                    throw new NullReferenceException(nameof(assemblyName));
                var pathBeforeAssembly = System.AppContext.BaseDirectory.BeforeLast(assemblyName);
                appSettingsJsonFilePath = Path.Combine(pathBeforeAssembly, assemblyName, "appsettings.json");
            }

            await _syncConfigFIle.WaitAsync();

            var json = await File.ReadAllTextAsync(appSettingsJsonFilePath);
            var jConfig = json.JsonDeserialize();
            key = key.Replace(":", ".");
            var keysButLast = key.BeforeLastOrNull(".");
            var lastKey = key.AfterLastOrWhole(".");
            var jParent = keysButLast != null ? jConfig.SelectToken(keysButLast) : jConfig;
            if (jParent == null)
            {
                _syncConfigFIle.Release();
                throw new NullReferenceException(nameof(jParent));
            }
                
            jParent[lastKey] = value;
            var modifiedJson = jConfig.JsonSerialize();

            await File.WriteAllTextAsync(appSettingsJsonFilePath, modifiedJson);

            _syncConfigFIle.Release();
        }

        public static event StringChangedEventHandler FrontendBaseUrlChanged;
        public static event StringChangedEventHandler BackendBaseUrlChanged;
        public static event StringChangedEventHandler FrontendDBCSChanged;
        public static event StringChangedEventHandler BackendDBCSChanged;

        private static void OnFrontendBaseUrlChanging(StringChangedEventArgs e) => FrontendBaseUrlChanged?.Invoke(null, e);
        private static void OnFrontendBaseUrlChanging(string changedFrom, string changedTo) => OnFrontendBaseUrlChanging(new StringChangedEventArgs(changedFrom, changedTo));
        private static void OnBackendBaseUrlChanging(StringChangedEventArgs e) => BackendBaseUrlChanged?.Invoke(null, e);
        private static void OnBackendBaseUrlChanging(string changedFrom, string changedTo) => OnBackendBaseUrlChanging(new StringChangedEventArgs(changedFrom, changedTo));
        private static void OnFrontendDBCSChanging(StringChangedEventArgs e) => FrontendDBCSChanged?.Invoke(null, e);
        private static void OnFrontendDBCSChanging(string changedFrom, string changedTo) => OnFrontendDBCSChanging(new StringChangedEventArgs(changedFrom, changedTo));
        private static void OnBackendDBCSChanging(StringChangedEventArgs e) => BackendDBCSChanged?.Invoke(null, e);
        private static void OnBackendDBCSChanging(string changedFrom, string changedTo) => OnBackendDBCSChanging(new StringChangedEventArgs(changedFrom, changedTo));
    }

    public delegate void StringChangedEventHandler(object sender, StringChangedEventArgs e);

    public class StringChangedEventArgs
    {
        public string ChangedFrom { get; }
        public string ChangedTo { get; }

        public StringChangedEventArgs(string changedFrom, string changedTo)
        {
            ChangedFrom = changedFrom;
            ChangedTo = changedTo;
        }
    }
}
