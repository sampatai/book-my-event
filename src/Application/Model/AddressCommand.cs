using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Model
{
    public record AddressCommand(
            string Street,
            string City,
            string State,
            string PostalCode,
            string Country,
            string AddressLine1,
            string? AddressLine2
        );
}
