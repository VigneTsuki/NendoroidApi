using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NendoroidApi.Auth;
using NendoroidApi.Data.Base;
using NendoroidApi.Data.Model;
using NendoroidApi.Data.Repository;
using NendoroidApi.Request;
using NendoroidApi.Response.Base;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly string _pepper;

        public UsuarioController(UsuarioRepository usuarioRepository, IUnitOfWork unitOfWork)
        {
            _usuarioRepository = usuarioRepository;
            _unitOfWork = unitOfWork;
            _pepper = Environment.GetEnvironmentVariable("PASSWORD_HASH");
        }

        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseBase>> Post(CadastroUsuarioRequest request)
        {
            var validacaoRequest = request.ValidarRequest();
            if (!validacaoRequest.IsValid)
                return BadRequest(new ResponseBase(false, validacaoRequest.Errors.FirstOrDefault()?.ErrorMessage));

            if(!request.SenhaEReSenhaIguais())
                return BadRequest(new ResponseBase(false, "As senhas precisam ser iguais."));

            if(await _usuarioRepository.UsuarioExiste(request.Nome))
                return BadRequest(new ResponseBase(false, "Usuário já existe."));

            var saltSenha = Hasher.GenerateSalt();

            var usuario = new Usuario
            {
                Nome = request.Nome,
                HashSenha = Hasher.ComputeHash(request.Senha, saltSenha, _pepper, 3),
                SaltSenha = saltSenha,
                UsuarioRoles = new List<Roles> { new Roles { Nome = "User" } }
            };

            _unitOfWork.BeginTransaction();

            await _usuarioRepository.Cadastrar(usuario);

            _unitOfWork.Commit();

            return Created(string.Empty, new ResponseBase(true, "Usuário cadastrado com sucesso"));
        }

        [HttpPost("Inativar")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseBase>> Inativar([FromQuery] int idUsuario = 0)
        {
            if(idUsuario == 0)
                return BadRequest(new ResponseBase(false, "O campo IdUsuario é obrigatório."));

            await _usuarioRepository.InativarUsuario(idUsuario);

            return Ok(new ResponseBase(true, "Usuário inativado com sucesso"));
        }

        [HttpPost("Ativar")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseBase>> Ativar([FromQuery] int idUsuario = 0)
        {
            if (idUsuario == 0)
                return BadRequest(new ResponseBase(false, "O campo IdUsuario é obrigatório."));

            await _usuarioRepository.AtivarUsuario(idUsuario);

            return Ok(new ResponseBase(true, "Usuário ativado com sucesso"));
        }
    }
}
