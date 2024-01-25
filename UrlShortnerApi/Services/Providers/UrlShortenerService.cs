namespace UrlShortnerApi.Services.Providers
{
    public class UrlShortenerService : IUrlShortenerService
    {
        private readonly UrlShortingConfig _urlConfig;
        private readonly ApplicationDbContext _dbContex;
        private readonly Random _random = new();

        public UrlShortenerService(
            IOptions<UrlShortingConfig> urlConfig,
            ApplicationDbContext dbContext) =>
            (_urlConfig, _dbContex) = (urlConfig.Value, dbContext);

        public async Task<ApiResponse<string>> GenerateShortUrl(string url,HttpRequest request)
        {
            try
            {
                if (!Uri.TryCreate(url, UriKind.Absolute, out _))
                {
                    return new ApiResponse<string>
                    {
                        Code = $"{StatusCodes.Status400BadRequest}",
                        Message = "Invalid URL specified."
                    };
                }

                var urlCode = GenerateUrlCode();

                var codeExists = await _dbContex.UrlShortners.AnyAsync(u => u.Code!.Equals(urlCode));

                if (codeExists)
                {
                    return new ApiResponse<string>
                    {
                        Code = $"{StatusCodes.Status409Conflict}",
                        Message = "Couldn't create url. Resource conflict occured."
                    };
                }

                var shortUrl = new UrlShortner
                {
                    LongUrl = url,
                    ShortUrl = $"{request.Scheme}://{request.Host}/api/{urlCode}",
                    Code = urlCode
                };

                await _dbContex.UrlShortners.AddAsync(shortUrl);

                var rowCount = await _dbContex.SaveChangesAsync();

                if (rowCount < 1)
                {
                    return new ApiResponse<string>
                    {
                        Code = $"{StatusCodes.Status424FailedDependency}",
                        Message = "Couldn't create url. Kindly try again"
                    };
                }

                return new ApiResponse<string>
                {
                    Code = $"{StatusCodes.Status201Created}",
                    Message = "Successful",
                    Data = shortUrl.ShortUrl
                };
            }
            catch (Exception e)
            {

                return new ApiResponse<string>
                {
                    Code = $"{StatusCodes.Status500InternalServerError}",
                    Message = "An error occured while creating the short url"
                };
            }
        }

        private string GenerateUrlCode()
        {
            var codeChars = new char[CoreConstants.ShortUrlCodeLength];

            for (int i = 0; i < CoreConstants.ShortUrlCodeLength; i++)
            {
                var randomIndex = _random.Next(_urlConfig.EncryptionValues!.Length! - 1);

                codeChars[i] = _urlConfig.EncryptionValues![randomIndex];
            }

            return new string(codeChars);
        }
    }
}
