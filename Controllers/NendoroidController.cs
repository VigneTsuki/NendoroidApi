using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NendoroidApi.Data.Model;
using NendoroidApi.Data.Repository;
using NendoroidApi.Enum;
using NendoroidApi.Request;
using NendoroidApi.Response.Base;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace NendoroidApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NendoroidController : ControllerBase
    {
        private readonly NendoroidRepository _nendoroidRepository;

        public NendoroidController(NendoroidRepository nendoroidRepository)
        {
            _nendoroidRepository = nendoroidRepository;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ResponseBase>> Post(NendoroidRequest request)
        {
            if(request == null)
                return BadRequest(new ResponseBase("Há algo errado com a requisição enviada. Por favor, faça os ajustes e tente novamente."));
            
            var validacaoRequest = request.ValidarRequest();
            if (!validacaoRequest.IsValid)
                return BadRequest(new ResponseBase(validacaoRequest.Errors));

            DateTime? data = null;

            if(request.DataLancamento != null)
                data = DateTime.ParseExact(request.DataLancamento!, "yyyy-MM", CultureInfo.InvariantCulture);

            if(await _nendoroidRepository.NendoroidExiste(request.Numero))
                return BadRequest(new ResponseBase("Nendoroid já está cadastrada."));

            var nendoroid = new Nendoroid
            {
                Nome = request.Nome,
                Numero = request.Numero,
                PrecoJpy = request.PrecoJpy,
                DataLancamento = data ?? null,
                Escultor = request.Escultor,
                Cooperacao = request.Cooperacao,
                IdSerie = (ETipoSerie) request.IdSerie!,
            };

            await _nendoroidRepository.CadastrarNendoroid(nendoroid);

            return Ok(request);
        }
    }
}
