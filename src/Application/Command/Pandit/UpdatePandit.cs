using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.IRepository;
using Application.Model;
using Domain.Pandit.Root;
using FluentValidation;
using SharedKernel;

namespace Application.Command
{
    public static class UpdatePandit
    {
       
        public sealed record UpdatePanditCommand(
            Guid PanditId,
            string FullName,
            string Languages,
            int ExperienceInYears,
            AddressCommand Address
        ) : ICommand<UpdatePanditResponse>;

        internal sealed class Validator : AbstractValidator<UpdatePanditCommand>
        {
            public Validator()
            {
                RuleFor(x => x.PanditId).NotEmpty();
                RuleFor(x => x.FullName).NotEmpty();
                RuleFor(x => x.Languages).NotEmpty();
                RuleFor(x => x.ExperienceInYears).GreaterThan(0);
            }
        }

        internal sealed class UpdatePanditHandler : ICommandHandler<UpdatePanditCommand, UpdatePanditResponse>
        {
            private readonly IPanditRepository _panditRepository;

            public UpdatePanditHandler(IPanditRepository panditRepository)
            {
                _panditRepository = panditRepository;
            }

            public async Task<Result<UpdatePanditResponse>> Handle(UpdatePanditCommand command, CancellationToken cancellationToken)
            {
                // Fetch the aggregate
                var pandit = await _panditRepository.GetPanditAsync(command.PanditId, cancellationToken);
                if (pandit is null)
                    return Result.Failure<UpdatePanditResponse>(Error.NotFound("Pandit", command.PanditId.ToString()));

                // Update fields
                pandit.SetPandit(command.FullName, command.Languages, command.ExperienceInYears);

                // Optionally update address if provided

                pandit.SetAddress(
                     addressLine1: command.Address.AddressLine1,
                     addressLine2: command.Address.AddressLine2,
                     city: command.Address.City,
                     postcode: command.Address.PostalCode,
                     state: command.Address.State,
                     country: command.Address.Country,
                     street: command.Address.Street,
                     timezone: TimeZoneInfo.Local.Id
                 );

                await _panditRepository.UpdateAsync(pandit, cancellationToken);
                await _panditRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

                return Result.Success(new UpdatePanditResponse());
            }
        }

        public sealed record UpdatePanditResponse();
    }
}
