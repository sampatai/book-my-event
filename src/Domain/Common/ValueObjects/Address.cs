using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ValueObjects;

public class Address : ValueObject
{
    protected Address()
    {

    }
    public string? Street { get; private set; }

    public string State { get; private set; }
    public string PostalCode { get; private set; }
    public string Country { get; private set; }
    public string City { get; private set; }

    public string AddressLine1 { get; private set; }
    public string? AddressLine2 { get; private set; }
    public string Timezone { get; private set; }
    public Address(string? street,
        string state, string postcode, string country, string city,
        string line1, string? line2, string timeZone)
    {
        Guard.Against.NullOrWhiteSpace(state, nameof(state));
        Guard.Against.NullOrWhiteSpace(postcode, nameof(postcode));
        Guard.Against.NullOrWhiteSpace(country, nameof(country));
        Guard.Against.NullOrWhiteSpace(city, nameof(city));
        Guard.Against.NullOrWhiteSpace(line1, nameof(line1));
        Guard.Against.NullOrWhiteSpace(timeZone, nameof(timeZone));
        this.Street = street;
        this.State = state;
        this.PostalCode = postcode;
        this.Country = country;
        City = city;
        this.AddressLine1 = line1;
        this.AddressLine2 = line2;
        this.Timezone = timeZone;
    }
    protected void SetAddressLines(string line1, string line2)
    {
        this.AddressLine1 = line1;
        this.AddressLine2 = line2;
    }
    protected void SetTimeZone(string timeZone)
    {
        Guard.Against.NullOrWhiteSpace(timeZone, nameof(timeZone));
        this.Timezone = timeZone;
    }
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Street;

        yield return State;
        yield return PostalCode;
        yield return Country;
        yield return AddressLine1;
        yield return AddressLine2;
        yield return Timezone;
        yield return City;
    }
}

