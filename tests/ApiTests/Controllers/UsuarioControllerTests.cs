using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NendoroidApi.Controllers;
using NendoroidApi.Data.Base;
using NendoroidApi.Data.Interface;
using NendoroidApi.Request;
using System.Threading.Tasks;
using Xunit;

namespace ApiTests.Controllers
{
    public class UsuarioControllerTests : IClassFixture<TestConfig>
    {
        private Mock<IUsuarioRepository> mockUsuarioRepository;
        private Mock<IUnitOfWork> mockUnitOfWork;
        private UsuarioController usuarioController;
        public UsuarioControllerTests()
        {
            mockUsuarioRepository = new Mock<IUsuarioRepository>();
            mockUnitOfWork = new Mock<IUnitOfWork>();
            usuarioController = new UsuarioController(mockUsuarioRepository.Object, mockUnitOfWork.Object);
        }

        [Fact]
        public async Task DeveCadastrarUsuario()
        {
            var request = new CadastroUsuarioRequest
            {
                Nome = "Gabriel",
                Senha = "123",
                ReSenha = "123"
            };

            mockUsuarioRepository.Setup(u => u.UsuarioExiste(It.IsAny<string>()))
                .ReturnsAsync(false);

            var resultado = await usuarioController.Post(request);

            Assert.Equal(StatusCodes.Status201Created, (resultado.Result as ObjectResult)?.StatusCode);
        }

        [Fact]
        public async Task DeveRetornarBadRequestPoisRequestInvalida()
        {
            var request = new CadastroUsuarioRequest
            {
                Nome = ""
            };

            var resultado = await usuarioController.Post(request);

            Assert.Equal(StatusCodes.Status400BadRequest, (resultado.Result as ObjectResult)?.StatusCode);
        }

        [Fact]
        public async Task DeveRetornarBadRequestPoisSenhasDiferentes()
        {
            var request = new CadastroUsuarioRequest
            {
                Nome = "Gabriel",
                Senha = "123",
                ReSenha = "1234"
            };

            var resultado = await usuarioController.Post(request);

            Assert.Equal(StatusCodes.Status400BadRequest, (resultado.Result as ObjectResult)?.StatusCode);
        }

        [Fact]
        public async Task DeveRetornarBadRequestPoisUsuarioJaExiste()
        {
            var request = new CadastroUsuarioRequest
            {
                Nome = "Gabriel",
                Senha = "123",
                ReSenha = "123"
            };

            mockUsuarioRepository.Setup(u => u.UsuarioExiste(It.IsAny<string>()))
                .ReturnsAsync(true);

            var resultado = await usuarioController.Post(request);

            Assert.Equal(StatusCodes.Status400BadRequest, (resultado.Result as ObjectResult)?.StatusCode);
        }

        [Fact]
        public async Task DeveInativarUsuario()
        {
            var resultado = await usuarioController.Inativar(1);

            Assert.Equal(StatusCodes.Status200OK, (resultado.Result as ObjectResult)?.StatusCode);
        }

        [Fact]
        public async Task DeveRetornarBadRequestAoInativarPoisIdNaoInformado()
        {
            var resultado = await usuarioController.Inativar(0);

            Assert.Equal(StatusCodes.Status400BadRequest, (resultado.Result as ObjectResult)?.StatusCode);
        }

        [Fact]
        public async Task DeveAtivarUsuario()
        {
            var resultado = await usuarioController.Ativar(1);

            Assert.Equal(StatusCodes.Status200OK, (resultado.Result as ObjectResult)?.StatusCode);
        }

        [Fact]
        public async Task DeveRetornarBadRequestAoAtivarPoisIdNaoInformado()
        {
            var resultado = await usuarioController.Ativar(0);

            Assert.Equal(StatusCodes.Status400BadRequest, (resultado.Result as ObjectResult)?.StatusCode);
        }
    }
}
