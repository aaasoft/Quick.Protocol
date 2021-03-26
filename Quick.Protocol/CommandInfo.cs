using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
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
        public CommandInfo(string name, Type requestType, Type responseType)
        {
            Name = name;
            RequestType = requestType;
            ResponseType = responseType;
        }


        /// <summary>
        /// 创建命令信息实例
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <returns></returns>
        public static CommandInfo Create<TResponse>(IQpCommandRequest<TResponse> request)
            where TResponse : class, new()
        {
            var requestType = request.GetType();
            var name = requestType.FullName;

            var attr = requestType.GetCustomAttribute<DisplayNameAttribute>();
            if (attr != null)
                name = attr.DisplayName;

            return new CommandInfo(name, requestType, typeof(TResponse));
        }
    }
}
