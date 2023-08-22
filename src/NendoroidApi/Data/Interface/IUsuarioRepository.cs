using NendoroidApi.Data.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NendoroidApi.Data.Interface
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> BuscarUsuarioComRolesPorNome(string nome);
        Task Cadastrar(Usuario usuario);
        Task CadastrarRoles(int idUsuario, List<Roles> roles);
        Task<bool> UsuarioExiste(string nome);
        Task InativarUsuario(int idUsuario);
        Task AtivarUsuario(int idUsuario);
    }
}
