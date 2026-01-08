using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Devotee.Entities
{
    public class DevoteeVerification : AuditableEntity
    {
        public Guid VerificationId { get; private set; } 
        public string DocumentPath { get; private set; }
        public string DocumentName { get; private set; }
        public bool IsVerified { get; private set; }
        internal DevoteeVerification(string documentPath, string documentName)
        {
            Guard.Against.NullOrWhiteSpace(documentPath, nameof(documentPath));
            Guard.Against.NullOrWhiteSpace(documentName, nameof(documentName));
            DocumentPath = documentPath;
            DocumentName = documentName;
            VerificationId = Guid.NewGuid();
        }
        internal void MarkAsVerified()
        {
            IsVerified = true;
        }
        internal void MarkAsNotVerified()
        {
            IsVerified = false;
        }

        internal void SetVerification(string documentPath,
            string documentName)
        {
            Guard.Against.NullOrWhiteSpace(documentPath, nameof(documentPath));
            Guard.Against.NullOrWhiteSpace(documentName, nameof(documentName));
            DocumentPath = documentPath;
            DocumentName = documentName;
           
        }

        protected DevoteeVerification()
        {
        }
        
    }
}
