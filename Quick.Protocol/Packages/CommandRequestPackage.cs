using Newtonsoft.Json;
using ProtoBuf;
using Quick.Protocol.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.Protocol.Packages
{
    [ProtoContract]
    public class CommandRequestPackage : AbstractPackage
    {
        public override byte PackageType => 0;
        [ProtoMember(1)]
        public string Id { get; set; }
        [ProtoMember(2)]
        public string Action { get; set; }
        [ProtoMember(3)]
        public string Content { get; set; }

        /// <summary>
        /// 转换为指令
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TRequestContent"></typeparam>
        /// <typeparam name="TResponseData"></typeparam>
        /// <returns></returns>
        public T ToCommand<T, TRequestContent, TResponseData>()
            where T : AbstractCommand<TRequestContent, TResponseData>, new()
        {
            var cmd = new T();
            if (cmd.Action != Action)
                throw new IOException($"Action not match.Package's Action is '{Action}' and Command's Action is '{cmd.Action}'");
            cmd.Id = Id;
            if (!string.IsNullOrEmpty(Content))
                cmd.ContentT = JsonConvert.DeserializeObject<TRequestContent>(Content);
            return cmd;
        }
    }
}
