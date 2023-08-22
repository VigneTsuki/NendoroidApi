using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NendoroidApi.Data.Model;
using NendoroidApi.Data.Repository;
using NendoroidApi.Request;
using NendoroidApi.Response;
using NendoroidApi.Response.Base;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace NendoroidApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NendoroidController : ControllerBase
    {
        private readonly NendoroidRepository _nendoroidRepository;
        private readonly SerieRepository _serieRepository;

        public NendoroidController(NendoroidRepository nendoroidRepository, SerieRepository serieRepository)
        {
            _nendoroidRepository = nendoroidRepository;
            _serieRepository = serieRepository;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<ResponseBase>> Post(CadastroNendoroidRequest request)
        {
            if(request == null)
                return BadRequest(new ResponseBase(false, "Há algo errado com a requisição enviada. Por favor, faça os ajustes e tente novamente."));
            
            var validacaoRequest = request.ValidarRequest();
            if (!validacaoRequest.IsValid)
                return BadRequest(new ResponseBase(false, validacaoRequest.Errors.FirstOrDefault()?.ErrorMessage));

            DateTime? data = null;

            if(request.DataLancamento != null)
                data = DateTime.ParseExact(request.DataLancamento!, "yyyy-MM", CultureInfo.InvariantCulture);

            if(await _nendoroidRepository.NendoroidExiste(request.Numero))
                return BadRequest(new ResponseBase(false, "Nendoroid já está cadastrada."));

            var nendoroid = new Nendoroid
            {
                Nome = request.Nome,
                Numero = request.Numero,
                PrecoJpy = request.PrecoJpy,
                DataLancamento = data ?? null,
                Escultor = request.Escultor,
                Cooperacao = request.Cooperacao,
                IdSerie = request.IdSerie!,
            };

            await _nendoroidRepository.CadastrarNendoroid(nendoroid);

            return Created(string.Empty, new ResponseBase(true, "Nendoroid cadastrada com sucesso"));
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseBase>> Delete([FromQuery] string? numero = null)
        {
            if (numero == null)
                return BadRequest(new ResponseBase(false, "O campo Numero é obrigatório."));

            if (!await _nendoroidRepository.NendoroidExiste(numero))
                return BadRequest(new ResponseBase(false, "Nendoroid não está cadastrada."));

            await _nendoroidRepository.Deletar(numero);

            return Ok(new ResponseBase(true, "Nendoroid deletada com sucesso"));
        }

        [HttpGet]
        [Authorize(Roles = "Admin, User")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<BuscaNendoroidResponse>> Get([FromQuery] int numeroPagina = 1, int tamanhoPagina = 10)
        {
            if(tamanhoPagina > 10)
                return BadRequest(new ResponseBase(false, "O campo TamanhoPagina não pode ser maior que 10."));

            var nendoroids = await _nendoroidRepository.BuscarNendoroidsPaginado(numeroPagina, tamanhoPagina);
            var totalNendoroids = await _nendoroidRepository.TotalNendorodoids();

            var resposta = new BuscaNendoroidResponse(numeroPagina, tamanhoPagina, totalNendoroids);

            foreach (var nendo in nendoroids)
            {
                var tituloSerie = await _serieRepository.BuscarTituloPorIdSerie(nendo.IdSerie);

                var nendoroid = new NendoroidResponse(nendo.Id, nendo.Nome, nendo.Numero, nendo.PrecoJpy,
                    nendo.DataLancamento, nendo.Escultor, nendo.Cooperacao, nendo.IdSerie, tituloSerie);

                resposta.Nendoroids.Add(nendoroid);
            }

            return Ok(new ResponseBase(true, null, resposta));
        }

        [HttpGet("BuscarPorId")]
        [Authorize(Roles = "Admin, User")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<NendoroidResponse>> BuscarPorId([FromQuery] int id = 0)
        {
            if (id == 0)
                return BadRequest(new ResponseBase(false, "O campo Id é obrigatório."));

            var nendoroid = await _nendoroidRepository.BuscarNendoroidPorId(id);

            if(nendoroid == null)
                return NotFound(new ResponseBase(false, "Nendoroid não encontrada."));

            var tituloSerie = await _serieRepository.BuscarTituloPorIdSerie(nendoroid.IdSerie);

            var resposta = new NendoroidResponse(nendoroid.Id, nendoroid.Nome, nendoroid.Numero, nendoroid.PrecoJpy,
                nendoroid.DataLancamento, nendoroid.Escultor, nendoroid.Cooperacao, nendoroid.IdSerie, tituloSerie);

            return Ok(new ResponseBase(true, null, resposta));
        }

        [HttpGet("BuscarPorNumero")]
        [Authorize(Roles = "Admin, User")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<NendoroidResponse>> BuscarPorNumero([FromQuery] string? numero = null)
        {
            if (numero == null)
                return BadRequest(new ResponseBase(false, "O campo Numero é obrigatório."));

            var nendoroid = await _nendoroidRepository.BuscarNendoroidPorNumero(numero);

            if (nendoroid == null)
                return NotFound(new ResponseBase(false, "Nendoroid não encontrada."));

            var tituloSerie = await _serieRepository.BuscarTituloPorIdSerie(nendoroid.IdSerie);

            var resposta = new NendoroidResponse(nendoroid.Id, nendoroid.Nome, nendoroid.Numero, nendoroid.PrecoJpy,
                nendoroid.DataLancamento, nendoroid.Escultor, nendoroid.Cooperacao, nendoroid.IdSerie, tituloSerie);

            return Ok(new ResponseBase(true, null, resposta));
        }
    }
}
