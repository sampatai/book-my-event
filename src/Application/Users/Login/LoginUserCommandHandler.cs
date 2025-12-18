using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using Domain.Users;
using Domain.Users.Root;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Users.Login;

internal sealed class LoginUserCommandHandler(

    ITokenProvider tokenProvider) : ICommand<string>
{
    public async Task<Result<string>> Handle(string command, CancellationToken cancellationToken)
    {
        // 1. Lookup user (mocked here, replace with actual repo lookup)
        var user = new User(
            "",
             "",
            "",
             "John",
             "Doe",
             "User"
        );



        // 3. Generate JWT or access token
        string token = tokenProvider.Create(user);

        // 4. Return result
        return await Task.FromResult(Result.Success(token));
    }

}
