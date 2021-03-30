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
    public class QpCommandInfo
    {
        /// <summary>
        /// 名称
        /// </summary>
        [DisplayName("名称")]
        [ReadOnly(true)]
        public string Name { get; set; }
        /// <summary>
        /// 命令请求类型名称
        /// </summary>
        [DisplayName("请求类型")]
        [ReadOnly(true)]
        public string RequestTypeName { get; set; }
        /// <summary>
        /// 命令响应类型名称
        /// </summary>
        [DisplayName("响应类型")]
        [ReadOnly(true)]
        public string ResponseTypeName { get; set; }

        private Type requestType;

        private Type responseType;

        public QpCommandInfo() { }
        public QpCommandInfo(string name, Type requestType, Type responseType)
        {
            Name = name;
            this.requestType = requestType;
            RequestTypeName = requestType.FullName;
            this.responseType = responseType;
            ResponseTypeName = responseType.FullName;
        }

        /// <summary>
        /// 获取命令请求类型
        /// </summary>
        /// <returns></returns>
        public Type GetRequestType() => requestType ?? Type.GetType(RequestTypeName);
        /// <summary>
        /// 获取命令响应类型
        /// </summary>
        /// <returns></returns>
        public Type GetResponseType() => responseType ?? Type.GetType(ResponseTypeName);

        /// <summary>
        /// 创建命令信息实例
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <returns></returns>
        public static QpCommandInfo Create<TResponse>(IQpCommandRequest<TResponse> request)
            where TResponse : class, new()
        {
            var requestType = request.GetType();
            var name = requestType.FullName;

            var attr = requestType.GetCustomAttribute<DisplayNameAttribute>();
            if (attr != null)
                name = attr.DisplayName;

            return new QpCommandInfo(name, requestType, typeof(TResponse));
        }
    }
}
