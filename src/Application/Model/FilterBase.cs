using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Model
{
    public record FilterBase(int PageNumber = 1, int PageSize = 10, string SearchTerm = "");

}
