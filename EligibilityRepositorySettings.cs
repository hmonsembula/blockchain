namespace Blockchain_Eligibility.Service.Settings
{
    public class EligibilityRepositorySettings : IEligibilityRepositorySettings
    {
        public string DataBaseName { get; set; }
        public string CollectionName { get; set; }
        public string ConnectionString { get; set; }
    }
}
