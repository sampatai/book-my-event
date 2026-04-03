using Application.Abstractions.IRepository;
using Application.Abstractions.Messaging;
using SharedKernel;

namespace Application.Navigation.Commands
{
    public class UpdateNavigationItemCommand : ICommand
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public string? RequiredPermission { get; set; }
    }

    public class UpdateNavigationItemCommandHandler : ICommandHandler<UpdateNavigationItemCommand>
    {
        private readonly INavigationRepository _repository;

        public UpdateNavigationItemCommandHandler(INavigationRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result> Handle(UpdateNavigationItemCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var navigationItem = await _repository.GetByIdAsync(command.Id, cancellationToken);
                if (navigationItem == null)
                    return Result.Failure(Error.NotFound("Navigation.NotFound", "Navigation item not found"));

                navigationItem.UpdateDetails(command.Title, command.Url, command.RequiredPermission, command.Icon);

                await _repository.UpdateAsync(navigationItem, cancellationToken);
                await _repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(Error.Problem("Navigation.UpdateError", ex.Message));
            }
        }
    }
}