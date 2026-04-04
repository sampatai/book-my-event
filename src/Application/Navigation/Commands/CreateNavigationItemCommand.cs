using Application.Abstractions.IRepository;
using Application.Abstractions.Messaging;
using Domain.Navigation.Root;
using SharedKernel;

namespace Application.Navigation.Commands
{
    public class CreateNavigationItemCommand : ICommand<long>
    {
        public string Title { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public string? RequiredPermission { get; set; }
        public long? ParentId { get; set; }
    }

    public class CreateNavigationItemCommandHandler : ICommandHandler<CreateNavigationItemCommand, long>
    {
        private readonly INavigationRepository _repository;

        public CreateNavigationItemCommandHandler(INavigationRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<long>> Handle(CreateNavigationItemCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var navigationItem = new NavigationItem(
                    command.Title,
                    command.Url,
                    command.RequiredPermission,
                    command.Icon);

                var addedItem = await _repository.AddAsync(navigationItem, cancellationToken);
                await _repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

                return Result<long>.Success(addedItem.Id);
            }
            catch (Exception ex)
            {
                return Result<long>.Failure<long>(Error.Problem("Navigation.CreateError", ex.Message));
            }
        }
    }
}