using System;

namespace NendoroidApi.Response.Base
{
    public class ResponseBase
    {
        public bool Sucesso { get; private set; }
        public string? Mensagem { get; set; }
        public object Data { get; private set; }

        public ResponseBase(bool sucesso, string? mensagem, object? data = null) 
        {
            Sucesso = sucesso;
            Mensagem = mensagem ?? null;
            Data = data ?? Array.Empty<object>();
        }
    }
}
