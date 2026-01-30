using System;
using System.Collections.Generic;
using Application.Abstractions.Messaging;

namespace Application.Query.Pandit
{
    public static class ListPandit
    {
        public sealed record Query() : IQuery<List<ListPanditResponse>>;

        public sealed record ListPanditResponse();

        public sealed class ListPanditHandler() : IQueryHandler<Query, List<ListPanditResponse>>
        {
            public Task<Result<List<ListPanditResponse>>> Handle(Query query, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}
