using System;

using Auth.Domain.ValueObjects;
using SharedKernel;

namespace Auth.Domain.TenantEntity.Root
{
    public class TenantEntity : AuditableEntity, IAggregateRoot
    {
        protected TenantEntity() { }

        public Guid TenantEntityId { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public DateOnly StartDate { get; private set; }
        public DateOnly? EndDate { get; private set; }
        public string? ImageUrl { get; private set; }
        public bool IsActive { get; private set; }
        public string? ContactEmail { get; private set; }
        public string? PhoneNumber { get; private set; }
        public string? WebsiteUrl { get; private set; }
        public Address? Address { get; private set; }
        public string? TimeZone { get; private set; }

        public TenantEntity(
            string name,
            string description,
            DateOnly startDate,
            DateOnly? endDate,
            string? image,
            string? contactEmail,
            string? phoneNumber,
            string? website,
            Address? address,
            Guid tenantEntityId)
        {
            Name = Guard.Against.NullOrWhiteSpace(name);
            Description = description ?? string.Empty;
            StartDate = startDate;
            EndDate = endDate;
            ImageUrl = image;
            ContactEmail = contactEmail;
            PhoneNumber = phoneNumber;
            WebsiteUrl = website;
            Address = address;
            IsActive = true;
            TenantEntityId = tenantEntityId;
        }

        public void UpdateDetails(
            string name,
            string description,
            DateOnly startDate,
            DateOnly? endDate,
            string? image,
            string? contactEmail,
            string? phoneNumber,
            string? website,
            Address? address)
        {
            Name = Guard.Against.NullOrWhiteSpace(name);
            Description = description ?? string.Empty;
            StartDate = startDate;
            EndDate = endDate;
            ImageUrl = image;
            ContactEmail = contactEmail;
            PhoneNumber = phoneNumber;
            WebsiteUrl = website;
            Address = address;
        }

        public void Activate() => IsActive = true;
        public void Deactivate() => IsActive = false;
        public void SetImage(string? image) => ImageUrl = image;
        public void SetTimeZone(string? timeZone)
        {
            TimeZone = Guard.Against.NullOrWhiteSpace(timeZone);
        }
    }
}
