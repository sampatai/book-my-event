using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Pandit.Root;

namespace Infrastructure.Database
{
    public interface IApplicationDbContext
    {
        DbSet<Pandit> Pandits { get;}

    }
}
