using NendoroidApi.Data.Model;
using NendoroidApi.Enum;
using System;
using System.Collections.Generic;

namespace NendoroidApi.Response
{
    public class BuscaNendoroidResponse
    {
        public int Page { get; set; }
        public int Size { get; set; }
        public int Total { get; set; }
        public List<NendoroidResponse> Nendoroids { get; set; } = new List<NendoroidResponse>();
        public BuscaNendoroidResponse(int page, int size, int total)
        {
            Page = page;
            Size = size;
            Total = total;
        }
        public void AdicionaNendoroid(NendoroidResponse nendo) => Nendoroids.Add(nendo);
    }

    public class NendoroidResponse
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Numero { get; set; }
        public decimal PrecoJpy { get; set; }
        public DateTime? DataLancamento { get; set; }
        public string? Escultor { get; set; }
        public string? Cooperacao { get; set; }
        public int IdSerie { get; set; }
        public string TituloSerie { get; set; }

        public NendoroidResponse(int id, string nome, string numero, decimal precoJpy, DateTime? dataLancamento, 
            string? escultor, string? cooperacao, int idSerie, string tituloSerie)
        {
            Id = id;
            Nome = nome;
            Numero = numero;
            PrecoJpy = precoJpy;
            DataLancamento = dataLancamento;
            Escultor = escultor;
            Cooperacao = cooperacao;
            IdSerie = idSerie;
            TituloSerie = tituloSerie;
        }
    }
}
