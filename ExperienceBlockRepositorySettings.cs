namespace Blockchain_Eligibility.Service.Settings
{
    public class ExperienceBlockRepositorySettings : IExperienceBlockRepositorySettings
    {
        public string DataBaseName { get; set; }
        public string CollectionName { get; set; }
        public string ConnectionString { get; set; }
    }
}
