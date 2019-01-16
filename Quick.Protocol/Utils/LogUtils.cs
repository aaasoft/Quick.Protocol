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

        public static ILogger GetCurrentClassLogger()
        {
            if (loggerFactory == null)
            {
                loggerFactory = new LoggerFactory();
#if DEBUG
                loggerFactory.AddConsole(LogLevel.Trace);
#endif
            }
            return loggerFactory.CreateLogger(new StackTrace(1).GetFrame(0).GetMethod().DeclaringType);
        }
    }
}
