﻿using Domain.Users.Root;

namespace Application.Abstractions.Authentication;

public interface ITokenProvider
{
    string Create(User user);
}
