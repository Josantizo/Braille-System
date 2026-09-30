namespace BrailleSuite.Api.Shared.Common;

public class AuthenticationFailedException(string message) : Exception(message);
