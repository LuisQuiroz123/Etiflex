using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EtiWeb.Data;
using System.Linq;
using System.Threading.Tasks;

namespace EtiWeb.Areas.ApiArea.Controllers
{
    [Area("ApiArea")]
    public class RequestsController : Controller
    {
        private readonly EtiContext _context;

        public RequestsController(EtiContext context)
        {
            _context = context;
        }

        // Vista principal
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.Requests.ToListAsync());
        //}

        public async Task<IActionResult> Index()
        {
            var data = await _context.vw_RequestDetails.ToListAsync();
            return View(data);
        }




        // Opcional: detalles de un Request
        public async Task<IActionResult> Details(Guid id)
        {
            var request = await _context.Requests
                .FirstOrDefaultAsync(r => r.PrintRequestId == id);
            if (request == null) return NotFound();
            return View(request);
        }

        // Opcional: Create
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RequestNumber,DeliveryType,ClientName,ClientCode,AddressLine1,AddressLine2,AddressLine3,PhoneNumber,Attent,Notes")] Request request)
        {
            if (ModelState.IsValid)
            {
                _context.Add(request);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(request);
        }

        // Opcional: Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var request = await _context.Requests.FindAsync(id);
            if (request == null) return NotFound();
            return View(request);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,RequestNumber,DeliveryType,ClientName,ClientCode,AddressLine1,AddressLine2,AddressLine3,PhoneNumber,Attent,Notes")] Request request)
        {
            if (id != request.PrintRequestId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(request);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Requests.Any(e => e.PrintRequestId == request.PrintRequestId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(request);
        }

        // Opcional: Delete
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null) return NotFound();
            var request = await _context.Requests.FirstOrDefaultAsync(m => m.PrintRequestId == id);
            if (request == null) return NotFound();
            return View(request);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var request = await _context.Requests.FindAsync(id);
            if (request != null) _context.Requests.Remove(request);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
