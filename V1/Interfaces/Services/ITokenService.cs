using CoviIDApiCore.V1.DTOs.Authentication;
using System.Collections.Generic;
using System.Security.Claims;

namespace CoviIDApiCore.V1.Interfaces.Services
{
    public interface ITokenService
    {
        string GenerateToken(string walletId, long otpId);
        List<TokenReturn> GetDetailsFromToken(string authToken);
        string CreateToken(List<Claim> claims);
    }
}