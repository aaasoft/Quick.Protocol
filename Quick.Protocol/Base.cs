using System;
using System.Collections.Generic;
using System.Text;

namespace Quick.Protocol
{
    public class Base
    {
        public static QpInstruction Instruction => new QpInstruction()
        {
            Id = typeof(Base).FullName,
            Name = "基础指令集",
            SupportPackages = new Packages.IPackage[]
            {
                new Packages.CommandRequestPackage(),
                new Packages.CommandResponsePackage(),
                new Packages.SplitPackage(),
                Packages.HeartBeatPackage.Instance
            },
            SupportCommands = new Commands.ICommand[]
            {
                new Commands.WelcomeCommand(),
                new Commands.AuthenticateCommand(),
                new Commands.UnknownCommand()
            }
        };
    }
}
