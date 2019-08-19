namespace Blockchain_Eligibility.Service.Settings
{
    public class CreditBlockRepositorySettings : ICreditBlockRepositorySettings
    {
        public string DataBaseName { get; set; }
        public string CollectionName { get; set; }
        public string ConnectionString { get; set; }
    }
}
