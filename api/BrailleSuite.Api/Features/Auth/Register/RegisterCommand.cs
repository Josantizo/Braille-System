using BrailleSuite.Api.Shared.Common;
using BrailleSuite.Api.Shared.Entities;
using BrailleSuite.Api.Shared.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BrailleSuite.Api.Features.Auth.Register;

public record RegisterCommand(string Username, string Email, string Password) : IRequest<RegisterResult>;

public record RegisterResult(long UserId, string Username, string Email);

public class RegisterCommandHandler(AppDbContext db) : IRequestHandler<RegisterCommand, RegisterResult>
{
    public async Task<RegisterResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Email))
        {
            throw new ValidationException("Username and email are required.");
        }

        if (request.Password.Length < 8)
        {
            throw new ValidationException("Password must be at least 8 characters long.");
        }

        var exists = await db.Users
            .AnyAsync(u => u.Username == request.Username || u.Email == request.Email, cancellationToken);

        if (exists)
        {
            throw new ConflictException("A user with that username or email already exists.");
        }

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        db.Users.Add(user);
        await db.SaveChangesAsync(cancellationToken);

        return new RegisterResult(user.Id, user.Username, user.Email);
    }
}
