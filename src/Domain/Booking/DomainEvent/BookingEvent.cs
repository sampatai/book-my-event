using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.ValueObjects;

namespace Domain.Booking.DomainEvent
{
    public record BookingCreatedEvent(long BookingId, long PanditId, long DevoteeId) : IDomainEvent;
    public record BookingAcceptedEvent(long BookingId, long PanditId, long DevoteeId) : IDomainEvent;
    public record BookingConfirmedEvent(long BookingId, long PanditId, long DevoteeId) : IDomainEvent;
    public record BookingCancelledEvent(long BookingId, long PanditId, long DevoteeId, string Reason) : IDomainEvent;
    public record BookingCompletedEvent(long BookingId, long PanditId, long DevoteeId) : IDomainEvent;
    public record BookingRescheduledEvent(long BookingId, DateOnly NewDate, TimeOnly NewTime) : IDomainEvent;
    public record BookingVenueUpdatedEvent(long BookingId, Address NewVenue) : IDomainEvent;

}
