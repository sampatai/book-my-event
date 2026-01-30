using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Pandit.Entities
{
    public class Review : AuditableEntity, IAggregateRoot
    {
        public Guid ReviewId { get; private set; }
       

        public int Rating { get; private set; }          // 1-5
        public string? Comment { get; private set; }     // nullable

        protected Review() { }

        internal Review(int rating,
            string? comment = null)
        {
            SetRating(rating);
            SetComment(comment);
            ReviewId = Guid.NewGuid();
        }
        internal void SetRating(int rating)
        {
            Guard.Against.OutOfRange(rating, nameof(rating), 1, 5);
            Rating = rating;
        }

        internal void SetComment(string? comment)
        {
            // Comment is nullable; only enforce max length if provided.
            if (!string.IsNullOrWhiteSpace(comment))
            {
                Guard.Against.OutOfRange(comment.Length, nameof(comment), 1, 4000);
            }

            Comment = comment;
        }
    }
}
