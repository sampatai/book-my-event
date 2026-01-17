using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Booking.Enums;
using Domain.ValueObjects;
using Serilog;

namespace Domain.Booking.Root
{
    public class Bookings : AuditableEntity, IAggregateRoot
    {
        protected Bookings() { }

        public Guid BookingId { get; private set; }
        public long PanditId { get; private set; }
        public long DevoteeId { get; private set; }

        public long PujaTypeId { get; private set; }
        public DateOnly BookingDate { get; private set; }
        public TimeOnly BookingTime { get; private set; }
        public string SpecialInstructions { get; private set; }
        public Address PujaVenue {  get; private set; }
        public BookingStatus BookingStatus { get; private set; }
        public string? CancellationReason { get; private set; }
        public DateTimeOffset? CancelledAt { get; private set; }
        public DateTimeOffset? CompletedAt { get; private set; }

    }
}
