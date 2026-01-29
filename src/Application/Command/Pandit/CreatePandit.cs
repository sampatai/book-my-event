using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Application.Todos.Complete;
using FluentValidation;

namespace Application.Command.Pandit
{
    public static class CreatePandit
    {
        public record Command(string name): ICommand;

        internal sealed class Validator: AbstractValidator<Command>
        {
            public Validator()
            {
                
            }
        }
        internal sealed class CreatePanditHandler(

    )
    : ICommandHandler<Command>
        {
           
            public Task<Result> Handle(Command command, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}
