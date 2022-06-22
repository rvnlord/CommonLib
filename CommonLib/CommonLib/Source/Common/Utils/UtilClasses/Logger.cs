using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using CommonLib.Source.Common.Extensions;
using CommonLib.Source.Common.Extensions.Collections;
using CommonLib.Source.Common.Utils.TypeUtils;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace CommonLib.Source.Common.Utils.UtilClasses
{
    public class Logger
    {
        private static LoggingConfiguration _config;
        private string _class;
        private NLog.Logger _nLogger;

        public static string LogPath { get; set; }

        public Logger Configure(string path = null, Type type = null)
        {
            if (_config == null || path != null)
            {
                _config = new LoggingConfiguration();

                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Create("browser")))
                {
                    LogPath ??= path ?? $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).BeforeFirstOrWholeIgnoreCase(@"\bin")}\ErrorLog.log";
                    var logfile = new FileTarget("logfile")
                    {
                        FileNameKind = FilePathKind.Absolute,
                        FileName = LogPath,
                        Layout = "${message}"
                    };
                    _config.AddRule(LogLevel.Trace, LogLevel.Fatal, logfile);

                    var logConsole = new ColoredConsoleTarget("logconsole") { Layout = "${message}" };
                    logConsole.RowHighlightingRules.Add(new ConsoleRowHighlightingRule
                    {
                        Condition = "level == LogLevel.Trace",
                        ForegroundColor = ConsoleOutputColor.DarkGreen
                    });
                    logConsole.RowHighlightingRules.Add(new ConsoleRowHighlightingRule
                    {
                        Condition = "level == LogLevel.Debug",
                        ForegroundColor = ConsoleOutputColor.DarkGray
                    });
                    logConsole.RowHighlightingRules.Add(new ConsoleRowHighlightingRule
                    {
                        Condition = "level == LogLevel.Info",
                        ForegroundColor = ConsoleOutputColor.Green
                    });
                    logConsole.RowHighlightingRules.Add(new ConsoleRowHighlightingRule
                    {
                        Condition = "level == LogLevel.Warn",
                        ForegroundColor = ConsoleOutputColor.Yellow
                    });
                    logConsole.RowHighlightingRules.Add(new ConsoleRowHighlightingRule
                    {
                        Condition = "level == LogLevel.Error",
                        ForegroundColor = ConsoleOutputColor.Red
                    });
                    logConsole.RowHighlightingRules.Add(new ConsoleRowHighlightingRule
                    {
                        Condition = "level == LogLevel.Fatal",
                        ForegroundColor = ConsoleOutputColor.DarkRed
                    });
                    _config.AddRule(LogLevel.Trace, LogLevel.Fatal, logConsole);
                }
                else
                {
                    var logConsole = new ConsoleTarget("logconsole") { Layout = "${message}" };
                    _config.AddRule(LogLevel.Trace, LogLevel.Fatal, logConsole);
                    var logDebugger = new DebuggerTarget("logconsole") { Layout = "${message}" };
                    _config.AddRule(LogLevel.Trace, LogLevel.Fatal, logDebugger);
                }
                
                var logDebug = new DebuggerTarget("logdebug") { Layout = "${message}" };
                _config.AddRule(LogLevel.Trace, LogLevel.Fatal, logDebug);

                LogManager.Configuration = _config;
            }

            if (type != null)
            {
                _class = type.FullName;
                _nLogger = LogManager.GetLogger(_class);
            }
            
            return this;
        }

        public Logger Log(LogLevel severity, string message)
        {
            if (_config == null)
                throw new Exception("Configure Logger First");

            _nLogger = LogManager.GetLogger(_class);
            var severityLength = severity.Name.ToLowerInvariant().Length;
            var fillerChars = StringUtils.Repeat(" ", 6 - severityLength);
            var spacing = StringUtils.Repeat(" ", 7);
            _nLogger.Log(severity, $"{severity.Name.ToLowerInvariant()}:{fillerChars}{DateTime.Now:dd-MM-yyyy HH:mm:ss:fff} {_class}\n{spacing}{message}");
            return this;
        }

        public Logger Debug(string message) => Log(LogLevel.Debug, message);
        public Logger Info(string message) => Log(LogLevel.Info, message);
        public Logger Trace(string message) => Log(LogLevel.Trace, message);
        public Logger Warn(string message) => Log(LogLevel.Warn, message);
        public Logger Error(string message) => Log(LogLevel.Error, message);
        public Logger Fatal(string message) => Log(LogLevel.Fatal, message);

        public static Logger For<T>(string path = null) => new Logger().Configure(path, typeof(T));
        public static Logger For(Type type, string path = null) => new Logger().Configure(path, type);
        public static void Close() => LogManager.Shutdown();
    }
}
