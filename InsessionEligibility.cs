using Blockchain_UserJourney.Common.Models;

namespace Blockchain_Eligibility.Service.Models
{
    public class InsessionEligibility : UserModel
    {
        public double CreditAmount { get; set; }
        public string OfferId { get; set; }
    }
}
