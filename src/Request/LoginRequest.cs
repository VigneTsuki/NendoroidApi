using FluentValidation;
using FluentValidation.Results;

namespace NendoroidApi.Request
{
    public class LoginRequest
    {
        public string Nome { get; set; }
        public string Senha { get; set; }

        public ValidationResult ValidarRequest()
        {
            return new LoginRequestValidator().Validate(this);
        }
    }

    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
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
        }
    }
}
