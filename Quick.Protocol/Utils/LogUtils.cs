using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Quick.Protocol.Utils
{
    public class LogUtils
    {
        private static LoggerFactory loggerFactory;

        private class ConsoleLogger : ILogger
        {
            private string categoryName;
            public ConsoleLogger(string categoryName)
            {
                this.categoryName = categoryName;
            }

            public IDisposable BeginScope<TState>(TState state)
            {
                return null;
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return true;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                Console.WriteLine($"[{categoryName}][{logLevel}]{formatter(state, exception)}");
            }
        }

        private class ConsoleProvider : ILoggerProvider
        {
            public ILogger CreateLogger(string categoryName)
            {
                return new ConsoleLogger(categoryName);
            }

            public void Dispose()
            {
            }
        }

        /// <summary>
        /// 是否记录心跳相关日志
        /// </summary>
        public static bool LogHeartbeat { get; set; } = false;
        public static bool LogPackage { get; set; } = false;
        public static bool LogCommand { get; set; } = false;
        public static bool LogCommandContent { get; set; } = false;

        /// <summary>
        /// 增加控制台输出
        /// </summary>
        public static void AddConsole()
        {
            GetLoggerFactory().AddProvider(new ConsoleProvider());
        }

        public static LoggerFactory GetLoggerFactory()
        {
            if (loggerFactory == null)
                loggerFactory = new LoggerFactory();
            return loggerFactory;
        }

        public static ILogger GetCurrentClassLogger()
        {
            return GetLoggerFactory().CreateLogger(new StackTrace(1).GetFrame(0).GetMethod().DeclaringType);
        }
    }
}
