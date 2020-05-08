using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using CoviIDApiCore.V1.Configuration;
using CoviIDApiCore.V1.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CoviIDApiCore.V1.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly TokenOptions _tokenOptions;

        private readonly string _key;
        private const string _name = "unique_name";

        public TokenService(IConfiguration configuration, IOptions<TokenOptions> tokenOptions)
        {
            _configuration = configuration;
            _tokenOptions = tokenOptions.Value;

            _key = _configuration.GetValue<string>("ServerKey");
        }

        public string GenerateToken(string sessionId, int validFor)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, sessionId)
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(validFor),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_key)), SecurityAlgorithms.HmacSha256),
                IssuedAt = DateTime.UtcNow
            };

            var tokens = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(tokens);
        }

        public string GetSessionIdFromToken(string authToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var readableToken = handler.CanReadToken(authToken);

            if(readableToken != true)
                throw new AuthenticationException("Token invalid");

            var token = handler.ReadJwtToken(authToken);

            var claim = token.Claims?.FirstOrDefault(t => string.Equals(t.Type,_name))?.Value;

            if(claim == null)
                throw new AuthenticationException("Token invalid");

            return claim;
        }
    }
}