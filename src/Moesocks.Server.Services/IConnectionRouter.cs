﻿using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Moesocks.Server.Services
{
    public class ConnectionRouterSettings : IOptions<ConnectionRouterSettings>
    {
        ConnectionRouterSettings IOptions<ConnectionRouterSettings>.Value => this;

        public string ServerIPAddress { get; set; }
        public int ServerPort { get; set; }
    }

    public class SecuritySettings : IOptions<SecuritySettings>
    {
        SecuritySettings IOptions<SecuritySettings>.Value => this;

        public string ServerCertificateFileName { get; set; }
        public string ServerCertificatePassword { get; set; }
        public ushort MaxRandomBytesLength { get; set; }
    }

    public interface IConnectionRouter
    {
        void Startup();
    }
}
