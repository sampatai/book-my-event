using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Model
{
    public record PanditFilter : FilterBase
    {
        public PanditFilter(int PageNumber = 1, int PageSize = 10, string SearchTerm = "") : base(PageNumber, PageSize, SearchTerm)
        {
        }
    }
}
