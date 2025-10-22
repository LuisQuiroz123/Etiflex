using EtiWeb.Data;
using EtiWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

[Area("ApiArea")]
[Route("ApiArea/[controller]/[action]")]
public class PrintController : Controller
{
    private readonly EtiContext _context;
    private readonly PrinterService _printerService;

    public PrintController(EtiContext context, PrinterService printerService)
    {
        _context = context;
        _printerService = printerService;
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> Preview(Guid id)
    {
        var request = await _context.Requests
            .Include(r => r.Files)
            .FirstOrDefaultAsync(r => r.PrintRequestId == id);

        if (request == null)
            return NotFound();

        var model = new PrintLabelViewModel
        {
            PrintRequestId = id
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Preview(PrintLabelViewModel model)
    {
        var request = await _context.Requests
            .Include(r => r.Files)
            .FirstOrDefaultAsync(r => r.PrintRequestId == model.PrintRequestId);

        if (request == null)
            return NotFound();

        var generator = new LabelZPL();
        string zpl;

        // Selección de marca
        switch (model.SelectedPrinterBrand.ToLower())
        {
            case "zebra":
                zpl = generator.GenerateZpl(request);
                break;
            case "sato":
                zpl = generator.GenerateZplForSato(request);
                break;
            case "godex":
                zpl = generator.GenerateZplForGodex(request);
                break;
            default:
                zpl = generator.GenerateZpl(request);
                break;
        }

        model.ZplCode = zpl;
        return View(model);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> Print(Guid id, string printerBrand = "Zebra", bool enviarImpresora = false)
    {
        var request = await _context.Requests
            .Include(r => r.Files)
            .FirstOrDefaultAsync(r => r.PrintRequestId == id);

        if (request == null)
            return NotFound();

        var generator = new LabelZPL();
        string zpl;

        switch (printerBrand.ToLower())
        {
            case "zebra":
                zpl = generator.GenerateZpl(request);
                break;
            case "sato":
                zpl = generator.GenerateZplForSato(request);
                break;
            case "godex":
                zpl = generator.GenerateZplForGodex(request);
                break;
            default:
                zpl = generator.GenerateZpl(request);
                break;
        }

        if (enviarImpresora)
        {
            // Envía a la impresora usando tu servicio
            bool success = await _printerService.PrintAsync(printerBrand, zpl);
            if (!success)
                return BadRequest("Error al enviar la etiqueta a la impresora.");
            return Ok();
        }

        // Descarga ZPL como archivo
        return File(Encoding.UTF8.GetBytes(zpl), "text/plain", $"{request.RequestNumber}.zpl");
    }
}







//using EtiWeb.Services;
//using Microsoft.AspNetCore.Mvc;
//using System.Text;
//using EtiWeb.Data;


//namespace EtiWeb.Areas.ApiArea.Controllers
//{
//    [Area("ApiArea")]
//    [Route("ApiArea/[controller]/[action]")]
//    public class PrintController : Controller
//    {
//        private readonly ApiService _apiService;

//        public PrintController(ApiService apiService)
//        {
//            _apiService = apiService;
//        }

//        [HttpGet("{id}")]
//        public async Task<IActionResult> Print(int id)
//        {
//            var request = await _apiService.GetRequestAsync(id);
//            if (request is null) return NotFound();

//            var generator = new LabelZPL();
//            string zpl = generator.GenerateZpl(request);

//            //return Content(zpl, "text/plain");
//            return File(Encoding.UTF8.GetBytes(zpl), "text/plain", $"Etiqueta_{id}.zpl");

//            //var generator = new LabelZPL();
//            //generator.SendToPrinter(zpl, "192.168.1.100"); // IP de la impresora
//            //return Ok("Etiqueta enviada a la impresora.");
//        }
//    }
//}
