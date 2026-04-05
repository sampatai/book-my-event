using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Model
{
    public record PanditSpecificFilters
    {
        public string? VerificationState { get; init; }
    }

    public record PanditFilter : FilterBase<PanditSpecificFilters>;
}
