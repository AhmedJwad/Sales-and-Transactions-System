using System.Runtime.InteropServices;

namespace Sale.Api.Helpers
{
    public class RuntimeInformationWrapper:IRuntimeInformationWrapper
    {
        public bool IsOSPlatform(OSPlatform osPlatform)
      => RuntimeInformation.IsOSPlatform(osPlatform);
    }
}
