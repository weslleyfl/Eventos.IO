using System;
using System.Collections.Generic;
using FluentValidation.Results;
using System.Text;
using Eventos.IO.Domain.Interfaces;

namespace Eventos.IO.Domain.CommandHandlers
{
    public abstract class CommandHandler
    {
        private readonly IUnitOfWork _uow;

        protected CommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        protected void NotificarValidacoesErro(ValidationResult validationResult)
        {
            foreach (var erro in validationResult.Errors)
            {
                Console.WriteLine(erro.ErrorMessage);
            }

        }

        protected bool Commit()
        {
            // TODO: Validar antes do commit se há alguma validaçao de negocio com erro!

            var responseCommand = _uow.Commit();
            if (responseCommand.Success) return true;

            Console.WriteLine("Ocorreu um erro ao salvar os dados no banco.");
            return false;

        }
    }
}
