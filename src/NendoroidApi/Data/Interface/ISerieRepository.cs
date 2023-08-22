using System.Threading.Tasks;

namespace NendoroidApi.Data.Interface
{
    public interface ISerieRepository
    {
        Task<string> BuscarTituloPorIdSerie(int id);
    }
}
