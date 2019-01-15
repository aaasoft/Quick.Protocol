using Quick.Protocol.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quick.Protocol
{
    public class Base
    {
        public static Instruction Instruction => new Instruction()
        {
            Id = typeof(Base).FullName,
            Name = "基础指令集"
        };
    }
}
