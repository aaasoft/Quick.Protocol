using System;
using System.Collections.Generic;
using System.Text;

namespace Quick.Protocol.Commands
{
    /// <summary>
    /// 指令响应
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CommandResponse<T>
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
