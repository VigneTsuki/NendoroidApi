using NendoroidApi.Data.Model;

namespace NendoroidApi.Auth
{
    public interface ITokenService
    {
        string CreateToken(Usuario usuario);
    }
}
