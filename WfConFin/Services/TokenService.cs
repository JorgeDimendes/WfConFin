using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WfConFin.Models;

namespace WfConFin.Services
{
    public class TokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GerarToken(Usuario usuario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            //var chave = Encoding.ASCII.GetBytes(_configuration.GetSection("Chave").Get<string>());
            var chave = Encoding.ASCII.GetBytes(_configuration["Chave"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                            new Claim[]
                            {
                                new Claim(ClaimTypes.Name, usuario.Login.ToString()),
                                new Claim(ClaimTypes.Role, usuario.Funcao.ToString()),

                            }
                ),
                Expires = DateTime.UtcNow.AddHours(8), //Quando o Token vai expirar, foi colocado 2h
                SigningCredentials = new SigningCredentials( 
                                    new SymmetricSecurityKey(chave), SecurityAlgorithms.HmacSha256Signature
                )
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
