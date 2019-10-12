using Quick.Protocol.Packages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quick.Protocol.Commands
{
    public interface ICommand
    {
        /// <summary>
        /// 动作
        /// </summary>
        string Action { get; }
        /// <summary>
        /// 编号
        /// </summary>
        string Id { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        object Content { get; set; }
        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        ICommand Parse(CommandRequestPackage package);

        Task<CommandResponsePackage> ResponseTask { get; }
        void Timeout();
        void SetResponse(CommandResponsePackage responsePackage);
    }
}
