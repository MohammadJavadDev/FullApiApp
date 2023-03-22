using Entities.Users;

namespace Services
{
    public interface IJwtService
    {
        Task<string> GenerateAsync(User user);
    }
}