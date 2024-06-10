using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Assignment_1_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using static System.Net.WebRequestMethods;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Assignment1_ClientWebApp.Controllers
{
    public class OrdersController : Controller
    {
        private readonly MyStoreContext _context;
        private readonly HttpClient client = null;
        private string ApiUrl = string.Empty;
        public OrdersController(MyStoreContext context)
        {
            _context = context;
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            //ApiUrl = "https://localhost:7271/api/";
            ApiUrl = "https://localhost:7271/odata/";
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            string query = "Orders";
            HttpResponseMessage response = await client.GetAsync($"{ApiUrl}Orders?$expand=Staff");
            string strData = await response.Content.ReadAsStringAsync();
            //var options = new JsonSerializerOptions
            //{
            //    PropertyNameCaseInsensitive = true,
            //};
            //var myStore_G5Context = JsonSerializer.Deserialize<List<Order>>(strData, options);
            var tmp = JObject.Parse(strData);
            var orderList = tmp["value"].ToObject<List<Order>>();
            return View(orderList);
        }
        [HttpPost]
        public async Task<IActionResult> Index(DateTime startOrderDate, DateTime endOrderDate, string staffName)
        {
            string query = "Orders?$expand=Staff";
            HttpResponseMessage response = await client.GetAsync($"{ApiUrl}{query}");
            string strData = await response.Content.ReadAsStringAsync();
            //var options = new JsonSerializerOptions
            //{
            //    PropertyNameCaseInsensitive = true,
            //};
            //var orderList = JsonSerializer.Deserialize<List<Order>>(strData, options);
            
            var orderList = JObject.Parse(strData)["value"].ToObject<List<Order>>();

            if (staffName == null)
                staffName = string.Empty;
            orderList = orderList.Where(i => startOrderDate.ToShortDateString().Trim().Equals(i.OrderDate.ToShortDateString().Trim()) && i.Staff.Name.ToLower().Trim().Contains(staffName.ToLower().Trim())).ToList();
            ViewData["startOrderDate"] = startOrderDate;
            ViewData["endOrderDate"] = endOrderDate;
            ViewData["staffName"] = staffName;
            return View(orderList);
        }

        // GET: Orders/Details/5    
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }
            string query = "OrderDetails";
            //HttpResponseMessage response = await client.GetAsync($"{ApiUrl}{query}");
            HttpResponseMessage response = await client.GetAsync($"https://localhost:7271/api/{query}");
            string strData = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            var myStore_G5Context = JsonSerializer.Deserialize<List<OrderDetail>>(strData, options);
            ViewData["OrderId"] = id;
            var orderDetailList = myStore_G5Context.Where(od => od.OrderId == id).ToList();
            if (orderDetailList == null)
            {
                return NotFound();
            }

            return View(orderDetailList);
        }
        // GET: Orders/Create
        public async Task<IActionResult> Create()
        {
            string query = "Staffs";
            HttpResponseMessage response = await client.GetAsync($"{ApiUrl}{query}");
            string strData = await response.Content.ReadAsStringAsync();

            var staffList = JObject.Parse(strData)["value"].ToObject<List<Staff>>();
            ViewData["StaffId"] = new SelectList(staffList, "StaffId", "Name");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,OrderDate,StaffId")] Order order)
        {
            string query;
            if (ModelState.IsValid)
            {
                string dataString = JsonSerializer.Serialize(order);
                query = "Orderss";
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, $"{ApiUrl}{query}");
                requestMessage.Content = new StringContent(dataString, Encoding.UTF8, "application/json");
                HttpResponseMessage responseMessage = await client.SendAsync(requestMessage);
                if (responseMessage.IsSuccessStatusCode)
                {
                    // Xử lý response thành công
                    string responseData = await responseMessage.Content.ReadAsStringAsync();
                    Console.WriteLine(responseData);
                }
                return RedirectToAction(nameof(Index));
            }
            query = "Staffs";
            HttpResponseMessage response = await client.GetAsync($"{ApiUrl}{query}");
            string strData = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            var staffList = JsonSerializer.Deserialize<List<Staff>>(strData, options);
            ViewData["StaffId"] = new SelectList(staffList, "StaffId", "Name", order.StaffId);
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            string query = "Orders";
            HttpResponseMessage response = await client.GetAsync($"{ApiUrl}{query}");
            string strData = await response.Content.ReadAsStringAsync();
            var tmp = JObject.Parse(strData);
            var orderList = tmp["value"].ToObject<List<Order>>();
            if (id == null || orderList == null)
            {
                return NotFound();
            }
            response = await client.GetAsync($"{ApiUrl}{query}/{id}");
            strData = await response.Content.ReadAsStringAsync();
            var order = JsonSerializer.Deserialize<Order>(strData);
            if (order == null)
            {
                return NotFound();
            }


            //var options = new JsonSerializerOptions
            //{
            //    PropertyNameCaseInsensitive = true,
            //};
            //var staffList = JsonSerializer.Deserialize<List<Staff>>(strData, options);
            query = "Staffs";
            response = await client.GetAsync($"{ApiUrl}{query}");
            strData = await response.Content.ReadAsStringAsync();
            tmp = JObject.Parse(strData);
            var staffList = tmp["value"].ToObject<List<Staff>>();
            ViewData["StaffId"] = new SelectList(staffList, "StaffId", "Name", order.StaffId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,OrderDate,StaffId")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //string dataString = JsonSerializer.Serialize(order);
                    //string query = "Orders";
                    //HttpContent content = new StringContent(dataString, Encoding.UTF8, "application/json");
                    //HttpResponseMessage responseMessage = await client.PutAsync($"{ApiUrl}{query}", content);
                    //string dataString = JsonSerializer.Serialize(order);
                    string query = "Orderss";
                    //HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Put, $"{ApiUrl}{query}/{id}");
                    //requestMessage.Content = new StringContent(dataString, Encoding.UTF8, "application/json");
                    //HttpResponseMessage responseMessage = await client.SendAsync(requestMessage);
                    HttpResponseMessage httpResponseMessage = await client.PutAsJsonAsync($"{ApiUrl}{query}/{id}", order);

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
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
            ViewData["StaffId"] = new SelectList(_context.Staffs, "StaffId", "Name", order.StaffId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            //var order = await _context.Orders
            //    .Include(o => o.Staff)
            //    .FirstOrDefaultAsync(m => m.OrderId == id);
            string query = "Orders";
            HttpResponseMessage response = await client.GetAsync($"{ApiUrl}Orders/{id}");
            string strData = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            var order = JsonSerializer.Deserialize<Order>(strData, options);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            //    .FirstOrDefaultAsync(m => m.OrderId == id);
            string query = "Orders";
            HttpResponseMessage response = await client.GetAsync($"{ApiUrl}Orders/{id}");
            string strData = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            var order = JsonSerializer.Deserialize<Order>(strData, options);
            //if (_context.Orders == null)
            //{
            //    return Problem("Entity set 'MyStore_G5Context.Orders'  is null.");
            //}
            if (order != null)
            {
                response = await client.DeleteAsync($"{ApiUrl}Orders/{id}");
            }
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return (_context.Orders?.Any(e => e.OrderId == id)).GetValueOrDefault();
        }
    }
}
