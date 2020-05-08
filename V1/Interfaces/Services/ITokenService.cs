namespace CoviIDApiCore.V1.Interfaces.Services
{
    public interface ITokenService
    {
        string GenerateToken(string sessionId, int validFor);
        string GetSessionIdFromToken(string token);
    }
}