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
        public (string token, int expiresIn) GenerateToken(ApplicationUser user)
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

            var exporesIn = 30;
            var expirationDate=DateTime.UtcNow.AddMinutes(exporesIn);
            var token = new JwtSecurityToken(issuer: "SurveyBaskedApp",
                audience: "SurveyBasked users",
                claims: claims, expires:
                DateTime.UtcNow.AddMinutes(exporesIn),
                signingCredentials: singingCredentials);
            return (token: new JwtSecurityTokenHandler().WriteToken(token), expiresIn: exporesIn);
        }
    }
}
