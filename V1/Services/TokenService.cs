using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using CoviIDApiCore.V1.Configuration;
using CoviIDApiCore.V1.Constants;
using CoviIDApiCore.V1.DTOs.Authentication;
using CoviIDApiCore.V1.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CoviIDApiCore.V1.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly TokenOptions _tokenOptions;

        private readonly string _key;

        public TokenService(IConfiguration configuration, IOptions<TokenOptions> tokenOptions)
        {
            _configuration = configuration;
            _tokenOptions = tokenOptions.Value;

            _key = _configuration.GetValue<string>("ServerKey");
        }

        public string GenerateToken(string walletId, long otpId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, walletId),
                new Claim(ClaimTypes.Sid, otpId.ToString())
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(30), //TODO: change
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_key)), SecurityAlgorithms.HmacSha256),
                IssuedAt = DateTime.UtcNow
            };

            var tokens = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(tokens);
        }

        public TokenReturn GetDetailsFromToken(string authToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var readableToken = handler.CanReadToken(authToken);

            if(readableToken != true)
                throw new AuthenticationException(Messages.Token_Invalid);

            var token = handler.ReadJwtToken(authToken);

            var walletClaim = token
                .Claims?
                .FirstOrDefault(t => string.Equals(t.Type,DefinitionConstants.IdentityClaimStrings[DefinitionConstants.IdentityClaims.UniqueName]))?
                .Value;

            var otpClaim = token
                .Claims?
                .FirstOrDefault(t => string.Equals(t.Type,DefinitionConstants.IdentityClaimStrings[DefinitionConstants.IdentityClaims.Sid]))?
                .Value;

            if(walletClaim == null)
                throw new AuthenticationException(Messages.Token_Invalid);

            return new TokenReturn()
            {
                WalletId = walletClaim,
                OtpId = Convert.ToInt64(otpClaim)
            };
        }
    }
}