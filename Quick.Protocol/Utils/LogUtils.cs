using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Quick.Protocol.Utils
{
    public class LogUtils
    {
        public const string NOT_SHOW_CONTENT_MESSAGE = "[NOT_SHOW: LogUtils.LogContent is False]";

        /// <summary>
        /// 是否记录心跳相关日志
        /// </summary>
        public static bool LogPackage { get; set; } = false;
        public static bool LogHeartbeat { get; set; } = false;
        public static bool LogNotice { get; set; } = false;
        public static bool LogCommand { get; set; } = false;
        public static bool LogContent { get; set; } = false;
        public static bool LogSplit{ get; set; } = false;
        public static bool LogConnection { get; set; } = false;
    }
}
