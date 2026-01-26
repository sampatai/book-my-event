using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Todos.Complete;

internal sealed class CompleteTodoCommandHandler(
   
    )
    : ICommandHandler<CompleteTodoCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CompleteTodoCommand command, CancellationToken cancellationToken)
    {
        

        return Guid.NewGuid();
    }
}
