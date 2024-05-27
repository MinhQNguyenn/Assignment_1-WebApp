using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Assignment_1_API.Models;

namespace Assignment_1_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly MyStoreContext _context;

        public ReportController(MyStoreContext context)
        {
            _context = context;
        }

        // GET: api/Report
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders([FromQuery] DateTime? StartDate, [FromQuery] DateTime? EndDate)
        {
            if (_context.Orders == null)
            {
                return NotFound();
            }

            //if (StartDate == null)
            //{
            //    StartDate = DateTime.Now.AddMonths(-1);
            //}

            var orders = _context.Orders.AsQueryable();

            if(StartDate != null)
            {
                orders = orders.Where(item => item.OrderDate >= StartDate);
            }
            if (EndDate != null)
            {
                orders = orders.Where(item => item.OrderDate <= EndDate);
            }

            return await orders.OrderBy(item =>item.OrderDate).ToListAsync();
        }

        private bool OrderExists(int id)
        {
            return (_context.Orders?.Any(e => e.OrderId == id)).GetValueOrDefault();
        }
    }
}
