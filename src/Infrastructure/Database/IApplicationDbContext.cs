using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Booking.Root;
using Domain.Devotee.Root;
using Domain.Pandit.Root;

namespace Infrastructure.Database
{
    public interface IApplicationDbContext
    {
        DbSet<Devotee> Devotees { get; }
        DbSet<Pandit> Pandits { get; }
        DbSet<Booking> Bookings { get; }

    }
}
