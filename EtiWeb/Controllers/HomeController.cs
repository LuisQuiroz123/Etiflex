using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using EtiWeb.Models;
using EtiWeb.Data; // 👈 Importante para usar EtiContext

namespace EtiWeb.Controllers
{
    //[Authorize(Roles = "SuperAdmin")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly EtiContext _context;

        public HomeController(ILogger<HomeController> logger, EtiContext context)
        {
            _logger = logger;
            _context = context;
        }

       // [Authorize(Roles = "SuperAdmin")]
        public IActionResult Index()
        {
            // Totales
            ViewBag.TotalOrders = _context.orders.Count();
            ViewBag.TotalRequests = _context.Requests.Count();
            ViewBag.TotalSales = _context.SalesOrders.Sum(x => x.TransactionAmount ?? 0);

            // Ventas por mes
            var salesData = _context.SalesOrders
                .Where(s => s.CreatedAt != null)
                .GroupBy(s => new { s.CreatedAt.Year, s.CreatedAt.Month })
                .Select(g => new
                {
                    Month = g.Key.Month + "/" + g.Key.Year,
                    Total = g.Sum(x => x.TransactionAmount ?? 0)
                })
                .ToList();

            // Estados de órdenes
            var orderStates = _context.SalesOrders
                .GroupBy(s => s.OrderState ?? "Sin estado")
                .Select(g => new
                {
                    Estado = g.Key,
                    Total = g.Count()
                })
                .ToList();

            ViewBag.SalesByMonth = salesData;
            ViewBag.OrderStates = orderStates;

            return View();
        }
        [HttpGet]
        public IActionResult Dashboard()
        {
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
