using NendoroidApi.Data.Base;

namespace NendoroidApi.Data.Repository
{
    public class SerieRepository
    {
        private readonly DbSession _session;

        public SerieRepository(DbSession session)
        {
            _session = session;
        }
    }
}
