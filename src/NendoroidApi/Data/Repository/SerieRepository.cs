using Dapper;
using NendoroidApi.Data.Base;
using NendoroidApi.Data.Interface;
using System.Threading.Tasks;

namespace NendoroidApi.Data.Repository
{
    public class SerieRepository : ISerieRepository
    {
        private readonly DbSession _session;

        public SerieRepository(DbSession session)
        {
            _session = session;
        }

        public async Task<string> BuscarTituloPorIdSerie(int id)
        {
            string sql = "SELECT TITULO FROM SERIE WHERE ID = @ID";

            var retorno = await _session.Connection.QueryFirstOrDefaultAsync<string>(sql, new { ID = id}, _session.Transaction);

            return retorno;
        }
    }
}
