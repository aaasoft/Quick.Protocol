using Newtonsoft.Json.Linq;
using Quick.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace QpTestClient
{
    public class ConnectionInfo
    {
        public string QpClientTypeName { get; set; }
        public string QpClientOptionsTypeName { get; set; }
        public JObject QpClientOptions { get; set; }

        public QpClient CreateQpClient()
        {
            return null;
        }
    }
}
