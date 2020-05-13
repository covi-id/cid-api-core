namespace CoviIDApiCore.V1.DTOs.Authentication
{
    public class TokenResponse
    {
        public string Token { get; set; }
    }

    public class TokenReturn
    {
        public string WalletId { get; set; }
        public long OtpId { get; set; }
    }
}
