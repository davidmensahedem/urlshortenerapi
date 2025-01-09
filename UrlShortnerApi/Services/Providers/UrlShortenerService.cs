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

        public async Task<ApiResponse<string>> GenerateShortUrl(string url, HttpRequest request)
        {
            try
            {
                if (!Uri.TryCreate(url, UriKind.Absolute, out _))
                {
                    return new ApiResponse<string>
                    {
                        Code = StatusCodes.Status400BadRequest.ToString(),
                        Message = "Invalid URL specified."
                    };
                }

                var urlCode = GenerateUrlCode();

                var codeExists = await _dbContex.UrlShortners.AnyAsync(u => u.Code!.Equals(urlCode));

                if (codeExists)
                {
                    return new ApiResponse<string>
                    {
                        Code = StatusCodes.Status409Conflict.ToString(),
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
                        Code = StatusCodes.Status424FailedDependency.ToString(),
                        Message = "Couldn't create url. Kindly try again"
                    };
                }

                return new ApiResponse<string>
                {
                    Code = StatusCodes.Status201Created.ToString(),
                    Message = "Successful",
                    Data = shortUrl.ShortUrl
                };
            }
            catch (Exception e)
            {

                return new ApiResponse<string>
                {
                    Code = StatusCodes.Status500InternalServerError.ToString(),
                    Message = "An error occured while creating the short url"
                };
            }
        }

        public async Task<ApiResponse<string>> RedirectUrl(string code)
        {
            try
            {
                var existingShortUrl = await _dbContex.UrlShortners.FirstOrDefaultAsync(u => u.Code!.Equals(code));

                if (existingShortUrl is null)
                {
                    return new ApiResponse<string>
                    {
                        Code = StatusCodes.Status400BadRequest.ToString(),
                        Message = "Invalid url code provided"
                    };
                }

                return new ApiResponse<string>
                {
                    Code = StatusCodes.Status200OK.ToString(),
                    Message = "Successful",
                    Data = existingShortUrl.LongUrl
                };
            }
            catch (Exception e)
            {

                return new ApiResponse<string>
                {
                    Code = StatusCodes.Status500InternalServerError.ToString(),
                    Message = "An error occured while redirecting url"
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
