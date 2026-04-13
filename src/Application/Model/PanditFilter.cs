using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Model
{
    public record PanditFilter : FilterBase
    {
        public string? VerificationState { get; init; }
    }
}
