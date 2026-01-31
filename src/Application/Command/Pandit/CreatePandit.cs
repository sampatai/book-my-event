using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.IRepository;
using Application.Model;
using Domain.Pandit.Root;
using FluentValidation;
using SharedKernel;

namespace Application.Command
{
    public static class CreatePandit
    {
        #region Command
        public record Command(
            long UserId,
            string FullName,
            string Languages,
            int ExperienceInYears,
            AddressCommand Address
        ) : ICommand;

        
        #endregion

        #region Validator
        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.FullName)
                    .NotEmpty().WithMessage("Full name is required.");

                RuleFor(x => x.Languages)
                    .NotEmpty().WithMessage("Languages are required.");

                RuleFor(x => x.ExperienceInYears)
                    .GreaterThan(0).WithMessage("Experience must be greater than zero.");
            }
        }
        #endregion

        #region Handler
        internal sealed class Handler : ICommandHandler<Command>
        {
            private readonly IPanditRepository _panditRepository;

            public Handler(IPanditRepository panditRepository)
            {
                _panditRepository = panditRepository;
            }

            public async Task<Result> Handle(Command command, CancellationToken cancellationToken)
            {

                var pandit = new Pandit(
                    command.UserId,
                    command.FullName,
                    command.Languages,
                    command.ExperienceInYears
                );
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
                await _panditRepository.AddAsync(pandit, cancellationToken);
                await _panditRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

                return Result.Success();
            }
        }
        #endregion
    }
}
