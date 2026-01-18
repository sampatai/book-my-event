using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common.Enums;
using Domain.Common.ValueObjects;
using Domain.Devotee.Entities;
using Domain.Pandit.Entities;
using Domain.ValueObjects;

namespace Domain.Pandit.Root
{
    public class Pandit : AuditableEntity, IAggregateRoot
    {
        public Guid PanditId { get; private set; }
        public long UserId { get; private set; }
        public string FullName { get; private set; }
        public Address Address { get; private set; }

        public string Languages { get; private set; }

        public int ExperienceInYears { get; private set; }

        public VerificationState VerificationState { get; private set; }

        public decimal? AverageRating { get; private set; }
        public readonly List<PanditVerification> _verifications = new();
        public IEnumerable<PanditVerification> Verifications => _verifications.AsReadOnly();

        public readonly List<PujaType> _pujaTypes = new();
        public IEnumerable<PujaType> PujaTypes => _pujaTypes.AsReadOnly();

        public readonly List<Review> _reviews = new();
        public IEnumerable<Review> Reviews => _reviews.AsReadOnly();
        protected Pandit() { }
        public Pandit(
            long userId,
            string fullName,
            string languages,
            int experienceInYears)
        {
            Guard.Against.NullOrWhiteSpace(fullName, nameof(fullName));
            Guard.Against.NullOrWhiteSpace(languages, nameof(languages));
            Guard.Against.NegativeOrZero(experienceInYears, nameof(experienceInYears));
            UserId = userId;
            FullName = fullName;
            PanditId = Guid.NewGuid();
            Languages = languages;
            ExperienceInYears = experienceInYears;


        }
        public void SetPandit(string fullName,
             string languages,
            int experienceInYears)
        {
            Guard.Against.NullOrWhiteSpace(fullName, nameof(fullName));
            Guard.Against.NullOrWhiteSpace(languages, nameof(languages));
            Guard.Against.NegativeOrZero(experienceInYears, nameof(experienceInYears));
            FullName = fullName;
        }

        public void SetAddress(
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
        public void VerifyPandit()
        {
            VerificationState = VerificationState.Verified;
        }
        public void AddVerification(PanditVerification verification)
        {
            Guard.Against.Null(verification, nameof(verification));
            _verifications.Add(verification);
        }
        public void SetRating(decimal newAverageRating)
        {
            Guard.Against.Negative(newAverageRating, nameof(newAverageRating));
            AverageRating = newAverageRating;
        }

        public void SetVerificationState(VerificationState newState)
        {
            VerificationState = newState;
        }

        public void AddVerification(string documentPath, string documentName)
        {
            var verification = new PanditVerification(documentPath, documentName);
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

        public void AddPujaType(string name,
           
            string? description,
            
            bool isRecurring)
        {
            var pujaType = new PujaType(
                name,
               
                isRecurring,
                description
                );
            pujaType.Activate();
            Guard.Against.Null(pujaType, nameof(pujaType));
            _pujaTypes.Add(pujaType);
        }
        public void RemovePujaType(Guid pujaTypeId)
        {
            var pujaType = _pujaTypes.FirstOrDefault(p => p.PujaTypeId == pujaTypeId);
            if (pujaType != null)
            {
                _pujaTypes.Remove(pujaType);
            }
        }
        public void SetPujaType(Guid pujaTypeId, string name,
            string? description = null,
            bool isActive = true)
        {
            var pujaType = _pujaTypes.FirstOrDefault(p => p.PujaTypeId == pujaTypeId);
            if (pujaType != null)
            {
                pujaType.SetName(name);
                pujaType.SetDescription(description);
                if (isActive)
                {
                    pujaType.Activate();
                }
                else
                {
                    pujaType.Deactivate();
                }
            }
        }
        public void AddReview(int rating,
            string? comment = null)
        {
            var review = new Review(rating, comment);
           
            _reviews.Add(review);
        }

        public void RemoveReview(Guid reviewId)
        {
            var review = _reviews.FirstOrDefault(r => r.ReviewId == reviewId);
            if (review != null)
            {
                _reviews.Remove(review);
            }
        }
        public void SetReview(Guid reviewId, int rating,
            string? comment = null)
        {
            var review = _reviews.FirstOrDefault(r => r.ReviewId == reviewId);
            if (review != null)
            {
                review.SetRating(rating);
                review.SetComment(comment);
            }
        }
    }
}
