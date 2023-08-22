using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NendoroidApi.Auth;
using NendoroidApi.Data.Repository;
using NendoroidApi.Request;
using NendoroidApi.Response;
using NendoroidApi.Response.Base;
using System;
using System.Linq;
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
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseBase>> Post(LoginRequest request)
        {
            var validacaoRequest = request.ValidarRequest();
            if(!validacaoRequest.IsValid)
                return BadRequest(new ResponseBase(false, validacaoRequest.Errors.FirstOrDefault()?.ErrorMessage));

            var usuario = await _usuarioRepository.BuscarUsuarioComRolesPorNome(request.Nome);

            if (usuario == null)
                return BadRequest(new ResponseBase(false, "Usuário ou senha incorretos."));

            var passwordHash = Hasher.ComputeHash(request.Senha, usuario.SaltSenha, _pepper, 3);

            if (!usuario.ValidarSenha(passwordHash))
                return BadRequest(new ResponseBase(false, "Usuário ou senha incorretos."));

            var token = _tokenService.CreateToken(usuario);

            return Ok(new ResponseBase(true, null, new LoginResponse { Token = token }));
        }
    }
}
