using NendoroidApi.Data.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NendoroidApi.Data.Interface
{
    public interface INendoroidRepository
    {
        Task CadastrarNendoroid(Nendoroid nendoroid);
        Task<bool> NendoroidExiste(string numero);
        Task Deletar(string numero);
        Task<int> TotalNendorodoids();
        Task<IEnumerable<Nendoroid>> BuscarNendoroidsPaginado(int numeroPagina, int TamanhoPagina);
        Task<Nendoroid?> BuscarNendoroidPorId(int id);
        Task<Nendoroid?> BuscarNendoroidPorNumero(string numero);
        Task EditarNendoroid(Nendoroid nendoroid);
    }
}
