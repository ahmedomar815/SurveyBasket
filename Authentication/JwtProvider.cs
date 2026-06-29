using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SurveyBasket.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace SurveyBasket.Authentication
{
    public class JwtProvider(IOptions<JwtOptions> options) : IJwtProvider
    {
        private readonly JwtOptions _options = options.Value;

        public (string Token, int ExpiresIn) GenerateToken(ApplicationUser user, IEnumerable<string> roles, IEnumerable<string> permissions)
        {
            Claim[] claims = [
                new( JwtRegisteredClaimNames.Sub,user.Id),
                new (JwtRegisteredClaimNames.Email,user.Email!),
                new (JwtRegisteredClaimNames.GivenName,user.FirstName),
                 new (JwtRegisteredClaimNames.FamilyName,user.LastName),
                  new (JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()    ),
                  new (nameof(roles),JsonSerializer.Serialize(roles),JsonClaimValueTypes.JsonArray),
                  new (nameof(permissions),JsonSerializer.Serialize(permissions),JsonClaimValueTypes.JsonArray)
                ];
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
            var singingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            
            var Token = new JwtSecurityToken(issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,expires: DateTime.UtcNow.AddMinutes(_options.ExpiryMinutes)
                   ,
                signingCredentials: singingCredentials);
            return (Token: new JwtSecurityTokenHandler().WriteToken(Token), ExpiresIn: _options.ExpiryMinutes);
        }

        public string? ValidateToken(string Token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var systemmetricSecurityKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
            try
            {
                tokenHandler.ValidateToken(Token, new TokenValidationParameters
                {
                    IssuerSigningKey = systemmetricSecurityKey,
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = true,
                    ValidateAudience =false,
                    ClockSkew=TimeSpan.Zero
                },out SecurityToken validatedToken);
                var jwtToken = (JwtSecurityToken)validatedToken;
              return  jwtToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)!.Value;
            }
            catch
            {
                return null;
            }
         }
    }
}
