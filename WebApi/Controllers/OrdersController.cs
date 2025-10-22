using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdenesController : ControllerBase
    {
        private readonly EtiContext _context;
        private readonly ILogger<OrdenesController> _logger;

        public OrdenesController(EtiContext context, ILogger<OrdenesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ✅ POST: aceptar lista u objeto
        //[HttpPost]
        //public IActionResult Post([FromBody] object body)
            [HttpPost]
        public IActionResult Post([FromBody] List<Orders> body)
        {
            if (body is null)
                return BadRequest(new { message = "El cuerpo de la solicitud está vacío." });

            var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            List<Orders> masters = new();

            try
            {
                var json = body.ToString();

                if (json!.TrimStart().StartsWith("["))
                {
                    masters = System.Text.Json.JsonSerializer.Deserialize<List<Orders>>(json, options) ?? new();
                }
                else
                {
                    var master = System.Text.Json.JsonSerializer.Deserialize<Orders>(json, options);
                    if (master != null)
                        masters.Add(master);
                }
            }
            catch (System.Text.Json.JsonException)
            {
                return BadRequest(new { message = "JSON inválido. Envía un objeto o una lista de objetos con los campos requeridos." });
            }

            if (!masters.Any())
                return BadRequest(new { message = "No se recibieron registros válidos." });


            _context.orders.AddRange(masters);
            _context.SaveChanges();

            var response = new
            {
                operationId = Guid.NewGuid(),
                timestamp = DateTime.UtcNow,
                message = $"Se insertaron {masters.Count} registro(s).",
                data = masters
            };

            _logger.LogInformation("Operación {OperationId} | Se insertaron {Count} registro(s) en {Timestamp}",
                response.operationId, masters.Count, response.timestamp);

            return Ok(response);
        }

        // ✅ GET: obtener todas las órdenes
        [HttpGet]
        public IActionResult Get()
        {
            var orders = _context.orders.AsNoTracking().ToList();
            return Ok(orders);
        }

        // ✅ GET by Id
        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var master = _context.orders.Find(id);
            if (master == null)
                return NotFound(new { message = $"No se encontró el registro con Id {id}" });

            return Ok(master);
        }
    }
}