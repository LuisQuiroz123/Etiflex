using System.Net.Http;
using System.Text;
using System.Text.Json;
using WebApi.DTOs.SalesOrder;
using WebApi.Data;

namespace WebApi.Services
{
    public class SalesOrderService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SalesOrderService> _logger;

        // URL base del sistema CERM o API externa
        private const string CermApiBaseUrl = "https://tuservidor-cerm/api/salesorders";

        public SalesOrderService(HttpClient httpClient, ILogger<SalesOrderService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        /// <summary>
        /// Envía un pedido de venta (SalesOrder) al sistema externo (CERM).
        /// </summary>
        public async Task<HttpResponseMessage> SendOrderAsync(SalesOrderDto salesOrder)
        {
            try
            {
                // Convertir el DTO plano en la estructura JSON esperada por CERM
                var payload = new
                {
                    ReferenceAtCustomer = salesOrder.ReferenceAtCustomer,
                    LineComment1 = salesOrder.LineComment1,
                    Delivery = new
                    {
                        Type = "Address",
                        Comment = salesOrder.LineComment1 ?? "No comments",
                        AddressId = salesOrder.AddressId,
                        ExpectedDate = salesOrder.ExpectedDate.ToString("yyyy-MM-dd")
                    },
                    InvoiceLines = new[]
                    {
                        new
                        {
                            InvoicePriceLineId = Guid.NewGuid().ToString(),
                            Description = $"Product {salesOrder.ProductId}",
                            Quantity = salesOrder.OrderQuantity,
                            UnitPrice = salesOrder.UnitPrice,
                            Type = "SalesOrder"
                        }
                    },
                    OrderQuantity = salesOrder.OrderQuantity,
                    ProductId = salesOrder.ProductId,
                    UnitPrice = salesOrder.UnitPrice,
                    Prepayment = new
                    {
                        TransactionId = salesOrder.TransactionId,
                        TransactionTime = salesOrder.TransactionTime.ToString("o"),
                        TransactionAmount = salesOrder.TransactionAmount,
                        TransactionMethod = salesOrder.TransactionMethod
                    }
                };

                var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                });

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _logger.LogInformation("Enviando SalesOrder a CERM: {json}", json);

                // Enviar el POST a CERM
                var response = await _httpClient.PostAsync(CermApiBaseUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("CERM devolvió un error ({StatusCode}): {Body}", response.StatusCode, errorBody);
                }

                return response;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error al enviar SalesOrder a CERM");
                throw; // Se maneja en el controlador
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al enviar SalesOrder a CERM");
                throw;
            }
        }
    }
}
