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
    public class SalesOrdersController : Controller
    {
        private readonly EtiContext _context;

        public SalesOrdersController(EtiContext context)
        {
            _context = context;
        }

        // GET: ApiArea/SalesOrders

   
            public async Task<IActionResult> Index()
        {
            var orders = await _context.SalesOrders
                .Include(o => o.Lines)
                .ToListAsync();

            var viewModel = orders.Select(o => new SalesOrderViewModel
            {
                SalesOrderId = o.SalesOrderId,
                ReferenceAtCustomer = o.ReferenceAtCustomer ?? "",
                ProductId = o.ProductId ?? "",
                OrderQuantity = o.OrderQuantity ?? 0,
                UnitPrice = o.UnitPrice ?? 0,
                TransactionAmount = o.TransactionAmount ?? 0,
                TransactionMethod = o.TransactionMethod ?? "",
                OrderState = o.OrderState ?? "",
                CreatedAt = o.CreatedAt,
                Lines = o.Lines.Select(l => new SalesOrderLineViewModel
                {
                    Description = l.Description,
                    Quantity = l.Quantity,
                    UnitPrice = l.UnitPrice
                }).ToList()
            }).ToList();

            return View(viewModel);
        }
    


        // GET: ApiArea/SalesOrders/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salesOrders = await _context.SalesOrders
                .FirstOrDefaultAsync(m => m.SalesOrderId == id);
            if (salesOrders == null)
            {
                return NotFound();
            }

            return View(salesOrders);
        }

        // GET: ApiArea/SalesOrders/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ApiArea/SalesOrders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SalesOrderId,ReferenceAtCustomer,LineComment1,ProductId,OrderQuantity,UnitPrice,AddressId,ExpectedDate,TransactionId,TransactionTime,TransactionAmount,TransactionMethod,OrderState,CreatedAt")] SalesOrders salesOrders)
        {
            if (ModelState.IsValid)
            {
                salesOrders.SalesOrderId = Guid.NewGuid();
                _context.Add(salesOrders);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(salesOrders);
        }

        // GET: ApiArea/SalesOrders/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salesOrders = await _context.SalesOrders.FindAsync(id);
            if (salesOrders == null)
            {
                return NotFound();
            }
            return View(salesOrders);
        }

        // POST: ApiArea/SalesOrders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("SalesOrderId,ReferenceAtCustomer,LineComment1,ProductId,OrderQuantity,UnitPrice,AddressId,ExpectedDate,TransactionId,TransactionTime,TransactionAmount,TransactionMethod,OrderState,CreatedAt")] SalesOrders salesOrders)
        {
            if (id != salesOrders.SalesOrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(salesOrders);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SalesOrdersExists(salesOrders.SalesOrderId))
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
            return View(salesOrders);
        }

        // GET: ApiArea/SalesOrders/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salesOrders = await _context.SalesOrders
                .FirstOrDefaultAsync(m => m.SalesOrderId == id);
            if (salesOrders == null)
            {
                return NotFound();
            }

            return View(salesOrders);
        }

        // POST: ApiArea/SalesOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var salesOrders = await _context.SalesOrders.FindAsync(id);
            if (salesOrders != null)
            {
                _context.SalesOrders.Remove(salesOrders);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SalesOrdersExists(Guid id)
        {
            return _context.SalesOrders.Any(e => e.SalesOrderId == id);
        }
    }
}
