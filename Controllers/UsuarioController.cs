using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
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
        [AllowAnonymous]
        public async Task<ActionResult<ResponseBase>> Post(CadastroUsuarioRequest request)
        {
            var validacaoRequest = request.ValidarRequest();
            if (!validacaoRequest.IsValid)
                return BadRequest(new ResponseBase(validacaoRequest.Errors));

            if(!request.SenhaEReSenhaIguais())
                return BadRequest(new ResponseBase("As senhas precisam ser iguais."));

            if(await _usuarioRepository.UsuarioExiste(request.Nome))
                return BadRequest(new ResponseBase("Usuário já existe."));

            var usuario = new Usuario
            {
                Nome = request.Nome,
                Senha = request.Senha,
                UsuarioRoles = new List<Roles> { new Roles { Nome = "User" } }
            };

            _unitOfWork.BeginTransaction();

            await _usuarioRepository.Cadastrar(usuario);

            _unitOfWork.Commit();

            return Created(string.Empty, new ResponseBase { Mensagem = "Usuário cadastrado com sucesso" });
        }

        [HttpPost("Inativar")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ResponseBase>> Inativar([FromQuery] int idUsuario = 0)
        {
            if(idUsuario == 0)
                return BadRequest(new ResponseBase("Id usuário não informado."));

            await _usuarioRepository.InativarUsuario(idUsuario);

            return Ok(new ResponseBase { Mensagem = "Usuário inativado com sucesso" });
        }

        [HttpPost("Ativar")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ResponseBase>> Ativar([FromQuery] int idUsuario = 0)
        {
            if (idUsuario == 0)
                return BadRequest(new ResponseBase("Id usuário não informado."));

            await _usuarioRepository.AtivarUsuario(idUsuario);

            return Ok(new ResponseBase { Mensagem = "Usuário ativado com sucesso" });
        }
    }
}
