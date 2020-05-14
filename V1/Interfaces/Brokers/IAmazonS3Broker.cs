using System.Threading.Tasks;

namespace CoviIDApiCore.V1.Interfaces.Brokers
{
    public interface IAmazonS3Broker
    {
        Task<string> AddImageToBucket(string file, string fileName);
        string GetImage(string fileName);
    }
}
