using CoviIDApiCore.Models.AppSettings;
using CoviIDApiCore.Models.Database;
using CoviIDApiCore.V1.DTOs.Session;
using CoviIDApiCore.V1.DTOs.Wallet;
using CoviIDApiCore.V1.Interfaces.Repositories;
using CoviIDApiCore.V1.Interfaces.Services;
using System;
using System.Threading.Tasks;

namespace CoviIDApiCore.V1.Services
{
    public class SessionService : ISessionService
    {
        private readonly IWalletService _walletService;
        private readonly ISessionRepository _sessionRepository;
        private readonly SessionSettings _sessionSettings;
        public SessionService(IWalletService walletService, ISessionRepository sessionRepository, SessionSettings sessionSettings)
        {
            _walletService = walletService;
            _sessionRepository = sessionRepository;
            _sessionSettings = sessionSettings;
        }

        public async Task<Session> CreateSession(CreateSessionRequest createSessionRequest)
        {
            var walletRequest = new CreateWalletRequest
            {
                MobileNumber = createSessionRequest.MobileNumber,
                MobileNumberReference = createSessionRequest.MobileNumberReference
            };
            var wallet = await _walletService.AddWalletToDatabase(walletRequest);

            var session = new Session
            {
                ExpireAt = DateTime.UtcNow.AddMinutes(_sessionSettings.ExpireInMinutes),
                Wallet = wallet
            };
            await _sessionRepository.AddAsync(session);
            await _sessionRepository.SaveAsync();

            return session;
        }
    }
}
