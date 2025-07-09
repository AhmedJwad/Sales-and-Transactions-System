using System.Runtime.InteropServices;

namespace Sale.Api.Helpers
{
    public interface IRuntimeInformationWrapper
    {
        bool IsOSPlatform(OSPlatform osPlatform);
    }
}
