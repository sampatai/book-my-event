using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.ValueObjects;
using SharedKernel;

namespace Domain.Devotee.Root;

public class Devotee : AuditableEntity, IAggregateRoot
{
    // Aggregate identity
    public Guid DevoteeId { get; private set; }

    // Invariant: one profile per user (enforced at repository/db level via unique index)
    public long UserId { get; private set; }

    // Simple value objects could be introduced later for richer behavior
    public string FullName { get; private set; }

    // Address as value object
    public Address Address { get; private set; }


    // For EF Core and serializers
    protected Devotee() { }

    public Devotee(

        long userId,
        string fullName,
        string street,
        string state, string postcode, string country, string city,
        string timezone, string addressLine1, string addressLine2)
    {

        UserId = userId;
        FullName = fullName;
        Address = new Address(
            street,
            suburb,
            state,
            postcode,
            country,
            city, addressLine1, addressLine2, timezone);
        DevoteeId = Guid.NewGuid();



    }


    public void UpdateName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name is required.", nameof(fullName));

        FullName = fullName.Trim();
    }

    public void UpdateAddress(
        string addressLine1,
        string? addressLine2,
        string city,
        string postcode,
        string state,
        string country, string street, string suburb, string timezone)
    {
        Address = new Address(
            street,
            state,
            postcode,
            country,
            city, addressLine1, addressLine2, timezone);
    }


}

