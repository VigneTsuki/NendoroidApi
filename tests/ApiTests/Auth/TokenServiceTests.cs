using NendoroidApi.Auth;
using NendoroidApi.Data.Model;
using Xunit;

namespace ApiTests.Auth
{
    public class TokenServiceTests
    {
        [Fact]
        public void DeveGerarToken()
        {
            var usuario = new Usuario
            {
                Id = 1,
                Nome = "Gabriel",
                UsuarioRoles =
                {
                    new Roles
                    {
                        Nome = "Admin"
                    }
                }
            };

            var resultado = new TokenService().CreateToken(usuario);

            Assert.NotNull(resultado);
        }
    }
}
