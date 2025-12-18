using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedKernel;

namespace Auth.Domain.ValueObjects;

public class Address : ValueObject
{
    protected Address()
    {

    }
    public string Street { get; private set; }
    public string Suburb { get; private set; }
    public string State { get; private set; }
    public string PostalCode { get; private set; }
    public string Country { get; private set; }
    public string City { get; private set; }
    public Address(string street, string suburb,
        string state, string postcode, string country, string city)
    {
        this.Street = street;
        this.Suburb = suburb;
        this.State = state;
        this.PostalCode = postcode;
        this.Country = country;
        City = city;
    }
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Street;
        yield return Suburb;
        yield return State;
        yield return PostalCode;
        yield return Country;
        yield return City;
    }
}

