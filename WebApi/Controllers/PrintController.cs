using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.DTOs;
using WebApi.DTOs.Print;
using WebApi.Services;

[ApiController]
[Route("print")]
public class PrintController : ControllerBase
{
    private readonly EtiDbContext _context;
   
    //private readonly SalesOrderService _salesOrderService; // servicio cerm sales orders

    //public PrintController(EtiDbContext context, SalesOrderService salesOrderService)
    //{
    //    _context = context;
    //    _salesOrderService = salesOrderService;
    //}
    public PrintController(EtiDbContext context)
    {
        _context = context;
    }

    // POST /print
    [HttpPost]
    [ProducesResponseType(typeof(PrintRequestDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreatePrintRequest([FromBody] PrintRequestDto dto)
    {
        if (dto == null || dto.ClientData == null || string.IsNullOrEmpty(dto.RequestNumber))
            return BadRequest(new { error = "BAD_REQUEST", message = "Datos incompletos o inválidos" });

        try
        {
            // Mapear DTO -> entidad
            var entity = new PrintRequest
            {
                RequestNumber = dto.RequestNumber,
                DeliveryType = dto.DeliveryType,
                Notes = dto.Notes,
                ClientData = new ClientData
                {
                    TransactionId = dto.ClientData.TransactionId != Guid.Empty
                        ? dto.ClientData.TransactionId
                        : Guid.NewGuid(),
                    ClientName = dto.ClientData.ClientName,
                    ClientCode = dto.ClientData.ClientCode,
                    AddressLine1 = dto.ClientData.AddressLine1,
                    AddressLine2 = dto.ClientData.AddressLine2,
                    AddressLine3 = dto.ClientData.AddressLine3,
                    PhoneNumber = dto.ClientData.PhoneNumber,
                    Attent = dto.ClientData.Attent
                },
                RequestFiles = dto.RequestFiles?.Select(f => new RequestFile
                {
                    Type = f.Type,
                    Name = f.Name,
                    TotalLabels = f.TotalLabels,
                    Url = f.Url
                }).ToList() ?? new List<RequestFile>()
            };

            _context.PrintRequests.Add(entity);
            await _context.SaveChangesAsync();

            //await _salesOrderService.SendOrderAsync(entity); //conectandonos a cerm


            return CreatedAtAction(nameof(GetPrintRequest), new { id = entity.PrintRequestId }, new { printRequestId = entity.PrintRequestId });
        }
        catch (DbUpdateException dbEx)
        {
            var innerMessage = dbEx.InnerException?.Message ?? dbEx.Message;
            return StatusCode(500, new
            {
                error = "INTERNAL_SERVER_ERROR",
                message = "Error al guardar los datos en la base de datos",
                details = innerMessage
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                error = "INTERNAL_SERVER_ERROR",
                message = ex.Message
            });
        }
    }

    // GET /print/{id}
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(List<PrintStatusDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPrintRequest(Guid id)
    {
        try
        {
            var request = await _context.PrintRequests
                .Include(r => r.RequestFiles)
                .FirstOrDefaultAsync(r => r.PrintRequestId == id);

            if (request == null)
                return NotFound(new { error = "NOT_FOUND", message = "Solicitud no encontrada" });

            // Mapeo a DTO
            var dto = new PrintRequestDto
            {
                RequestNumber = request.RequestNumber,
                DeliveryType = request.DeliveryType,
                Notes = request.Notes,
                ClientData = new ClientDataDto
                {
                    TransactionId = request.ClientData.TransactionId,
                    ClientName = request.ClientData.ClientName,
                    ClientCode = request.ClientData.ClientCode,
                    AddressLine1 = request.ClientData.AddressLine1,
                    AddressLine2 = request.ClientData.AddressLine2,
                    AddressLine3 = request.ClientData.AddressLine3,
                    PhoneNumber = request.ClientData.PhoneNumber,
                    Attent = request.ClientData.Attent
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
            return StatusCode(500, new { error = "INTERNAL_SERVER_ERROR", message = ex.Message });
        }
    }

    // GET /print/all
    [HttpGet("all")]
    public async Task<IActionResult> GetAllRequests()
    {
        try
        {
            var requests = await _context.PrintRequests
                .Include(r => r.RequestFiles)
                .ToListAsync();

            // Mapear a DTO
            var dtoList = requests.Select(request => new PrintRequestDto
            {
                RequestNumber = request.RequestNumber,
                DeliveryType = request.DeliveryType,
                Notes = request.Notes,
                ClientData = new ClientDataDto
                {
                    TransactionId = request.ClientData.TransactionId,
                    ClientName = request.ClientData.ClientName,
                    ClientCode = request.ClientData.ClientCode,
                    AddressLine1 = request.ClientData.AddressLine1,
                    AddressLine2 = request.ClientData.AddressLine2,
                    AddressLine3 = request.ClientData.AddressLine3,
                    PhoneNumber = request.ClientData.PhoneNumber,
                    Attent = request.ClientData.Attent
                },
                RequestFiles = request.RequestFiles.Select(f => new RequestFileDto
                {
                    Type = f.Type,
                    Name = f.Name,
                    TotalLabels = f.TotalLabels,
                    Url = f.Url
                }).ToList()
            }).ToList();

            return Ok(dtoList);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                error = "INTERNAL_SERVER_ERROR",
                message = "Ocurrió un error interno en el servidor",
                details = ex.Message
            });
        }
    }


    // GET /print/by-client/{clientCode}
    [HttpGet("by-client/{clientCode}")]
    public async Task<IActionResult> GetRequestsByClient(string clientCode)
    {
        if (string.IsNullOrEmpty(clientCode))
            return BadRequest(new { error = "BAD_REQUEST", message = "Código de cliente requerido" });

        try
        {
            // Traer todos los registros con sus RequestFiles
            var list = await _context.PrintRequests
                .Include(r => r.RequestFiles)
                .ToListAsync();

            // Filtrar en memoria usando ClientData
            var filtered = list
                .Where(r => r.ClientData.ClientCode == clientCode)
                .Select(r => new PrintRequestDto
                {
                    RequestNumber = r.RequestNumber,
                    DeliveryType = r.DeliveryType,
                    Notes = r.Notes,
                    ClientData = new ClientDataDto
                    {
                        TransactionId = r.ClientData.TransactionId,
                        ClientName = r.ClientData.ClientName,
                        ClientCode = r.ClientData.ClientCode,
                        AddressLine1 = r.ClientData.AddressLine1,
                        AddressLine2 = r.ClientData.AddressLine2,
                        AddressLine3 = r.ClientData.AddressLine3,
                        PhoneNumber = r.ClientData.PhoneNumber,
                        Attent = r.ClientData.Attent
                    },
                    RequestFiles = r.RequestFiles.Select(f => new RequestFileDto
                    {
                        Type = f.Type,
                        Name = f.Name,
                        TotalLabels = f.TotalLabels,
                        Url = f.Url
                    }).ToList()
                })
                .ToList();

            if (!filtered.Any())
                return NotFound(new { error = "NOT_FOUND", message = "No se encontraron solicitudes para este cliente" });

            return Ok(filtered);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "INTERNAL_SERVER_ERROR", message = ex.Message });
        }
    }


    [HttpPost("reprint/{id}")]
    [ProducesResponseType(typeof(PrintRequestDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ReprintPrintRequest(Guid id)
    {
        try
        {
            // Buscar el pedido original
            var original = await _context.PrintRequests
                .Include(r => r.RequestFiles)
                .FirstOrDefaultAsync(r => r.PrintRequestId == id);

            if (original == null)
                return NotFound(new { error = "NOT_FOUND", message = "Solicitud original no encontrada" });

            // Crear un nuevo PrintRequest con la misma información
            var newRequest = new PrintRequest
            {
                PrintRequestId = Guid.NewGuid(), 
                RequestNumber = original.RequestNumber, 
                DeliveryType = original.DeliveryType,
                ClientData = new ClientData
                {
                    TransactionId = Guid.NewGuid(), 
                    ClientName = original.ClientData.ClientName,
                    ClientCode = original.ClientData.ClientCode,
                    AddressLine1 = original.ClientData.AddressLine1,
                    AddressLine2 = original.ClientData.AddressLine2,
                    AddressLine3 = original.ClientData.AddressLine3,
                    PhoneNumber = original.ClientData.PhoneNumber,
                    Attent = original.ClientData.Attent
                },
                Notes = original.Notes,
                RequestFiles = original.RequestFiles.Select(f => new RequestFile
                {
                    Type = f.Type,
                    Name = f.Name,
                    TotalLabels = f.TotalLabels,
                    Url = f.Url
                }).ToList()
            };

            _context.PrintRequests.Add(newRequest);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPrintRequest), new { id = newRequest.PrintRequestId },
                new { printRequestId = newRequest.PrintRequestId });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "INTERNAL_SERVER_ERROR", message = ex.Message });
        }
    }

    [HttpGet("{id}/status")]
    [ProducesResponseType(typeof(List<PrintStatusDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPrintStatus(Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest(new { error = "BAD_REQUEST", message = "ID inválido" });

        try
        {
            var statusList = await _context.PrintStatuses
                .Where(s => s.PrintRequestId == id)
                .Select(s => new PrintStatusDto
                {
                    Date = s.Date,
                    Status = s.Status,
                    Code = s.Code,
                    Data = new TrackingDataDto
                    {
                        TrackingId = s.TrackingId
                    }
                })
                .ToListAsync();

            if (!statusList.Any())
                return NotFound(new { error = "NOT_FOUND", message = "No se encontraron estados para esta solicitud" });

            return Ok(statusList);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "INTERNAL_SERVER_ERROR", message = ex.Message });
        }
    }


}
