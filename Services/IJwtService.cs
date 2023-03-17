using Entities.Users;

namespace Services
{
    public interface IJwtService
    {
        string Generate(User user);
    }
}