using CoviIDApiCore.V1.DTOs.SafePlaces;
using CoviIDApiCore.V1.Interfaces.Brokers;
using CoviIDApiCore.V1.Interfaces.Repositories;
using CoviIDApiCore.V1.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoviIDApiCore.V1.Services
{
    public class StaySafeService : IStaySafeService
    {
        private readonly IWalletLocationReceiptRepository _walletLocationReceiptRepository;
        private readonly ISafePlacesBroker _safePlacesBroker;

        public StaySafeService(IWalletLocationReceiptRepository walletLocationReceiptRepository, ISafePlacesBroker safePlacesBroker)
        {
            _walletLocationReceiptRepository = walletLocationReceiptRepository;
            _safePlacesBroker = safePlacesBroker;
        }

        public async Task CaptureData(Guid walletId, DateTime testedAtDate, int totalDays)
        {
            var trails = new List<Trail>();
            var logs = await _walletLocationReceiptRepository.GetReceiptsByStartDate(walletId, testedAtDate.AddDays(totalDays));

            if (logs.Count > 0)
            {
                foreach (var log in logs)
                {
                    if (!(log.Latitude == 0 && log.Longitude == 0))
                    {
                        trails.Add(new Trail
                        {
                            Time = log.CreatedAt.Value.Ticks,
                            Latitude = decimal.ToDouble(log.Latitude),
                            Longitude = decimal.ToDouble(log.Longitude)
                        });
                    }
                }
                if (trails.Count > 0)
                {
                    var request = new RedactedRequest
                    {
                        Identifier = Guid.NewGuid().ToString(),
                        Trails = trails
                    };

                    await _safePlacesBroker.AddRedacted(request);
                }
            }
            return;
        }
    }
}
