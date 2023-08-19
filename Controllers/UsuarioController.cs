using Microsoft.AspNetCore.Mvc;
using NendoroidApi.Data.Base;
using NendoroidApi.Data.Model;
using NendoroidApi.Data.Repository;
using NendoroidApi.Request;
using NendoroidApi.Response;
using NendoroidApi.Response.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NendoroidApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioRepository _usuarioRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UsuarioController(UsuarioRepository usuarioRepository, IUnitOfWork unitOfWork)
        {
            _usuarioRepository = usuarioRepository;
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        public async Task<ActionResult<ResponseBase>> Post(CadastroUsuarioRequest request)
        {
            var validacaoRequest = request.ValidarRequest();
            if (!validacaoRequest.IsValid)
                return BadRequest(new ResponseBase(validacaoRequest.Errors));

            if(!request.SenhaEReSenhaIguais())
                return BadRequest(new ResponseBase("As senhas precisam ser iguais."));

            var usuario = new Usuario
            {
                Nome = request.Nome,
                Senha = request.Senha,
                UsuarioRoles = new List<Roles> { new Roles { Nome = "User" } }
            };

            _unitOfWork.BeginTransaction();

            await _usuarioRepository.Cadastrar(usuario);

            _unitOfWork.Commit();

            return Created(string.Empty, new ResponseBase(new CadastroUsuarioResponse { Mensagem = "Usuário cadastrado com sucesso" }));
        }
    }
}
