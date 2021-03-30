using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace Quick.Protocol
{
    /// <summary>
    /// 通知信息
    /// </summary>
    public class NoticeInfo
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 通知类型名称
        /// </summary>
        public string NoticeTypeName { get; set; }
        /// <summary>
        /// 通知类型
        /// </summary>
        [JsonIgnore]
        public Type NoticeType { get; set; }

        public NoticeInfo() { }
        public NoticeInfo(string name, Type noticeType)
        {
            Name = name;
            NoticeType = noticeType;
            NoticeTypeName = noticeType.FullName;
        }

        /// <summary>
        /// 创建通知信息实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static NoticeInfo Create<T>()
            where T : class, new()
        {
            return Create(typeof(T));
        }

        /// <summary>
        /// 创建通知信息实例
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static NoticeInfo Create(Type type)
        {
            var name = type.FullName;

            var attr = type.GetCustomAttribute<DisplayNameAttribute>();
            if (attr != null)
                name = attr.DisplayName;

            return new NoticeInfo(name, type);
        }

        /// <summary>
        /// 创建通知信息实例
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static NoticeInfo Create(object instance)
        {
            return Create(instance.GetType());
        }
    }
}
