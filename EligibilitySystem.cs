using Akka.Actor;
using Blockchain_Eligibility.Service.Actors;
using Blockchain_Eligibility.Service.Repositories;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Blockchain_Eligibility.Service.System
{
    public class EligibilitySystem : IEligibilitySystem
    {
        private const string SYSTEM_NAME = "daily-deal-system";
        private ActorSystem _eligibilityActorSystem;
        private readonly IEligibilityRepository _eligibilityRepository;
        private readonly IEligibilityBlockRepository _blockChainRepository;
        private readonly IExperienceBlockRepository _experienceBlockRepository;
        private readonly ICreditBlockRepository _creditBlockRepository;
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;
        public EligibilitySystem(ILoggerFactory loggerFactory, IEligibilityRepository eligibilityRepository, IEligibilityBlockRepository eligibilityBlockRepository, IExperienceBlockRepository experienceBlockRepository, ICreditBlockRepository creditBlockRepository)
        {
            _loggerFactory = loggerFactory;
            _logger = new Logger<EligibilitySystem>(loggerFactory);
            _eligibilityRepository = eligibilityRepository;
            _blockChainRepository = eligibilityBlockRepository;
            _experienceBlockRepository = experienceBlockRepository;
            _creditBlockRepository = creditBlockRepository;
        }

        public void Start()
        {
            _eligibilityActorSystem = ActorSystem.Create(SYSTEM_NAME);

            var block = _eligibilityActorSystem.ActorOf(Props.Create(() => new InsessionBlockChainActor(_blockChainRepository, _experienceBlockRepository, _creditBlockRepository)));
            _eligibilityActorSystem.ActorOf(Props.Create(() => new EligibilityActor(block,_loggerFactory, _eligibilityRepository, _blockChainRepository, _experienceBlockRepository, _creditBlockRepository)));

            _logger.LogInformation($"{SYSTEM_NAME.ToUpper()} started...");
        }

        public async Task Stop()
        {
            await _eligibilityActorSystem.Terminate();
        }
    }
}
