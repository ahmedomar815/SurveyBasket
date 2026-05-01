using Microsoft.IdentityModel.Tokens;
using SurveyBasket.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SurveyBasket.Authentication
{
    public class JwtProvider : IJwtProvider
    {
        public (string Token, int ExpiresIn) GenerateToken(ApplicationUser user)
        {
            Claim[] claims = [
                new( JwtRegisteredClaimNames.Sub,user.Id),
                new (JwtRegisteredClaimNames.Email,user.Email!),
                new (JwtRegisteredClaimNames.GivenName,user.FirstName),
                 new (JwtRegisteredClaimNames.FamilyName,user.LastName),
                  new (JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()    )
                ];
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("FMJfAlChvFKJdyOIvaNAlnWR"));
            var singingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var ExporesIn = 30;
            var Token = new JwtSecurityToken(issuer: "SurveyBasketApp",
                audience: "SurveyBasket users",
                claims: claims, expires:
                DateTime.UtcNow.AddMinutes(ExporesIn),
                signingCredentials: singingCredentials);
            return (Token: new JwtSecurityTokenHandler().WriteToken(Token), ExpiresIn: ExporesIn);
        }
    }
}
