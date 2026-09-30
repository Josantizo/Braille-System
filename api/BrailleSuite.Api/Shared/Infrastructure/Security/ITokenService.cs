using BrailleSuite.Api.Shared.Entities;

namespace BrailleSuite.Api.Shared.Infrastructure.Security;

public interface ITokenService
{
    string CreateToken(User user);
}
