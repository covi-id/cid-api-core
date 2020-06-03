using CoviIDApiCore.Exceptions;
using CoviIDApiCore.V1.Constants;
using CoviIDApiCore.V1.DTOs.SafePlaces;
using CoviIDApiCore.V1.Interfaces.Brokers;
using CoviIDApiCore.V1.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoviIDApiCore.V1.Services
{
    public class StaySafeService : IStaySafeService
    {
        private readonly IWalletLocationReceiptService _walletLocationReceiptService;
        private readonly ISafePlacesBroker _safePlacesBroker;

        public StaySafeService(IWalletLocationReceiptService walletLocationReceiptService, ISafePlacesBroker safePlacesBroker)
        {
            _walletLocationReceiptService = walletLocationReceiptService;
            _safePlacesBroker = safePlacesBroker;
        }

        public async Task CaptureData(Guid walletId, DateTime testedAtDate)
        {
            var trails = new List<Trail>();
            var logs = await _walletLocationReceiptService.GetReceiptsByStartDate(walletId, testedAtDate.AddDays(-14));

            if (logs == null)
                throw new NotFoundException(Messages.Oal_NotFound);

            if (logs.Count > 0)
            {
                foreach (var log in logs)
                {
                    if (!(log.Latitude == 0 && log.Longitude == 0))
                    {
                        trails.Add(new Trail
                        {
                            Time = log.CreatedAt.ToString(),
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
