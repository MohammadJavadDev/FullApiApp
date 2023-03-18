using Entities.Users;

namespace Services.SdkServices
{
    public interface ISdk
    {
        CurrentUser CurrentUser { get; }
    }
}