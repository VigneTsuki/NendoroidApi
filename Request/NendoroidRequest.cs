﻿using FluentValidation;
using FluentValidation.Results;
using NendoroidApi.Enum;
using System;

namespace NendoroidApi.Request
{
    public class NendoroidRequest
    {
        public string Nome { get; set; }
        public string Numero { get; set; }
        public decimal PrecoJpy { get; set; }
        public string? DataLancamento { get; set; }
        public string? Escultor { get; set; }
        public string? Cooperacao { get; set; }
        public ETipoSerie? IdSerie { get; set; }

        public ValidationResult ValidarRequest()
        {
            return new NendoroidRequestValidator().Validate(this);
        }
    }

    public class NendoroidRequestValidator : AbstractValidator<NendoroidRequest>
    {
        public NendoroidRequestValidator()
        { 
            RuleFor(request => request.Nome)
                .NotEmpty()
                .WithMessage("O campo Nome é obrigatório.")
                .MaximumLength(250)
                .WithMessage("O campo Nome aceita no máximo 250 caracteres.");

            RuleFor(request => request.Numero)
                .NotEmpty()
                .WithMessage("O campo Numero é obrigatório.")
                .MaximumLength(20)
                .WithMessage("O campo Numero aceita no máximo 20 caracteres.");

            When(request => request.PrecoJpy != 0, () =>
            {
                RuleFor(request => request.PrecoJpy)
                    .LessThan(9_999_999_999.99m)
                    .WithMessage("O campo Preco aceita no máximo o valor 9.");
            });

            When(request => request.DataLancamento != null, () =>
            {
                RuleFor(request => request.DataLancamento)
                    .Matches(@"^\d{4}-\d{2}$")
                    .WithMessage("O campo DataLancamento precisa estar no padrão 'yyyy-MM'.");
            });

            When(request => request.Escultor != null, () =>
            {
                RuleFor(request => request.Escultor)
                    .MaximumLength(250)
                    .WithMessage("O campo Escultor aceita no máximo 250 caracteres.");
            });

            When(request => request.Cooperacao != null, () =>
            {
                RuleFor(request => request.Cooperacao)
                    .MaximumLength(250)
                    .WithMessage("O campo Cooperacao aceita no máximo 250 caracteres.");
            });

            RuleFor(request => request.IdSerie)
                .NotEmpty()
                .WithMessage("O campo IdSerie é obrigatório.")
                .IsInEnum()
                .WithMessage("O campo IdSerie precisa ter o valor entre 1 e 4.");
        }
    }
}