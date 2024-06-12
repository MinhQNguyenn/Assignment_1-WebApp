using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assignment_1_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace Assignment1_ClientWebApp.Controllers
{
    public class OrderDetailsController : Controller
    {
        private readonly MyStoreContext _context;

        public OrderDetailsController(MyStoreContext context)
        {
            _context = context;
        }

        // GET: OrderDetails
        public async Task<IActionResult> Index()
        {
            var myStore_G5Context = _context.OrderDetails.Include(o => o.Order).Include(o => o.Product);
            return View(await myStore_G5Context.ToListAsync());
        }

        // GET: OrderDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.OrderDetails == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetails
                .Include(o => o.Order)
                .Include(o => o.Product)
                .FirstOrDefaultAsync(m => m.OrderDetailId == id);
            if (orderDetail == null)
            {
                return NotFound();
            }

            return View(orderDetail);
        }

        // GET: OrderDetails/Create
        public IActionResult Create(int id)
        {
            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId", id);
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName");
            ViewData["SelectedId"] = id;
            return View();
        }

        // POST: OrderDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderDetailId,OrderId,ProductId,Quantity,UnitPrice")] OrderDetail orderDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orderDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), "Orders", new { id = orderDetail.OrderId });
            }
            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId", orderDetail.OrderId);
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName", orderDetail.ProductId);
            return View(orderDetail);
        }

        // GET: OrderDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.OrderDetails == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetails.Include(o => o.Product).FirstOrDefaultAsync(od => od.OrderDetailId == id);
            if (orderDetail == null)
            {
                return NotFound();
            }
            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId", orderDetail.OrderId);
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName", orderDetail.ProductId);
            //return RedirectToAction($"Details/{id}", "Orders");
            return View(orderDetail);
        }

        // POST: OrderDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderDetailId,OrderId,ProductId,Quantity,UnitPrice")] OrderDetail orderDetail)
        {
            if (id != orderDetail.OrderDetailId)
            {
                return NotFound();
            }

            //if (ModelState.IsValid)
            //{
            try
            {
                _context.Update(orderDetail);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderDetailExists(orderDetail.OrderDetailId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction($"Details", "Orders", new { id = orderDetail.OrderId });
            //}
            //ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId", orderDetail.OrderId);
            //ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName", orderDetail.ProductId);
            //return View(orderDetail);
        }

        // GET: OrderDetails/Delete/5
        public async Task<IActionResult> Delete(int? id, int orderId)
        {
            if (id == null || _context.OrderDetails == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetails
                .Include(o => o.Order)
                .Include(o => o.Product)
                .FirstOrDefaultAsync(m => m.OrderDetailId == id);
            if (orderDetail == null)
            {
                return NotFound();
            }
            ViewData["OrderId"] = orderId;
            return View(orderDetail);
        }

        // POST: OrderDetails/Delete/5 **********
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int orderId)
        {
            if (_context.OrderDetails == null)
            {
                return Problem("Entity set 'MyStore_G5Context.OrderDetails'  is null.");
            }
            var orderDetail = await _context.OrderDetails.FindAsync(id);
            if (orderDetail != null)
            {
                _context.OrderDetails.Remove(orderDetail);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Orders", new { id = orderId });
        }

        private bool OrderDetailExists(int id)
        {
            return (_context.OrderDetails?.Any(e => e.OrderDetailId == id)).GetValueOrDefault();
        }
    }
}
