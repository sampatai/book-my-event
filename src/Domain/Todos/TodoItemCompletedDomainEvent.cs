﻿using SharedKernel;

namespace Domain.Todos;

public sealed record TodoItemCompletedDomainEvent(long TodoItemId) : IDomainEvent;
