using FluentValidation;
using FluentValidation.Results;

namespace NendoroidApi.Request
{
    public class CadastroUsuarioRequest
    {
        public string Nome { get; set; }
        public string Senha { get; set; }
        public string ReSenha { get; set; }

        public ValidationResult ValidarRequest()
        {
            return new CadastroUsuarioRequestValidator().Validate(this);
        }

        public bool SenhaEReSenhaIguais() => Senha.Equals(ReSenha);
    }

    public class CadastroUsuarioRequestValidator : AbstractValidator<CadastroUsuarioRequest>
    {
        public CadastroUsuarioRequestValidator()
        {
            RuleFor(request => request.Nome)
                .NotEmpty()
                .WithMessage("O campo Nome é obrigatório.")
                .MaximumLength(250)
                .WithMessage("O campo Nome aceita no máximo 250 caracteres.");

            RuleFor(request => request.Senha)
                .NotEmpty()
                .WithMessage("O campo Senha é obrigatório.")
                .MaximumLength(250)
                .WithMessage("O campo Senha aceita no máximo 250 caracteres.");

            RuleFor(request => request.ReSenha)
                .NotEmpty()
                .WithMessage("O campo ReSenha é obrigatório.")
                .MaximumLength(250)
                .WithMessage("O campo ReSenha aceita no máximo 250 caracteres.");
        }
    }
}
