using System;
using System.Collections.Generic;
using Application.Abstractions.IRepository;
using Application.Abstractions.Messaging;
using Application.Model;

namespace Application.Query.Pandit
{
    public static class ListPandit
    {
        public sealed record Query(PanditFilter PanditFilter) : IQuery<ListPanditResponse>;

        public record PanditResponse(Guid PanditId,
            string FullName,
            string Languages,
            int ExperienceInYears,
            string? VerificationState,
            string? City,
            string? Country);
        public sealed record ListPanditResponse : ListBase<PanditResponse>;

        #region Validation
        public sealed class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(x => x.PanditFilter.PageNumber)
                    .GreaterThanOrEqualTo(1)
                    .WithMessage("Invalid {PropertyName} (> 0)")
                    .WithName("page number");

                RuleFor(x => x.PanditFilter.PageSize)
                    .GreaterThanOrEqualTo(1)
                    .WithMessage("Invalid {PropertyName} (> 0)")
                    .WithName("page size");

                When(x => !string.IsNullOrEmpty(x.PanditFilter.SearchTerm), () => RuleFor(x => x.PanditFilter.SearchTerm)
                        .MinimumLength(3)
                        .WithMessage("Please provide at least 3 letters to initiate a search."));
            }
        }
        #endregion
        public sealed class ListPanditHandler(IPanditReadRepository panditReadRepository) : IQueryHandler<Query, ListPanditResponse>
        {
            public async Task<Result<ListPanditResponse>> Handle(Query query, CancellationToken cancellationToken)
            {
                var result = await panditReadRepository.ReadAsync(query.PanditFilter, cancellationToken);

                var responses = result.Pandits.Select(pandit => new PanditResponse(
                    pandit.PanditId,
                    pandit.FullName,
                    pandit.Languages,
                    pandit.ExperienceInYears,
                    pandit.VerificationState?.Name,
                    pandit.Address?.City,
                    pandit.Address?.Country
                )).ToList();

                return new ListPanditResponse {
                    Records = responses,
                    TotalRecords = result.TotalCount
                };
            }
        }
    }
}
