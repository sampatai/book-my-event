using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Booking.Enums;
using Domain.ValueObjects;

namespace Domain.Booking.DomainEvent
{
    public record BookingCreatedEvent(Guid BookingId, long PanditId, long DevoteeId) : IDomainEvent;
    public record BookingStatusEvent(Guid BookingId, long PanditId, long DevoteeId, BookingStatus BookingStatus) : IDomainEvent;
    public record BookingRescheduledEvent(Guid BookingId, DateOnly NewDate, TimeOnly NewTime) : IDomainEvent;
    public record BookingVenueUpdatedEvent(Guid BookingId, Address NewVenue) : IDomainEvent;

}
