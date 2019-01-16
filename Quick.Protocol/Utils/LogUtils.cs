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

        private class ConsoleProvider : ILoggerProvider
        {
            public ILogger CreateLogger(string categoryName)
            {
                throw new NotImplementedException();
            }

            public void Dispose()
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 增加控制台输出
        /// </summary>
        public static void AddConsole()
        {
            GetLoggerFactory().AddConsole(LogLevel.Trace);
        }

        private static LoggerFactory GetLoggerFactory()
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
