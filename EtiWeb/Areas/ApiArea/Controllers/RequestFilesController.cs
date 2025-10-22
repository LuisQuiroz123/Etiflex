using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EtiWeb.Data;


namespace EtiWeb.Areas.ApiArea.Controllers
{
    [Area("ApiArea")]
    public class RequestFilesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RequestFilesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ApiArea/RequestFiles
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.RequestFile.Include(r => r.Request);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ApiArea/RequestFiles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var requestFile = await _context.RequestFile
                .Include(r => r.Request)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (requestFile == null)
            {
                return NotFound();
            }

            return View(requestFile);
        }

        // GET: ApiArea/RequestFiles/Create
        public IActionResult Create()
        {
            ViewData["RequestId"] = new SelectList(_context.Request, "Id", "AddressLine1");
            return View();
        }

        // POST: ApiArea/RequestFiles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PrintRequestId,Type,Name,TotalLabels,Url")] RequestFile requestFile)
        {
            if (ModelState.IsValid)
            {
                _context.Add(requestFile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PrintRequestId"] = new SelectList(_context.Request, "Id", "AddressLine1", requestFile.PrintRequestId);
            return View(requestFile);
        }

        // GET: ApiArea/RequestFiles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var requestFile = await _context.RequestFile.FindAsync(id);
            if (requestFile == null)
            {
                return NotFound();
            }
            ViewData["PrintRequestId"] = new SelectList(_context.Request, "Id", "ClientData_AddressLine1", requestFile.PrintRequestId);
            return View(requestFile);
        }

        // POST: ApiArea/RequestFiles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PrintRequestId,Type,Name,TotalLabels,Url")] RequestFile requestFile)
        {
            if (id != requestFile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(requestFile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RequestFileExists(requestFile.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["RequestId"] = new SelectList(_context.Request, "Id", "AddressLine1", requestFile.PrintRequestId);
            return View(requestFile);
        }

        // GET: ApiArea/RequestFiles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var requestFile = await _context.RequestFile
                .Include(r => r.Request)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (requestFile == null)
            {
                return NotFound();
            }

            return View(requestFile);
        }

        // POST: ApiArea/RequestFiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var requestFile = await _context.RequestFile.FindAsync(id);
            if (requestFile != null)
            {
                _context.RequestFile.Remove(requestFile);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RequestFileExists(int id)
        {
            return _context.RequestFile.Any(e => e.Id == id);
        }
    }
}
