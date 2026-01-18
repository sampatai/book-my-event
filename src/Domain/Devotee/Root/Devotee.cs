using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common.Enums;
using Domain.Devotee.Entities;
using Domain.Devotee.Events;
using Domain.ValueObjects;
using SharedKernel;

namespace Domain.Devotee.Root;

public class Devotee : AuditableEntity, IAggregateRoot
{
    public readonly List<DevoteeVerification> _verifications = new();
    public Guid DevoteeId { get; private set; }

    public long UserId { get; private set; }

    public string FullName { get; private set; }

    public Address Address { get; private set; }
    public VerificationState VerificationState { get; private set; }

    public IEnumerable<DevoteeVerification> Verifications => _verifications.AsReadOnly();


    protected Devotee() { }

    public Devotee(

        long userId,
        string fullName,
        string street,
        string state, string postcode, string country, string city,
        string timezone, string addressLine1, string addressLine2,
        VerificationState verificationState)
    {

        Guard.Against.NullOrWhiteSpace(fullName, nameof(fullName));

        UserId = userId;
        FullName = fullName;
        Address = new Address(
            street,
            state,
            postcode,
            country,
            city, addressLine1, addressLine2, timezone);
        DevoteeId = Guid.NewGuid();
        VerificationState = verificationState;
        Raise(new DevoteeCreatedEvent(this.DevoteeId, userId)); 
            }


    public void UpdateName(string fullName)
    {
        Guard.Against.NullOrWhiteSpace(fullName, nameof(fullName));
        FullName = fullName;
        Raise(new DevoteeUpdateEvent(this.DevoteeId, this.UserId));
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

    public void SetVerificationState(VerificationState newState)
    {
        VerificationState = newState;
        Raise(new DevoteeVerificationEvent(this.DevoteeId, newState));
    }

    public void AddVerification(string documentPath, string documentName)
    {
        var verification = new DevoteeVerification(documentPath, documentName);
        _verifications.Add(verification);
    }
    public void RemoveVerification(Guid verificationId)
    {
        var verification = _verifications.FirstOrDefault(v => v.VerificationId == verificationId);
        if (verification != null)
        {
            _verifications.Remove(verification);
        }
    }
    public void SetVerifications(Guid verificationId, string documentPath,
        string documentName
        )
    {
        var verification = _verifications.FirstOrDefault(v => v.VerificationId == verificationId);
        if (verification != null)
        {
            verification.SetVerification(documentPath, documentName);
            verification.MarkAsNotVerified();
        }
    }
    public void SetDocumentVerify(Guid verificationId)
    {
        var verification = _verifications.FirstOrDefault(v => v.VerificationId == verificationId);
        if (verification != null)
        {
            verification.MarkAsVerified();
        }
    }
}

