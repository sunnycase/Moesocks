﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Moesocks.Security
{
    public class SecurePrefix
    {
        private readonly byte[] _randomReadBytes;
        private readonly byte[] _randomReadUint32;
        private readonly byte[] _randomWriteBytes;
        private readonly byte[] _randomWriteUint32;
        private readonly RandomNumberGenerator _randomGenerator;
        private readonly ILogger _logger;

        public SecurePrefix(uint maxRandomBytesLength, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<SecurePrefix>();
            _randomReadBytes = new byte[maxRandomBytesLength];
            _randomReadUint32 = new byte[sizeof(uint)];
            _randomWriteBytes = new byte[maxRandomBytesLength];
            _randomWriteUint32 = new byte[sizeof(uint)];
            _randomGenerator = RandomNumberGenerator.Create();
        }

        public async Task WriteAsync(Stream stream)
        {
            _randomGenerator.GetBytes(_randomWriteUint32);
            var randomPrefix = BitConverter.ToUInt32(_randomWriteUint32, 0);
            var randomLength = (int)(randomPrefix % _randomWriteBytes.Length);
            _randomGenerator.GetBytes(_randomWriteBytes);

            await stream.WriteAsync(_randomWriteUint32, 0, _randomWriteUint32.Length);
            await stream.WriteAsync(_randomWriteBytes, 0, randomLength);
            _logger.LogDebug($"Send {randomLength} bytes random data.");
        }

        public async Task ReadAsync(Stream stream)
        {
            await stream.ReadAsync(_randomReadUint32, 0, _randomReadUint32.Length);
            var randomPrefix = BitConverter.ToUInt32(_randomReadUint32, 0);
            var randomLength = (int)(randomPrefix % _randomReadBytes.Length);

            var len = await stream.ReadAsync(_randomReadBytes, 0, randomLength);
            _logger.LogDebug($"Receive {len} bytes random data.");
        }
    }
}
