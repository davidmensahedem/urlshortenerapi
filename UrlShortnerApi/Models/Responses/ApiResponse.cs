namespace UrlShortnerApi.Models.Response
{
    public class ApiResponse<T> where T : class
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }
        public ApiResponse() { }
        public ApiResponse(string code, string message) => (Code,Message) = (code,message);
    }
}
