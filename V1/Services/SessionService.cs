using CoviIDApiCore.Models.AppSettings;
using CoviIDApiCore.Models.Database;
using CoviIDApiCore.V1.Constants;
using CoviIDApiCore.V1.Interfaces.Repositories;
using CoviIDApiCore.V1.Interfaces.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace CoviIDApiCore.V1.Services
{
    public class SessionService : ISessionService
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly SessionSettings _sessionSettings;
        public SessionService(ISessionRepository sessionRepository, SessionSettings sessionSettings)
        {
            _sessionRepository = sessionRepository;
            _sessionSettings = sessionSettings;
        }

        public async Task<Session> CreateSession(string mobileNumber, Wallet wallet)
        {
            var session = new Session
            {
                ExpireAt = DateTime.UtcNow.AddMinutes(_sessionSettings.ExpiresInMinutes),
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

            if (session == default || session == null || session.isUsed || DateTime.UtcNow > session.ExpireAt)
                throw new ValidationException(Messages.Ses_Invalid);

            session.isUsed = true;
            _sessionRepository.Update(session);

            await _sessionRepository.SaveAsync();
            return session;
        }
    }
}
