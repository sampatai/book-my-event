using Domain.Booking.Enums;
using Domain.ValueObjects;
using Domain.Booking.DomainEvent;

namespace Domain.Booking.Root
{
    public class Booking : Entity, IAggregateRoot
    {

        protected Booking() { }

        public Booking(

            long panditId,
            long devoteeId,
            long pujaTypeId,
            DateOnly bookingDate,
            TimeOnly bookingTime,
            string specialInstructions,
            Address pujaVenue)
        {
            BookingId = Guid.NewGuid();
            PanditId = panditId;
            DevoteeId = devoteeId;
            PujaTypeId = pujaTypeId;
            BookingDate = bookingDate;
            BookingTime = bookingTime;
            SpecialInstructions = specialInstructions;
            PujaVenue = pujaVenue;
            BookingStatus = BookingStatus.Pending;
            Raise(new BookingCreatedEvent(
               this.Id,
               panditId,
               devoteeId));
        }
        public Guid BookingId { get; private set; }
        public long PanditId { get; private set; }
        public long DevoteeId { get; private set; }
        public long PujaTypeId { get; private set; }
        public DateOnly BookingDate { get; private set; }
        public TimeOnly BookingTime { get; private set; }
        public string SpecialInstructions { get; private set; } = string.Empty;
        public Address PujaVenue { get; private set; } = default!;
        public BookingStatus BookingStatus { get; private set; }
        public string? CancellationReason { get; private set; }

        public void SetStatus(BookingStatus bookingStatus)
        {
            BookingStatus = bookingStatus;
            Raise(new BookingStatusEvent(
                this.Id,
                PanditId,
                DevoteeId, bookingStatus));

        }
        public void SetCancellation(string? cancellationReason)
        {
            CancellationReason = cancellationReason;

        }

        public void Reschedule(DateOnly newDate, TimeOnly newTime)
        {

            BookingDate = newDate;
            BookingTime = newTime;
            Raise(new BookingRescheduledEvent(
                this.Id,
                newDate,
                newTime));
        }
        public void UpdateVenue(Address newVenue)
        {
            PujaVenue = newVenue;

            Raise(new BookingVenueUpdatedEvent(
                this.Id,
                newVenue));
        }
    }
}
