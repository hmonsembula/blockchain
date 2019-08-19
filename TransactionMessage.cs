using Blockchain_UserJourney.Common.Core.Models;

namespace Blockchain_Eligibility.Service.Models
{
    public class TransactionMessage
    {
        public class Insession
        {
            public IBlockChain Transction { get; }
            public Insession(IBlockChain transaction)
            {
                Transction = transaction;
            }
        }

        public class Acquisition
        {

        }

        public class Funplay
        {

        }

        public class Tournament
        {

        }

        public class Other
        {

        }
    }
}
