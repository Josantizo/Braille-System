using BrailleSuite.Api.Shared.Common;
using BrailleSuite.Api.Shared.Infrastructure;
using BrailleSuite.Api.Shared.Infrastructure.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BrailleSuite.Api.Features.Auth.Login;

public record LoginCommand(string UsernameOrEmail, string Password) : IRequest<LoginResult>;

public record LoginResult(string Token, long UserId, string Username);

public class LoginCommandHandler(AppDbContext db, ITokenService tokenService) : IRequestHandler<LoginCommand, LoginResult>
{
    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await db.Users.FirstOrDefaultAsync(
            u => u.Username == request.UsernameOrEmail || u.Email == request.UsernameOrEmail,
            cancellationToken);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new AuthenticationFailedException("Invalid username/email or password.");
        }

        var token = tokenService.CreateToken(user);
        return new LoginResult(token, user.Id, user.Username);
    }
}
