using CoviIDApiCore.V1.DTOs.SafePlaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoviIDApiCore.V1.Interfaces.Brokers
{
    public interface ISafePlacesBroker
    {
        Task<Redacted> AddRedacted(RedactedRequest request);
        Task<List<Redacted>> GetRedacted();
    }
}
