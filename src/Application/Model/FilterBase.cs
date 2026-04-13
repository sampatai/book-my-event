using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Model
{
    public record FilterBase
    {
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 10;
        public string? SearchTerm { get; init; }
        public string? SortBy { get; init; }
        public string? SortDirection { get; init; } = "asc";

        public bool IsDescending => SortDirection?.Equals("desc", StringComparison.OrdinalIgnoreCase) == true;
    }
}
