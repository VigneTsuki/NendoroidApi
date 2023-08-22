using Dapper;
using NendoroidApi.Data.Base;
using NendoroidApi.Data.Model;
using System;
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
            string sql = @"SELECT U.ID, U.NOME, U.HASHSENHA, U.SALTSENHA, UR.IDROLE, R.ID, R.NOME FROM USUARIO U 
                           INNER JOIN USUARIOROLE UR ON U.ID = UR.IDUSUARIO
                           INNER JOIN ROLES R ON UR.IDROLE = R.ID
                           WHERE U.NOME = @NOME AND ATIVO = 1";

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
                splitOn: "ID, IDROLE, ID",
                transaction: _session.Transaction
            );

            return usuarioDictionary.Values.FirstOrDefault();
        }

        public async Task Cadastrar(Usuario usuario)
        {
            string sql = "INSERT INTO USUARIO (NOME, HASHSENHA, SALTSENHA) VALUES (@NOME, @HASHSENHA, @SALTSENHA)";

            var parametros = new DynamicParameters();
            parametros.Add(name: "NOME", value: usuario.Nome);
            parametros.Add(name: "HASHSENHA", value: usuario.HashSenha);
            parametros.Add(name: "SALTSENHA", value: usuario.SaltSenha);

            await _session.Connection.ExecuteAsync(sql, parametros, _session.Transaction);

            string query = "SELECT ID FROM USUARIO WHERE NOME = @NOME";

            var idUsuario = await _session.Connection.QueryFirstOrDefaultAsync<int>(query,
                new { NOME = usuario.Nome }, _session.Transaction);

            await CadastrarRoles(idUsuario, usuario.UsuarioRoles);
        }

        public async Task CadastrarRoles(int idUsuario, List<Roles> roles)
        {
            foreach (var r in roles)
            {
                string sql = "SELECT ID FROM ROLES WHERE NOME = @NOME";

                var idRole = await _session.Connection.QueryFirstOrDefaultAsync<Guid>(sql,
                    new { NOME = r.Nome }, _session.Transaction);

                string sqlInsert = "INSERT INTO USUARIOROLE (IDROLE, IDUSUARIO) VALUES (@IDROLE, @IDUSUARIO)";

                await _session.Connection.ExecuteAsync(sqlInsert,
                    new { IDROLE = idRole, IDUSUARIO = idUsuario }, _session.Transaction);
            }
        }

        public async Task<bool> UsuarioExiste(string nome)
        {
            string sql = "SELECT COUNT(*) FROM USUARIO WHERE NOME = @NOME";

            var quantidade = await _session.Connection.QuerySingleOrDefaultAsync<int>(sql,
                new { NOME = nome }, _session.Transaction);

            return quantidade != 0;
        }

        public async Task InativarUsuario(int idUsuario)
        {
            string sql = "UPDATE USUARIO SET ATIVO = 0 WHERE ID = @IDUSUARIO";

            await _session.Connection.ExecuteAsync(sql,
                new { IDUSUARIO = idUsuario }, _session.Transaction);
        }

        public async Task AtivarUsuario(int idUsuario)
        {
            string sql = "UPDATE USUARIO SET ATIVO = 1 WHERE ID = @IDUSUARIO";

            await _session.Connection.ExecuteAsync(sql,
                new { IDUSUARIO = idUsuario }, _session.Transaction);
        }
    }
}
