using System;

namespace NendoroidApi.Data.Model
{
    public class Nendoroid
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Numero { get; set; }
        public decimal PrecoJpy { get; set; }
        public DateTime? DataLancamento { get; set; }
        public string? Escultor { get; set; }
        public string? Cooperacao { get; set; }
        public int IdSerie { get; set; }
    }
}
