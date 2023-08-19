﻿using Dapper;
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

        public async Task Cadastrar(Usuario usuario)
        {
            string sql = "INSERT INTO USUARIO (NOME, SENHA) VALUES (@NOME, @SENHA)";

            var parametros = new DynamicParameters();
            parametros.Add(name: "NOME", value: usuario.Nome);
            parametros.Add(name: "SENHA", value: usuario.Senha);

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

                string sqlInsert = "INSERT INTO USUARIOROLE (ID_ROLE, ID_USUARIO) VALUES (@IDROLE, @IDUSUARIO)";

                await _session.Connection.ExecuteAsync(sqlInsert,
                    new { IDROLE = idRole, IDUSUARIO = idUsuario }, _session.Transaction);
            }
        }
    }
}
