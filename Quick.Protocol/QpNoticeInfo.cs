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
    public class QpNoticeInfo
    {
        private Type noticeType;

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 通知类型名称
        /// </summary>
        public string NoticeTypeName { get; set; }
        
        public QpNoticeInfo() { }
        public QpNoticeInfo(string name, Type noticeType)
        {
            Name = name;
            this.noticeType = noticeType;
            NoticeTypeName = noticeType.FullName;
        }

        /// <summary>
        /// 获取通知类型
        /// </summary>
        public Type GetNoticeType() => noticeType ?? Type.GetType(NoticeTypeName);

        /// <summary>
        /// 创建通知信息实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static QpNoticeInfo Create<T>()
            where T : class, new()
        {
            return Create(typeof(T));
        }

        /// <summary>
        /// 创建通知信息实例
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static QpNoticeInfo Create(Type type)
        {
            var name = type.FullName;

            var attr = type.GetCustomAttribute<DisplayNameAttribute>();
            if (attr != null)
                name = attr.DisplayName;

            return new QpNoticeInfo(name, type);
        }

        /// <summary>
        /// 创建通知信息实例
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static QpNoticeInfo Create(object instance)
        {
            return Create(instance.GetType());
        }
    }
}
