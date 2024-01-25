using Microsoft.Extensions.Options;
using UrlShortnerApi.Options;

namespace UrlShortnerApi.Services.Providers
{
    public class UrlShortenerService : IUrlShortenerService
    {
        private readonly UrlShortingConfig _urlConfig;

        public UrlShortenerService(IOptions<UrlShortingConfig> urlConfig) => _urlConfig = urlConfig.Value;

    }
}
