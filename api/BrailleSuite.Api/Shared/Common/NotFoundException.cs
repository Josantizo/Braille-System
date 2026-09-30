namespace BrailleSuite.Api.Shared.Common;

public class NotFoundException(string message) : Exception(message);
