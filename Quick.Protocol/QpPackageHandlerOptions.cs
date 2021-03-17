using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quick.Protocol
{
    public abstract class QpPackageHandlerOptions
    {
        /// <summary>
        /// 心跳间隔，为发送或接收超时中小的值的三分一
        /// </summary>
        public int HeartBeatInterval => InternalTransportTimeout / 3;
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        private QpInstruction[] _InstructionSet = new QpInstruction[] { Base.Instruction };
        /// <summary>
        /// 支持的指令集
        /// </summary>
        public QpInstruction[] InstructionSet
        {
            get { return _InstructionSet; }
            set
            {
                _InstructionSet = value;
                //必须加上QP基础指令集
                if (Array.IndexOf(_InstructionSet, Base.Instruction) < 0)
                    _InstructionSet = new QpInstruction[] { Base.Instruction }
                        .Concat(_InstructionSet)
                        .ToArray();
            }
        }

        /// <summary>
        /// 内部是否压缩
        /// </summary>
        internal virtual bool InternalCompress { get; set; } = false;
        /// <summary>
        /// 内部是否加密
        /// </summary>
        internal virtual bool InternalEncrypt { get; set; } = false;
        /// <summary>
        /// 内部接收超时(默认15秒)
        /// </summary>
        internal int InternalTransportTimeout { get; set; } = 15 * 1000;

        /// <summary>
        /// 最大包大小(默认为：10MB)
        /// </summary>
        public int MaxPackageSize { get; set; } = 10 * 1024 * 1024;

        public virtual void Check()
        {
            if (InternalTransportTimeout <= 0)
                throw new ArgumentException("TransportTimeout must larger than 0", nameof(InternalTransportTimeout));

            if (Password == null)
                throw new ArgumentNullException(nameof(Password));
        }
    }
}
