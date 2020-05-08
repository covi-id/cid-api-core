using CoviIDApiCore.V1.DTOs.Authentication;

namespace CoviIDApiCore.V1.Interfaces.Services
{
    public interface ITokenService
    {
        string GenerateToken(string walletId, long otpId);
        TokenReturn GetDetailsFromToken(string authToken);
    }
}