namespace NendoroidApi.Response.Base
{
    public class ResponseBase
    {
        public bool Sucesso { get; set; }
        public object Data { get; set; }
        public string Erro { get; set; }

        public ResponseBase(bool sucesso, object data, string erro) 
        {
            Sucesso = sucesso;
            Data = data;
            Erro = erro;
        }
    }
}
