using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;


namespace WebScrapper.JWT
{
    public class JwtAuthenticationManager : IJwtAuthenticationManager
    {
        private readonly IDictionary<string, string> users = new Dictionary<string, string>()
            {{"Cliff", "CliffChecksEverybody"}, {"Mikkel", "MikkelChecksEverybody"} };
    
        private readonly string key;
    
        public JwtAuthenticationManager(String key)
        {
            this.key = key;
        }

        public string Authenticate(string username, string password)
        {
            if (!users.Any(u => u.Key == username && u.Value == password))
            {
                return null;
            }
        
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(key);
            var tokerDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, username)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokerDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}