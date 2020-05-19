using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using CoviIDApiCore.Exceptions;
using CoviIDApiCore.Models.AppSettings;
using CoviIDApiCore.V1.Constants;
using CoviIDApiCore.V1.DTOs.Authentication;
using CoviIDApiCore.V1.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CoviIDApiCore.V1.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly string _key;
        private readonly TokenSettings _tokenOptions;

        public TokenService(IConfiguration configuration, TokenSettings tokenSettings)
        {
            _configuration = configuration;
            _tokenOptions = tokenSettings;
            _key = _configuration.GetValue<string>("ServerKey");
        }

        public string GenerateToken(string walletId, long otpId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, walletId),
                new Claim(ClaimTypes.Sid, otpId.ToString())
            };

            return CreateToken(claims);
        }

        public string CreateToken(List<Claim> claims)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_tokenOptions.ExpiresInMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_key)), SecurityAlgorithms.HmacSha256),
                IssuedAt = DateTime.UtcNow
            };

            var tokens = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(tokens);
        }

        public List<TokenReturn> GetDetailsFromToken(string authToken)
        {
            var response = new List<TokenReturn>();
            var handler = new JwtSecurityTokenHandler();
            var readableToken = handler.CanReadToken(authToken);

            if (readableToken != true)
                throw new AuthenticationException(Messages.Token_Invalid);

            var token = handler.ReadJwtToken(authToken);

            var walletClaims = token
                .Claims?
                .Where(t => string.Equals(t.Type, DefinitionConstants.IdentityClaimStrings[DefinitionConstants.IdentityClaims.UniqueName])).ToList();

            var otpClaims = token
                .Claims?
                .Where(t => string.Equals(t.Type, DefinitionConstants.IdentityClaimStrings[DefinitionConstants.IdentityClaims.Sid])).ToList();

            if (walletClaims == null || otpClaims == null)
                throw new AuthenticationException(Messages.Token_Invalid);

            var claims = walletClaims.Zip(otpClaims, (w, o) => new { walletClaims = w, otpClaims = o});

            foreach (var claim in claims)
            {
                response.Add(new TokenReturn
                {
                    WalletId = claim.walletClaims.Value,
                    OtpId = Convert.ToInt64(claim.otpClaims.Value)
                });
            }

            if (response == null)
                throw new ValidationException(Messages.Token_Invalid);

            return response; 
        }
    }
}