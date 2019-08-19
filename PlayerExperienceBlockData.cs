namespace Blockchain_Eligibility.Service.Models
{
    public class PlayerExperienceBlockData
    {
        public PlayerExperienceBlockData(ExperienceResult experienceResult, InsessionEligibility eligibility)
        {
            ExperienceResult = experienceResult;
            InsessionEligibility = eligibility;
        }
        public ExperienceResult ExperienceResult { get; set; }
        public InsessionEligibility InsessionEligibility { get; set; }
    }
}
