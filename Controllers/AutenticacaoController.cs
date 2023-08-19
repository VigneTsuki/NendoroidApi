using Microsoft.AspNetCore.Mvc;
using NendoroidApi.Auth;
using NendoroidApi.Data.Repository;
using NendoroidApi.Request;
using System.Threading.Tasks;

namespace NendoroidApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AutenticacaoController : ControllerBase
    {
        private readonly UsuarioRepository _usuarioRepository;
        private readonly TokenService _tokenService;


        public AutenticacaoController(UsuarioRepository usuarioRepository, TokenService tokenService)
        {
            _usuarioRepository = usuarioRepository;
            _tokenService = tokenService;
        }

        [HttpPost]
        public async Task<JsonResult> Post(LoginRequest request)
        {
            var usuario = await _usuarioRepository.BuscarUsuarioComRolesPorNome(request.Nome);

            if(usuario == null) { return new JsonResult(new { Sucesso = false }); }

            if(!usuario.ValidarSenha(request.Senha)) { return new JsonResult(new { Sucesso = false }); }

            var token = _tokenService.CreateToken(usuario);

            return new JsonResult(new { Sucesso = true, Token = token });
        }
    }
}
