using Application.Abstractions.IRepository;
using Domain.Pandit;

namespace Application.Query.Pandit
{
    public static class GetPandit
    {
        #region Query
        public sealed record Query(Guid PanditId) : IQuery<Response>;
        #endregion

        #region Validation
        public sealed class Validator : AbstractValidator<Query>
        {
            public Validator(IPanditReadRepository panditReadRepository)
            {
                RuleFor(x => x.PanditId)
                    .NotEmpty().WithMessage("PanditId is required.")
                    .MustAsync(async (id, ct) => await panditReadRepository.Exists(id, ct))
                    .WithMessage("Pandit not found.");
            }
        }
        #endregion

        #region Handler
        public sealed class Handler : IQueryHandler<Query, Response>
        {
            private readonly IPanditReadRepository _panditReadRepository;

            public Handler(IPanditReadRepository panditReadRepository)
            {
                _panditReadRepository = panditReadRepository;
            }

            public async Task<Result<Response>> Handle(Query request, CancellationToken cancellationToken)
            {
                var pandit = await _panditReadRepository.GetPanditAsync(request.PanditId, cancellationToken);
                if (pandit is null)
                    return Result.Failure<Response>(PanditErrors.NotFound(request.PanditId));
                var reviews = pandit.PujaTypes
                    .Select(r => new PujaTypeResponse(r.PujaTypeId, r.Name, r.Description, r.IsRecurring))
                    .ToList();

                var verifications = pandit.Verifications
                    .Select(v => new VerificationResponse(v.VerificationId, v.DocumentName, v.DocumentPath, v.VerifiedOn))
                    .ToList();

                return new Response(
                    pandit.PanditId,
                    pandit.FullName,
                    pandit.Languages,
                    pandit.ExperienceInYears,
                    pandit.VerificationState?.Name,
                    pandit.AverageRating,
                    pandit.Address?.City,
                    pandit.Address?.Country,
                    reviews,
                    verifications
                );
            }
        }
        #endregion

        #region Response
        public record Response(
            Guid PanditId,
            string FullName,
            string Languages,
            int ExperienceInYears,
            string? VerificationState,
            decimal? AverageRating,
            string? City,
            string? Country,
            IReadOnlyList<PujaTypeResponse> PujaTypes,
            IReadOnlyList<VerificationResponse> Verifications
        );

        public record PujaTypeResponse(
            Guid Id,
            string Name,
            string ?Description,
            bool IsRecurring
        );

        public record VerificationResponse(
            Guid Id,
            string DocumentName,
            string DocumentPath,
            DateTimeOffset VerifiedOn
        );
        #endregion
    }
}
