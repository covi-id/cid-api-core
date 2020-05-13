using CoviIDApiCore.V1.Attributes;

namespace CoviIDApiCore.V1.DTOs.Wallet
{
    public class WalletContract
    {
        // Proof of concept for the attribute. This file will be deleted/rewritten anyway.
        [Encrypted(true)]
        public string WalletId { get; set; }
        [Encrypted]
        public string Name { get; set; }    
    }
}
