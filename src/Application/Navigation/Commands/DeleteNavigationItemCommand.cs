using Application.Abstractions.IRepository;
using Application.Abstractions.Messaging;
using SharedKernel;

namespace Application.Navigation.Commands
{
    public class DeleteNavigationItemCommand : ICommand
    {
        public long Id { get; set; }
    }

    public class DeleteNavigationItemCommandHandler : ICommandHandler<DeleteNavigationItemCommand>
    {
        private readonly INavigationRepository _repository;

        public DeleteNavigationItemCommandHandler(INavigationRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result> Handle(DeleteNavigationItemCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var navigationItem = await _repository.GetByIdAsync(command.Id, cancellationToken);
                if (navigationItem == null)
                    return Result.Failure(Error.NotFound("Navigation.NotFound", "Navigation item not found"));

                await _repository.RemoveAsync(navigationItem, cancellationToken);
                await _repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(Error.Problem("Navigation.DeleteError", ex.Message));
            }
        }
    }
}