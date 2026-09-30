using System.Security.Claims;

namespace BrailleSuite.Api.Shared.Common;

// Reads the current user id from the JWT's "sub" claim. Endpoints that use
// this depend only on ICurrentUserContext, so the token/claims details stay
// isolated here.
public class ClaimsCurrentUserContext : ICurrentUserContext
{
    private readonly long _userId;

    public ClaimsCurrentUserContext(IHttpContextAccessor httpContextAccessor)
    {
        var user = httpContextAccessor.HttpContext?.User
            ?? throw new CurrentUserNotProvidedException("No authenticated user for this request.");

        var subject = user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? user.FindFirstValue("sub")
            ?? throw new CurrentUserNotProvidedException("The access token is missing a 'sub' claim.");

        if (!long.TryParse(subject, out var userId))
        {
            throw new CurrentUserNotProvidedException("The 'sub' claim is not a valid user id.");
        }

        _userId = userId;
    }

    public long UserId => _userId;
}
