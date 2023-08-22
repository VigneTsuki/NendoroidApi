﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NendoroidApi.Auth;
using NendoroidApi.Controllers;
using NendoroidApi.Data.Interface;
using NendoroidApi.Data.Model;
using NendoroidApi.Request;
using System.Threading.Tasks;
using Xunit;

namespace ApiTests.Controllers
{
    public class AutenticacaoControllerTests : IClassFixture<TestConfig>
    {
        [Fact]
        public async Task DeveAutenticarUsuario()
        {
            var usuarioRepositoryMock = new Mock<IUsuarioRepository>();
            var tokenServiceMock = new Mock<ITokenService>();

            usuarioRepositoryMock.Setup(u => u.BuscarUsuarioComRolesPorNome(It.IsAny<string>()))
                .ReturnsAsync(new Usuario { Id = 1, Nome = "Gabriel", SaltSenha = "123", 
                    HashSenha = "CnzBhES4WP7fGKudkvfcurSH3vxf8zelUaDLv3ZFgPA=" });

            var request = new LoginRequest
            {
                Nome = "Gabriel",
                Senha = "Teste"
            };

            var autenticacaoController = new AutenticacaoController(usuarioRepositoryMock.Object, tokenServiceMock.Object);

            var resultado = await autenticacaoController.Post(request);

            Assert.Equal(StatusCodes.Status200OK, (resultado.Result as ObjectResult)?.StatusCode);
        }

        [Fact]
        public async Task DeveRetornarBadRequestPoisRequestInvalida()
        {
            var usuarioRepositoryMock = new Mock<IUsuarioRepository>();
            var tokenServiceMock = new Mock<ITokenService>();

            var request = new LoginRequest
            {
                Nome = "",
                Senha = "Teste"
            };

            var autenticacaoController = new AutenticacaoController(usuarioRepositoryMock.Object, tokenServiceMock.Object);

            var resultado = await autenticacaoController.Post(request);

            Assert.Equal(StatusCodes.Status400BadRequest, (resultado.Result as ObjectResult)?.StatusCode);
        }

        [Fact]
        public async Task DeveRetornarBadRequestPoisUsuarioNaoExiste()
        {
            var usuarioRepositoryMock = new Mock<IUsuarioRepository>();
            var tokenServiceMock = new Mock<ITokenService>();

            var request = new LoginRequest
            {
                Nome = "Gabriel",
                Senha = "Teste"
            };

            usuarioRepositoryMock.Setup(u => u.BuscarUsuarioComRolesPorNome(It.IsAny<string>()))
                .ReturnsAsync((Usuario?)null);

            var autenticacaoController = new AutenticacaoController(usuarioRepositoryMock.Object, tokenServiceMock.Object);

            var resultado = await autenticacaoController.Post(request);

            Assert.Equal(StatusCodes.Status400BadRequest, (resultado.Result as ObjectResult)?.StatusCode);
        }

        [Fact]
        public async Task DeveRetornarBadRequestPoisSenhaIncorreta()
        {
            var usuarioRepositoryMock = new Mock<IUsuarioRepository>();
            var tokenServiceMock = new Mock<ITokenService>();

            usuarioRepositoryMock.Setup(u => u.BuscarUsuarioComRolesPorNome(It.IsAny<string>()))
                .ReturnsAsync(new Usuario
                {
                    Id = 1,
                    Nome = "Gabriel",
                    SaltSenha = "123"
                });

            var request = new LoginRequest
            {
                Nome = "Gabriel",
                Senha = "Teste"
            };

            var autenticacaoController = new AutenticacaoController(usuarioRepositoryMock.Object, tokenServiceMock.Object);

            var resultado = await autenticacaoController.Post(request);

            Assert.Equal(StatusCodes.Status400BadRequest, (resultado.Result as ObjectResult)?.StatusCode);
        }
    }
}
