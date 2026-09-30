namespace BrailleSuite.Api.Shared.Common;

public class CurrentUserNotProvidedException(string message) : Exception(message);
