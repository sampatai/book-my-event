using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Command.Pandit
{
    public static class UpdatePandit
    {
        public sealed record Command() : ICommand<UpdatePanditResponse>;

        internal sealed class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
            }
        }
        internal sealed class UpdatePanditHandler() : ICommandHandler<Command, UpdatePanditResponse>
        {
            public Task<Result<UpdatePanditResponse>> Handle(Command command, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }

        public sealed record UpdatePanditResponse();
    }
}
