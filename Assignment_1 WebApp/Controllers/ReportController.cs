using Assignment_1_API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Assignment_1_WebApp.Controllers
{
    public class ReportController : Controller
    {
        private readonly HttpClient client = null;
        private string ReportApiUrl = "";

        public ReportController()
        {
            this.client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            ReportApiUrl = "https://localhost:7271/api/Report";
        }

        public async Task<IActionResult> Index()
        {
            HttpResponseMessage response = await client.GetAsync(ReportApiUrl);
            string strData = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            List<Order> listOrders = JsonSerializer.Deserialize<List<Order>>(strData, options);

            return View(listOrders);
        }
    }
}
