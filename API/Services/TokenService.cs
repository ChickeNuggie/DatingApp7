using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class TokenService : ITokenService
    {
        // use same key to decrypt and encrypt data as API server is responsible for both signing and decrypting token key.
        // It stays in server and never go to client as client does not need to decrypt this token key.
        // AsymmetricSecurityKey used when server needs to encrypt something and client needs to decrypt something (requires public key to decrypt data and private key which stays in the server)
        private readonly SymmetricSecurityKey _key; 
        // construct and store secret key in the configuration via injecting iConfig into service.
        public TokenService(IConfiguration config)
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        }
        public string CreateToken(AppUser user)
        {
            var claims = new List<Claim> 
            {   //set claims to user's name.
                //To check if users are who they claimed to be.
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),//set user's name claim in token
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
            };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature); // specify type of algorithm used to encrypt key

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims), // Includes claims that you want to return.
                Expires = DateTime.Now.AddDays(7), // Token expiry date.
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            //write and pass out the token
            return tokenHandler.WriteToken(token); 

            throw new NotImplementedException();
        }
    }
}