using CoviIDApiCore.Models.AppSettings;
using CoviIDApiCore.Models.Database;
using CoviIDApiCore.V1.Constants;
using CoviIDApiCore.V1.Interfaces.Repositories;
using CoviIDApiCore.V1.Interfaces.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace CoviIDApiCore.V1.Services
{
    public class SessionService : ISessionService
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly SessionSettings _sessionSettings;
        private readonly IConfiguration _configuration;

        public SessionService(ISessionRepository sessionRepository, SessionSettings sessionSettings, IConfiguration configuration)
        {
            _sessionRepository = sessionRepository;
            _sessionSettings = sessionSettings;
            _configuration = configuration;
        }

        public async Task<Session> CreateSession(string mobileNumber, Wallet wallet)
        {
            var session = new Session
            {
                ExpireAt = DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("SessionSettings:ExpiryInMinutes")),
                CreatedAt = DateTime.UtcNow,
                Wallet = wallet
            };
            await _sessionRepository.AddAsync(session);
            await _sessionRepository.SaveAsync();

            return session;
        }

        public async Task<Session> GetAndUseSession(string sessionId)
        {
            var session = await _sessionRepository.GetSessionAndWalletAsync(Guid.Parse(sessionId));

            if (session == null || session.isUsed || DateTime.UtcNow > session.ExpireAt)
                throw new ValidationException(Messages.Ses_Invalid);

            session.isUsed = true;

            _sessionRepository.Update(session);

            await _sessionRepository.SaveAsync();
            return session;
        }
    }
}
