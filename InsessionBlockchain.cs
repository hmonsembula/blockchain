using Blockchain_Eligibility.Service.System;
using System.Threading.Tasks;

namespace Blockchain_UserJourney.ActorSystem
{
    public class InsessionBlockchain : IInsessionBlockchain
    {
        private readonly IEligibilitySystem _eligibilitySystem;

        public InsessionBlockchain(IEligibilitySystem eligibilitySystem)
        {
            _eligibilitySystem = eligibilitySystem;
        }

        public void Start()
        {
            _eligibilitySystem.Start();
        }

        public async Task Stop()
        {
            await _eligibilitySystem.Stop();
        }
    }
}
