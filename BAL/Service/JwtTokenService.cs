using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceMgmt.BAL.Service
{
    public class JwtTokenService
    {
        private readonly string _jwtKey;
        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;
        private readonly int _expirationInMinutes;

        public JwtTokenService(IConfiguration configuration)
        {
            _jwtKey = configuration["JwtSettings:Key"];
            _jwtIssuer = configuration["JwtSettings:Issuer"];
            _jwtAudience = configuration["JwtSettings:Audience"];
            _expirationInMinutes = configuration.GetValue<int>("JwtSettings:ExpirationInMinutes");
        }

        public string GenerateJwtToken(string username, List<Claim> claims)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var expirationTime = DateTime.Now.AddMinutes(_expirationInMinutes);

            var token = new JwtSecurityToken(
                _jwtIssuer,
                _jwtAudience,
                claims,
                expires: expirationTime,
                signingCredentials: credentials
            );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenString;
        }
    }

}
