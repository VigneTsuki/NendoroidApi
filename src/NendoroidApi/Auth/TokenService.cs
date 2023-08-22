using NendoroidApi.Data.Model;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace NendoroidApi.Auth
{
    public class TokenService : ITokenService
    {
        public string CreateToken(Usuario usuario)
        {
            List<Claim> claims = new List<Claim> {
                new Claim(ClaimTypes.Name, usuario.Nome)
            };

            foreach (var role in usuario.UsuarioRoles)
                claims.Add(new Claim(ClaimTypes.Role, role.Nome ));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Settings.Secret));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                    issuer: Settings.Issuer,
                    audience: Settings.Audience,
                    claims: claims,
                    expires: DateTime.Now.AddHours(2),
                    signingCredentials: creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}
