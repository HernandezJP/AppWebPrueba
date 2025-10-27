using System.Net.Http.Headers;

namespace AppWebPrueba.Services
{
    public class ApiClientFactory
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _accessor;

        public ApiClientFactory(IHttpClientFactory factory, IHttpContextAccessor accessor)
        {
            _httpClientFactory = factory;
            _accessor = accessor;
        }

        public HttpClient Create()
        {
            var client = _httpClientFactory.CreateClient("Api");
            var token = _accessor.HttpContext?.Session.GetString("JWT");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }
    }
}
