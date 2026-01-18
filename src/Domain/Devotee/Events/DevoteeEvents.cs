using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common.Enums;

namespace Domain.Devotee.Events
{
    public record DevoteeCreatedEvent(Guid DevoteeId, long userId) : IDomainEvent;
    public record DevoteeUpdateEvent(Guid DevoteeId, long userId) : IDomainEvent;
    public record DevoteeVerificationEvent(Guid DevoteeId, VerificationState verificationState) : IDomainEvent;

}
