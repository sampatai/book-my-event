using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Pandit.Entities
{
    public class PujaType : AuditableEntity, IAggregateRoot
    {

        public Guid PujaTypeId { get; private set; }
        public string Name { get; private set; }
        public string? Description { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsRecurring { get; private set; }
        protected PujaType() { }

        internal PujaType(
            string name,
            bool isRecurring,
            string? description = null
            )
        {
            SetName(name);
            SetDescription(description);
            SetIsRecurring(isRecurring);
            Activate();
            PujaTypeId = Guid.NewGuid();

        }

        internal void SetName(string name)
        {
            Guard.Against.NullOrWhiteSpace(name, nameof(name));
            Guard.Against.OutOfRange(name.Length, nameof(name), 1, 100);
            Name = name;
        }

        internal void SetDescription(string? description)
        {
            if (!string.IsNullOrWhiteSpace(description))
            {
                Guard.Against.OutOfRange(description.Length, nameof(description), 0, 500);
            }

            Description = description;
        }
        internal void Activate()
        {
            IsActive = true;
        }
        internal void Deactivate()
        {
            IsActive = false;
        }

        internal void SetIsRecurring(bool isRecurring) =>
            IsRecurring = isRecurring;

    }
}
