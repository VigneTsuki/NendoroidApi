using NendoroidApi.Enum;

namespace NendoroidApi.Data.Model
{
    public class Serie
    {
        public int Id { get; set; }
        public ETipoSerie Tipo { get; set; }
        public string Titulo { get; set; }
    }
}
