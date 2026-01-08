using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Common.Enums
{
    public class VerificationState : Enumeration
    {
        public static readonly VerificationState Pending = new(1, "Pending");
        public static readonly VerificationState Verified = new(2, "Verified");
        public static readonly VerificationState Rejected = new(3, "Rejected");
        public static readonly VerificationState UnVerified = new(4, "UnVerified");

        public VerificationState(int id, string name) : base(id, name)
        {
        }
    }
}
