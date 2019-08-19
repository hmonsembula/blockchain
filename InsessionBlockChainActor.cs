using Akka.Actor;
using Blockchain_Eligibility.Service.Models;
using Blockchain_Eligibility.Service.Repositories;
namespace Blockchain_Eligibility.Service.Actors
{
    public class InsessionBlockChainActor : ReceiveActor
    {
        private readonly IEligibilityBlockRepository _eligibilityRepository;
        private readonly IExperienceBlockRepository _experienceBlockRepository;
        private readonly ICreditBlockRepository _creditBlockRepository;

        public InsessionBlockChainActor(IEligibilityBlockRepository eligibilityBlockRepository, IExperienceBlockRepository experienceBlockRepository, ICreditBlockRepository creditBlockRepository)
        {
            _eligibilityRepository = eligibilityBlockRepository;
            _experienceBlockRepository = experienceBlockRepository;
            _creditBlockRepository = creditBlockRepository;
            Receive<TransactionMessage.Insession>(_ => HandleTransactionMessage(_));
        }

        private void HandleTransactionMessage(TransactionMessage.Insession transactionMessage)
        {
            _eligibilityRepository.Save(transactionMessage.Transction);
            _experienceBlockRepository.Save(transactionMessage.Transction);
            _creditBlockRepository.Save(transactionMessage.Transction);
        }
    }
}
