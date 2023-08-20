using Dapper;
using NendoroidApi.Data.Base;
using NendoroidApi.Data.Model;
using System.Threading.Tasks;

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
    }
}
