using Blockchain_UserJourney.ActorSystem;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;

namespace Blockchain_UserJourney
{
    public class Startup : IDisposable
    {
        private IInsessionBlockchain _blockChain;
        public void Configure(IInsessionBlockchain blockchain, ILoggerFactory logFactory)
        {
            logFactory.AddNLog();
            _blockChain = blockchain;
            _blockChain.Start();
        }

        public void Dispose()
        {
           _blockChain.Stop().Wait();
        }
    }
}
