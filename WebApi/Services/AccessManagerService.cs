using WebApi.DTOs.Authentication;

namespace WebApi.Services
{
    public class AccessManagerService
    {
        private readonly HttpClient _httpClient;

        public AccessManagerService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<TokenResponse?> GetAccessTokenAsync(ApplicationCredentials credentials)
        {
            var response = await _httpClient.PostAsJsonAsync("token", credentials);

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<TokenResponse>();
            else
                throw new Exception(await response.Content.ReadAsStringAsync());
        }
    }
}
