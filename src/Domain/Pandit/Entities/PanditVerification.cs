using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Domain.Common.ValueObjects
{
    public class PanditVerification : AuditableEntity
    {

        public Guid VerificationId { get; private set; }

        public string DocumentPath { get; private set; }
        public string DocumentName { get; private set; }
        public bool IsVerified { get; private set; }

        internal PanditVerification(string documentPath, string documentName)
        {
            Guard.Against.NullOrWhiteSpace(documentPath, nameof(documentPath));
            Guard.Against.NullOrWhiteSpace(documentName, nameof(documentName));
            DocumentPath = documentPath;
            DocumentName = documentName;
        }
        internal void MarkAsVerified()
        {
            IsVerified = true;
        }

        internal void MarkAsNotVerified()
        {
            IsVerified = false;
        }

        internal void SetVerification(string documentPath, string documentName)
        {
            Guard.Against.NullOrWhiteSpace(documentPath, nameof(documentPath));
            Guard.Against.NullOrWhiteSpace(documentName, nameof(documentName));
            DocumentPath = documentPath;
            DocumentName = documentName;
        }

        protected PanditVerification()
        {
        }

    }
}
