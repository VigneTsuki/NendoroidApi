using Dapper;
using NendoroidApi.Data.Base;
using NendoroidApi.Data.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NendoroidApi.Data.Repository
{
    public class NendoroidRepository
    {
        private readonly DbSession _session;

        public NendoroidRepository(DbSession session)
        {
            _session = session;
        }

        public async Task CadastrarNendoroid(Nendoroid nendoroid)
        {
            string sql = @"INSERT INTO NENDOROID (NOME, NUMERO, PRECOJPY, DATALANCAMENTO, ESCULTOR, COOPERACAO, IDSERIE) 
                           VALUES (@NOME, @NUMERO, @PRECOJPY, @DATALANCAMENTO, @ESCULTOR, @COOPERACAO, @IDSERIE);";

            var parametros = new DynamicParameters();
            parametros.Add(name: "NOME", value: nendoroid.Nome);
            parametros.Add(name: "NUMERO", value: nendoroid.Numero);
            parametros.Add(name: "PRECOJPY", value: nendoroid.PrecoJpy);
            parametros.Add(name: "DATALANCAMENTO", value: nendoroid.DataLancamento);
            parametros.Add(name: "ESCULTOR", value: nendoroid.Escultor);
            parametros.Add(name: "COOPERACAO", value: nendoroid.Cooperacao);
            parametros.Add(name: "IDSERIE", value: nendoroid.IdSerie);

            await _session.Connection.ExecuteAsync(sql, parametros, _session.Transaction);
        }

        public async Task<bool> NendoroidExiste(string numero)
        {
            string sql = "SELECT COUNT(*) FROM NENDOROID WHERE NUMERO = @NUMERO";

            var quantidade = await _session.Connection.QuerySingleOrDefaultAsync<int>(sql,
                new { NUMERO = numero }, _session.Transaction);

            return quantidade != 0;
        }

        public async Task Deletar(string numero)
        {
            string sql = "DELETE FROM NENDOROID WHERE NUMERO = @NUMERO";

            await _session.Connection.ExecuteAsync(sql, new { NUMERO = numero}, _session.Transaction);
        }

        public async Task<int> TotalNendorodoids()
        {
            string sql = "SELECT COUNT(*) FROM NENDOROID;";

            var quantidade = await _session.Connection.QuerySingleOrDefaultAsync<int>(sql,
                null, _session.Transaction);

            return quantidade;
        }

        public async Task<IEnumerable<Nendoroid>> BuscarNendoroidsPaginado(int numeroPagina, int TamanhoPagina)
        {
            string sql = "SELECT * FROM NENDOROID LIMIT @SIZE OFFSET @PAGE;";

            var parametros = new DynamicParameters();
            parametros.Add("SIZE", TamanhoPagina);
            parametros.Add("PAGE", (numeroPagina - 1) * TamanhoPagina);

            var nendoroids = await _session.Connection.QueryAsync<Nendoroid>(sql,
                parametros, _session.Transaction);

            return nendoroids;
        }

        public async Task<Nendoroid?> BuscarNendoroidPorId(int id)
        {
            string sql = "SELECT * FROM NENDOROID WHERE ID = @ID;";

            var nendoroid = await _session.Connection.QueryFirstOrDefaultAsync<Nendoroid?>(sql,
                new { ID = id }, _session.Transaction);

            return nendoroid;
        }

        public async Task<Nendoroid?> BuscarNendoroidPorNumero(string numero)
        {
            string sql = "SELECT * FROM NENDOROID WHERE NUMERO = @NUMERO;";

            var nendoroid = await _session.Connection.QueryFirstOrDefaultAsync<Nendoroid?>(sql,
                new { NUMERO = numero }, _session.Transaction);

            return nendoroid;
        }
    }
}
