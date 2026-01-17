using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Booking.Enums
{
    public class BookingStatus : Enumeration
    {
        public static readonly BookingStatus Pending = new(1, "Pending");
        public static readonly BookingStatus Accepted = new(2, "Accepted");
        public static readonly BookingStatus Confirmed = new(3, "Confirmed");     
        public static readonly BookingStatus Cancelled = new(4, "Cancelled");
        public static readonly BookingStatus Completed = new(5, "Completed");

        public BookingStatus(int id, string name) : base(id, name)
        {
        }
    }
}
