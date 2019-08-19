namespace Blockchain_UserJourney.Common.Models
{
    public interface IRepositorySettings
    {
        string DataBaseName { get; set; }
        string CollectionName { get; set; }
        string ConnectionString { get; set; }
    }
}
