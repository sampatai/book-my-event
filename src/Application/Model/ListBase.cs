using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Model
{
    public abstract record ListBase<T>
    {
        public IEnumerable<T> Records { get; init; }
        public int TotalRecords { get; init; }
    }
}
