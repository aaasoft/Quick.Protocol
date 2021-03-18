using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        public string NoticeTypeName => NoticeType.FullName;
        /// <summary>
        /// 通知类型
        /// </summary>
        [JsonIgnore]
        public Type NoticeType { get; set; }

        public NoticeInfo() { }
        public NoticeInfo(Type noticeType)
        {
            NoticeType = noticeType;
        }

        /// <summary>
        /// 创建通知信息实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static NoticeInfo Create<T>()
            where T : class, new()
        {
            return new NoticeInfo(typeof(T));
        }
    }
}
