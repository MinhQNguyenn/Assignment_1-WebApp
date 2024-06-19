using Assignment_1_API.Models;
using Assignment_1_WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace Assignment_1_WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient client = null;
        private string ReportApiUrl = "";

        public HomeController(ILogger<HomeController> logger)
        {
            this.client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            ReportApiUrl = "https://localhost:7271/api/Report";
        }

        [HttpGet]
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
        {
            ViewData["startDate"] = startDate?.ToString("yyyy-MM-dd");
            ViewData["endDate"] = endDate?.ToString("yyyy-MM-dd");

            if (startDate == null)
            {
                startDate = DateTime.Now.AddMonths(-1);
                ViewData["startDate"] = startDate.Value.ToString("yyyy-MM-dd");
            }

            if(endDate == null)
            {
                endDate = DateTime.Now;
                ViewData["endDate"] = endDate.Value.ToString("yyyy-MM-dd");
            }

            string apiUrl = ReportApiUrl;
            if (startDate.HasValue && endDate.HasValue)
            {
                apiUrl += $"?StartDate={startDate.Value.ToString("yyyy-MM-dd")}&EndDate={endDate.Value.ToString("yyyy-MM-dd")}";
            }

            HttpResponseMessage response = await client.GetAsync(apiUrl);
            string strData = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            List<Order> listOrders = JsonSerializer.Deserialize<List<Order>>(strData, options);

            return View(listOrders);
        }

        [HttpGet]
        public async Task<IActionResult> ReportDetail(int orderId)
        {
            ReportApiUrl = "https://localhost:7271/api/OrderDetails/" + orderId; 

            HttpResponseMessage response = await client.GetAsync(ReportApiUrl);
            string strData = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            OrderDetail listOrdersDetail = JsonSerializer.Deserialize<OrderDetail>(strData, options);

            return View("ReportDetails",listOrdersDetail);
        }

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
