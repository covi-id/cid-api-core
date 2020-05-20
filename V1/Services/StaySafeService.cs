using CoviIDApiCore.Exceptions;
using CoviIDApiCore.V1.Constants;
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
        private readonly IOrganisationAccessLogRepository _organisationAccessLogRepository;
        private readonly ISafePlacesBroker _safePlacesBroker;

        public StaySafeService(IOrganisationAccessLogRepository organisationAccessLogRepository, ISafePlacesBroker safePlacesBroker)
        {
            _organisationAccessLogRepository = organisationAccessLogRepository;
            _safePlacesBroker = safePlacesBroker;
        }

        public async Task CaptureData(Guid walletId, DateTime testedAt)
        {
            var trails = new List<Trail>();
            var logs = await _organisationAccessLogRepository.GetLogsForLastTwoWeeks(walletId, testedAt);

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

                var request = new RedactedRequest
                {
                    Identifier = Guid.NewGuid().ToString(),
                    Trails = trails
                };

                await _safePlacesBroker.AddRedacted(request);
            }
            return;
        }
    }
}
