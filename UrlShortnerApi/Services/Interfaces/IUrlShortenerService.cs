namespace UrlShortnerApi.Services.Interfaces
{
    public interface IUrlShortenerService
    {
        Task<ApiResponse<string>> GenerateShortUrl(string url, HttpRequest request);
        Task<ApiResponse<string>> RedirectUrl(string code);
    }
}
