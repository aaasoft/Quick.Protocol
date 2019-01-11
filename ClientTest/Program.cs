using System;

namespace ClientTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var password = "*&^*(HFDU(*Y";
            var a = Quick.Protocol.Utils.CryptographyUtils.DesEncrypt("Hello QuickProtocol.", password);
            var b = Quick.Protocol.Utils.CryptographyUtils.DesDecrypt(a, password);
            Console.WriteLine(b);
            Console.ReadLine();
        }
    }
}
