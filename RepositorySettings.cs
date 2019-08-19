namespace Blockchain_UserJourney.Common.Models
{
    public class RepositorySettings : IRepositorySettings
    {
        public string DataBaseName { get; set; }
        public string CollectionName { get; set; }
        public string ConnectionString { get; set; }
    }
}
