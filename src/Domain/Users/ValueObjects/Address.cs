using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Users.ValueObjects;

public class Address : ValueObject
{
    protected Address()
    {

    }
    public string Street { get; private set; }
    public string Suburb { get; private set; }
    public string State { get; private set; }
    public string Postcode { get; private set; }
    public string Country { get; private set; }
    public string City { get; private set; }
    internal Address(string street, string suburb,
        string state, string postcode, string country, string city)
    {
        this.Street = street;
        this.Suburb = suburb;
        this.State = state;
        this.Postcode = postcode;
        this.Country = country;
        City = city;
    }
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Street;
        yield return Suburb;
        yield return State;
        yield return Postcode;
        yield return Country;
        yield return City;
    }
}

