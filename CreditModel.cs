namespace Blockchain_Eligibility.Service.Models
{
    public class CreditModel
    {
        public CreditModel(InsessionEligibility insessionEligibility, bool isSuccess)
        {
            IsSuccess = isSuccess;
            Eligibility = insessionEligibility;
        }
        public InsessionEligibility Eligibility { get; set; }
        public bool IsSuccess { get; set; }
    }
}
