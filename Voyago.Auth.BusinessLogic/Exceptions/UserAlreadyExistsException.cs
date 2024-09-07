namespace Voyago.Auth.BusinessLogic.Exceptions;
public class UserAlreadyExistsException(string? message) : Exception(message)
{
}
