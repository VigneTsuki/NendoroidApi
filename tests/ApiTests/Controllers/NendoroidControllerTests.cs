using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NendoroidApi.Controllers;
using NendoroidApi.Data.Interface;
using NendoroidApi.Data.Model;
using NendoroidApi.Request;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ApiTests.Controllers
{
    public class NendoroidControllerTests
    {
        private Mock<INendoroidRepository> mockNendoroidRepository;
        private Mock<ISerieRepository> mockSerieRepository;
        private NendoroidController nendoroidController;
        public NendoroidControllerTests()
        {
            mockNendoroidRepository = new Mock<INendoroidRepository>();
            mockSerieRepository = new Mock<ISerieRepository>();
            nendoroidController = new NendoroidController(mockNendoroidRepository.Object, mockSerieRepository.Object);
        }

        [Fact]
        public async Task DeveCadastrarNendoroid()
        {
            mockNendoroidRepository.Setup(n => n.NendoroidExiste(It.IsAny<string>()))
                .ReturnsAsync(false);

            var request = new CadastroNendoroidRequest
            {
                Numero = "1100",
                Nome = "Magamihara Nadeshiko",
                DataLancamento = DateTime.Now.ToString("yyyy-MM"),
                Cooperacao = "Nendoron",
                PrecoJpy = 6000,
                Escultor = "Nendoron",
                IdSerie = 1
            };

            var resultado = await nendoroidController.Post(request);

            Assert.Equal(StatusCodes.Status201Created, (resultado.Result as ObjectResult)?.StatusCode);
        }

        [Fact]
        public async Task DeveRetornarBadRequestPoisRequestNula()
        {
            var resultado = await nendoroidController.Post(It.IsAny<CadastroNendoroidRequest>());

            Assert.Equal(StatusCodes.Status400BadRequest, (resultado.Result as ObjectResult)?.StatusCode);
        }

        [Fact]
        public async Task DeveRetornarBadRequestPoisRequestInvalida()
        {
            var request = new CadastroNendoroidRequest
            {
                Numero = ""
            };

            var resultado = await nendoroidController.Post(request);

            Assert.Equal(StatusCodes.Status400BadRequest, (resultado.Result as ObjectResult)?.StatusCode);
        }

        [Fact]
        public async Task DeveRetornarBadRequestPoisNendoroidJaCadastrada()
        {
            mockNendoroidRepository.Setup(n => n.NendoroidExiste(It.IsAny<string>()))
                .ReturnsAsync(true);

            var request = new CadastroNendoroidRequest
            {
                Numero = "1100",
                Nome = "Magamihara Nadeshiko",
                DataLancamento = DateTime.Now.ToString("yyyy-MM"),
                Cooperacao = "Nendoron",
                PrecoJpy = 6000,
                Escultor = "Nendoron",
                IdSerie = 1
            };

            var resultado = await nendoroidController.Post(request);

            Assert.Equal(StatusCodes.Status400BadRequest, (resultado.Result as ObjectResult)?.StatusCode);
        }

        [Fact]
        public async Task DeveDeletarNendoroid()
        {
            mockNendoroidRepository.Setup(n => n.NendoroidExiste(It.IsAny<string>()))
                .ReturnsAsync(true);

            var resultado = await nendoroidController.Delete("1");

            Assert.Equal(StatusCodes.Status200OK, (resultado.Result as ObjectResult)?.StatusCode);
        }

        [Fact]
        public async Task DeveRetornarBadRequestAoDeletarPoisRequestInvalida()
        {
            var resultado = await nendoroidController.Delete(It.IsAny<string>());

            Assert.Equal(StatusCodes.Status400BadRequest, (resultado.Result as ObjectResult)?.StatusCode);
        }

        [Fact]
        public async Task DeveRetornarBadRequestAoDeletarPoisNendoroidNaoCadastrada()
        {
            mockNendoroidRepository.Setup(n => n.NendoroidExiste(It.IsAny<string>()))
                .ReturnsAsync(false);

            var resultado = await nendoroidController.Delete("1");

            Assert.Equal(StatusCodes.Status400BadRequest, (resultado.Result as ObjectResult)?.StatusCode);
        }

        [Fact]
        public async Task DeveBuscarNendoroids()
        {
            mockNendoroidRepository.Setup(n => n.BuscarNendoroidsPaginado(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new List<Nendoroid> { new() { Id = 1 } });

            mockNendoroidRepository.Setup(n => n.TotalNendorodoids())
                .ReturnsAsync(1);

            mockSerieRepository.Setup(s => s.BuscarTituloPorIdSerie(It.IsAny<int>()))
                .ReturnsAsync("Yuru Camp");

            var resultado = await nendoroidController.Get(1, 1);

            Assert.Equal(StatusCodes.Status200OK, (resultado.Result as ObjectResult)?.StatusCode);
        }

        [Fact]
        public async Task DeveRetornarBadRequestPoisTamanhoPaginaMaiorQue10()
        {
            var resultado = await nendoroidController.Get(1, 11);

            Assert.Equal(StatusCodes.Status400BadRequest, (resultado.Result as ObjectResult)?.StatusCode);
        }

        [Fact]
        public async Task DeveBuscarNendoroidPorId()
        {
            mockNendoroidRepository.Setup(n => n.BuscarNendoroidPorId(It.IsAny<int>()))
                .ReturnsAsync(new Nendoroid { Id = 1 });

            mockSerieRepository.Setup(s => s.BuscarTituloPorIdSerie(It.IsAny<int>()))
                .ReturnsAsync("Yuru Camp");

            var resultado = await nendoroidController.BuscarPorId(1);

            Assert.Equal(StatusCodes.Status200OK, (resultado.Result as ObjectResult)?.StatusCode);
        }

        [Fact]
        public async Task DeveRetornarBadRequestPoisIdNendoroidNaoEnviado()
        {
            var resultado = await nendoroidController.BuscarPorId(It.IsAny<int>());

            Assert.Equal(StatusCodes.Status400BadRequest, (resultado.Result as ObjectResult)?.StatusCode);
        }

        [Fact]
        public async Task DeveRetornarBadRequestPoisIdNendoroidNaoEncontrado()
        {
            mockNendoroidRepository.Setup(n => n.BuscarNendoroidPorId(It.IsAny<int>()))
                .ReturnsAsync(It.IsAny<Nendoroid>());

            var resultado = await nendoroidController.BuscarPorId(It.IsAny<int>());

            Assert.Equal(StatusCodes.Status400BadRequest, (resultado.Result as ObjectResult)?.StatusCode);
        }

        [Fact]
        public async Task DeveBuscarNendoroidPorNumero()
        {
            mockNendoroidRepository.Setup(n => n.BuscarNendoroidPorNumero(It.IsAny<string>()))
                .ReturnsAsync(new Nendoroid { Id = 1 });

            mockSerieRepository.Setup(s => s.BuscarTituloPorIdSerie(It.IsAny<int>()))
                .ReturnsAsync("Yuru Camp");

            var resultado = await nendoroidController.BuscarPorNumero("Chiaki");

            Assert.Equal(StatusCodes.Status200OK, (resultado.Result as ObjectResult)?.StatusCode);
        }

        [Fact]
        public async Task DeveRetornarBadRequestPoisNumeroNendoroidNaoEnviado()
        {
            var resultado = await nendoroidController.BuscarPorNumero(It.IsAny<string>());

            Assert.Equal(StatusCodes.Status400BadRequest, (resultado.Result as ObjectResult)?.StatusCode);
        }

        [Fact]
        public async Task DeveRetornarBadRequestPoisNumeroNendoroidNaoEncontrado()
        {
            mockNendoroidRepository.Setup(n => n.BuscarNendoroidPorNumero(It.IsAny<string>()))
                .ReturnsAsync(It.IsAny<Nendoroid>());

            var resultado = await nendoroidController.BuscarPorNumero(It.IsAny<string>());

            Assert.Equal(StatusCodes.Status400BadRequest, (resultado.Result as ObjectResult)?.StatusCode);
        }
    }
}
