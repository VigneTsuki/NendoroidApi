using System.Collections.Generic;

namespace NendoroidApi.Data.Model
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string HashSenha { get; set; }
        public string SaltSenha { get; set; }
        public virtual List<Roles> UsuarioRoles { get; set; } = new List<Roles>();

        public bool ValidarSenha(string senha) => senha.Equals(HashSenha);
    }
}
