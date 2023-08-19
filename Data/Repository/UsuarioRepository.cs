using Dapper;
using NendoroidApi.Data.Base;
using NendoroidApi.Data.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NendoroidApi.Data.Repository
{
    public class UsuarioRepository
    {
        private readonly DbSession _session;

        public UsuarioRepository(DbSession session)
        {
            _session = session;
        }


        public async Task<Usuario?> BuscarUsuarioComRolesPorNome(string nome)
        {
            string sql = @"SELECT U.ID, U.NOME, U.SENHA, UR.ID_ROLE, R.ID, R.NOME FROM USUARIO U 
                           INNER JOIN USUARIOROLE UR ON U.ID = UR.ID_USUARIO
                           INNER JOIN ROLES R ON UR.ID_ROLE = R.ID
                           WHERE U.NOME = @NOME";

            var usuarioDictionary = new Dictionary<int, Usuario>();

            var parametros = new DynamicParameters();
            parametros.Add(name: "NOME", value: nome);

            await _session.Connection.QueryAsync<Usuario, Roles, Usuario>(
                sql: sql, 
                map: (usuario, role) =>
                {
                    if (!usuarioDictionary.TryGetValue(usuario.Id, out Usuario? usuarioEntry))
                        {
                            usuarioEntry = usuario;
                            usuarioDictionary.Add(usuarioEntry.Id, usuarioEntry);
                        }

                    usuarioEntry.UsuarioRoles.Add(role);
                    return usuarioEntry;
                },
                param: parametros,
                splitOn: "ID, ID_ROLE, ID",
                transaction: _session.Transaction
            );

            return usuarioDictionary.Values.FirstOrDefault();
        }
    }
}
