﻿using Quick.Protocol.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quick.Protocol.Pipeline
{
    public class QpPipelineClientOptions : QpClientOptions
    {
        public string ServerName { get; set; } = ".";
        public string PipeName { get; set; }

        public override void Check()
        {
            base.Check();
            if (string.IsNullOrEmpty(PipeName))
                throw new ArgumentNullException(nameof(PipeName));
        }
    }
}
