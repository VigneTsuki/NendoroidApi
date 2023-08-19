using FluentValidation.Results;
using System;
using System.Collections.Generic;

namespace NendoroidApi.Response.Base
{
    public class ResponseBase
    {
        public bool Sucesso { get; private set; }
        public object Data { get; private set; }
        public List<ErrosResponse> Erros { get; private set; } = new List<ErrosResponse>();

        public ResponseBase(string erro) 
        {
            Sucesso = false;
            Data = Array.Empty<object>();
            AdicionarErro(erro);
        }

        public ResponseBase(List<ValidationFailure> erros)
        {
            Sucesso = false;
            Data = Array.Empty<object>();
            foreach (var r in erros)
                AdicionarErro(r.ErrorMessage);
        }

        public ResponseBase(object data)
        {
            Sucesso = true;
            Data = data;
        }

        private void AdicionarErro(string mensagemErro) => Erros.Add(new ErrosResponse(mensagemErro));
    }

    public class ErrosResponse
    {
        public string Mensagem { get; set; }
        public ErrosResponse(string mensagem) { Mensagem = mensagem; }
    }
}
