using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quick.Protocol
{
    /// <summary>
    /// 命令信息
    /// </summary>
    public class CommandInfo
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 命令请求类型名称
        /// </summary>
        public string RequestTypeName => RequestType.FullName;
        /// <summary>
        /// 命令响应类型名称
        /// </summary>
        public string ResponseTypeName => ResponseType.FullName;
        /// <summary>
        /// 命令请求类型
        /// </summary>
        [JsonIgnore]
        public Type RequestType { get; set; }
        /// <summary>
        /// 命令响应类型
        /// </summary>
        [JsonIgnore]
        public Type ResponseType { get; set; }

        public CommandInfo() { }
        public CommandInfo(Type requestType, Type responseType)
        {
            RequestType = requestType;
            ResponseType = responseType;
        }

        /// <summary>
        /// 创建命令信息实例
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <returns></returns>
        public static CommandInfo Create<TRequest, TResponse>()
        {
            return new CommandInfo(typeof(TRequest), typeof(TResponse));
        }
    }
}
