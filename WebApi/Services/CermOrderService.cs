using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Serilog;
using WebApi.DTOs.SalesOrder;

namespace WebApi.Services
{
    public class CermOrderService
    {
        private readonly HttpClient _httpClient;

        public CermOrderService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> SendOrderAsync(SalesOrderDto order)
        {
            try
            {
                // POST a la API de CERM
                var response = await _httpClient.PostAsJsonAsync("endpoint/de/ventas", order);

                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsStringAsync();
                return result;
            }
            catch (HttpRequestException ex)
            {
                Log.Error(ex, "Error enviando la orden a CERM");
                throw;
            }
        }
    }
}
