using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.DTOs;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrintController : ControllerBase
    {
        private readonly EtiDbContext _context;
        private readonly ILogger<PrintController> _logger;

        public PrintController(EtiDbContext context, ILogger<PrintController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Procesa una solicitud de impresión (POST /print)
        /// </summary>
        [HttpPost("print")]
        public async Task<IActionResult> CreatePrintRequest([FromBody] PrintRequestDto requestDto)
        {
            try
            {
                if (requestDto == null)
                    return BadRequest(new { message = "El cuerpo de la solicitud no puede estar vacío." });

                // Validar campos requeridos
                if (string.IsNullOrWhiteSpace(requestDto.RequestNumber) ||
                    string.IsNullOrWhiteSpace(requestDto.DeliveryType) ||
                    requestDto.Data == null)
                {
                    return BadRequest(new { message = "Faltan datos obligatorios en la solicitud." });
                }

                // Map DTO → Entity
                var printRequest = new PrintRequest
                {
                    RequestNumber = requestDto.RequestNumber,
                    DeliveryType = requestDto.DeliveryType,
                    Notes = requestDto.Notes,
                    Data = new ClientData
                    {
                        TransactionId = requestDto.Data.TransactionId,
                        ClientName = requestDto.Data.ClientName,
                        ClientCode = requestDto.Data.ClientCode,
                        AddressLine1 = requestDto.Data.AddressLine1,
                        AddressLine2 = requestDto.Data.AddressLine2,
                        AddressLine3 = requestDto.Data.AddressLine3,
                        PhoneNumber = requestDto.Data.PhoneNumber,
                        Attent = requestDto.Data.Attent
                    },
                    RequestFiles = requestDto.RequestFiles?.Select(f => new RequestFile
                    {
                        Type = f.FileType,
                        Name = f.FileName,
                        TotalLabels = f.TotalLabels,
                        Url = f.Url
                    }).ToList() ?? new List<RequestFile>()
                };

                _context.PrintRequests.Add(printRequest);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetPrintRequestTrace),
                    new { printRequestId = printRequest.PrintRequestId },
                    new
                    {
                        message = "Solicitud de impresión procesada exitosamente",
                        printRequestId = printRequest.PrintRequestId
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar la solicitud de impresión");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Procesa una reimpresión de etiquetas (POST /reprint)
        /// </summary>
        [HttpPost("reprint")]
        public async Task<IActionResult> ReprintRequest([FromBody] PrintRequestDto reprintDto)
        {
            try
            {
                if (reprintDto == null)
                    return BadRequest(new { message = "El cuerpo de la solicitud no puede estar vacío." });

                if (string.IsNullOrWhiteSpace(reprintDto.RequestNumber))
                    return BadRequest(new { message = "Debe especificarse el número de pedido para reimpresión." });

                // Buscar la solicitud original
                var existingRequest = await _context.PrintRequests
                    .Include(r => r.RequestFiles)
                    .FirstOrDefaultAsync(r => r.RequestNumber == reprintDto.RequestNumber);

                if (existingRequest == null)
                    return NotFound(new { message = $"No se encontró la solicitud con número {reprintDto.RequestNumber}" });

                // Registrar reimpresión como nuevo registro
                var reprint = new PrintRequest
                {
                    RequestNumber = reprintDto.RequestNumber,
                    DeliveryType = existingRequest.DeliveryType,
                    Notes = $"Reimpresión generada el {DateTime.Now:yyyy-MM-dd HH:mm:ss}",
                    Data = existingRequest.Data,
                    RequestFiles = existingRequest.RequestFiles
                        .Select(f => new RequestFile
                        {
                            Type = f.Type,
                            Name = f.Name,
                            TotalLabels = f.TotalLabels,
                            Url = f.Url
                        }).ToList()
                };

                _context.PrintRequests.Add(reprint);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetPrintRequestTrace),
                    new { printRequestId = reprint.PrintRequestId },
                    new
                    {
                        message = "Solicitud de reimpresión procesada exitosamente",
                        printRequestId = reprint.PrintRequestId
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar la reimpresión");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Consulta el estado de una solicitud de impresión (GET /trace/{printRequestId})
        /// </summary>
        [HttpGet("trace/{printRequestId:guid}")]
        public async Task<IActionResult> GetPrintRequestTrace(Guid printRequestId)
        {
            try
            {
                var request = await _context.PrintRequests
                    .Include(r => r.RequestFiles)
                    .FirstOrDefaultAsync(r => r.PrintRequestId == printRequestId);

                if (request == null)
                    return NotFound(new { message = "Solicitud de impresión no encontrada" });

                var dto = new PrintRequestDto
                {
                    RequestNumber = request.RequestNumber,
                    DeliveryType = request.DeliveryType,
                    Notes = request.Notes,
                    Data = new ClientDataDto
                    {
                        TransactionId = request.Data.TransactionId,
                        ClientName = request.Data.ClientName,
                        ClientCode = request.Data.ClientCode,
                        AddressLine1 = request.Data.AddressLine1,
                        AddressLine2 = request.Data.AddressLine2,
                        AddressLine3 = request.Data.AddressLine3,
                        PhoneNumber = request.Data.PhoneNumber,
                        Attent = request.Data.Attent
                    },
                    RequestFiles = request.RequestFiles.Select(f => new RequestFileDto
                    {
                        Type = f.Type,
                        Name = f.Name,
                        TotalLabels = f.TotalLabels,
                        Url = f.Url
                    }).ToList()
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el estado de la solicitud");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}
