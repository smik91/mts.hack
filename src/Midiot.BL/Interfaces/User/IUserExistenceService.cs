namespace Midiot.BL.Services.User;

public interface IUserExistenceService
{
    Task EnsureUserDoesNotExist(string username);
}
