using Newtonsoft.Json;
using ProtoBuf;
using Quick.Protocol.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.Protocol
{
    [ProtoContract]
    public class CommandRequestPackage : AbstractPackage
    {
        public override byte PackageType => 200;
        [ProtoMember(1)]
        public string Action { get; set; }
        [ProtoMember(2)]
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
            if (!string.IsNullOrEmpty(Content))
                cmd.Content = JsonConvert.DeserializeObject<TRequestContent>(Content);
            return cmd;
        }
    }
}
