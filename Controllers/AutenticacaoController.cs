using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NendoroidApi.Auth;
using NendoroidApi.Data.Repository;
using NendoroidApi.Request;
using NendoroidApi.Response;
using NendoroidApi.Response.Base;
using System;
using System.Threading.Tasks;

namespace NendoroidApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AutenticacaoController : ControllerBase
    {
        private readonly UsuarioRepository _usuarioRepository;
        private readonly TokenService _tokenService;
        private readonly string _pepper;

        public AutenticacaoController(UsuarioRepository usuarioRepository, TokenService tokenService)
        {
            _usuarioRepository = usuarioRepository;
            _tokenService = tokenService;
            _pepper = Environment.GetEnvironmentVariable("PASSWORD_HASH");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<ResponseBase>> Post(LoginRequest request)
        {
            var validacaoRequest = request.ValidarRequest();
            if(!validacaoRequest.IsValid)
                return BadRequest(new ResponseBase(validacaoRequest.Errors));

            var usuario = await _usuarioRepository.BuscarUsuarioComRolesPorNome(request.Nome);

            if (usuario == null)
                return BadRequest(new ResponseBase("Usuário ou senha incorretos."));

            var passwordHash = Hasher.ComputeHash(request.Senha, usuario.SaltSenha, _pepper, 3);

            if (!usuario.ValidarSenha(passwordHash))
                return BadRequest(new ResponseBase("Usuário ou senha incorretos."));

            var token = _tokenService.CreateToken(usuario);

            return Ok(new ResponseBase(new LoginResponse { Token = token }));
        }
    }
}
