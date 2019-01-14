using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quick.Protocol.Packages
{
    /// <summary>
    /// 心跳包
    /// </summary>
    [ProtoContract]
    public class HeartBeatPackage : AbstractPackage
    {
        public override byte PackageType => 1;
        public static HeartBeatPackage Instance { get; } = new HeartBeatPackage();
    }
}
