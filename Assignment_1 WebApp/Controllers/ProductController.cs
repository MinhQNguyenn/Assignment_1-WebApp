using Assignment_1_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Assignment_1_WebApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly HttpClient client = null;
        private string ProductApiUrl = "";
        public ProductController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            ProductApiUrl = "https://localhost:7271/odata/Products";
        }
        public async Task<IActionResult> Index(string? pName = null, int? unitPrice = null)
        {
            HttpResponseMessage response;
            string odataQuery = "";

            // Build OData query string
            if (!string.IsNullOrEmpty(pName) && unitPrice.HasValue)
            {
                odataQuery = $"?$filter=contains(ProductName,'{pName}') and UnitPrice eq {unitPrice.Value}";
            }
            else if (!string.IsNullOrEmpty(pName))
            {
                odataQuery = $"?$filter=contains(ProductName,'{pName}')";
            }
            else if (unitPrice.HasValue)
            {
                odataQuery = $"?$filter=UnitPrice eq {unitPrice.Value}";
            }

            if (!string.IsNullOrEmpty(odataQuery))
            {
                response = await client.GetAsync($"{ProductApiUrl}{odataQuery}");
                ViewBag.name = pName;
                ViewBag.unitPrice = unitPrice;
            }
            else
            {
                response = await client.GetAsync(ProductApiUrl);
            }

            string strData = await response.Content.ReadAsStringAsync();
            var temp = JObject.Parse(strData);
            dynamic list = temp["value"];
            List<Product> listProducts = JsonConvert.DeserializeObject<List<Product>>(list.ToString());
            return View(listProducts);
        }


        public async Task<IActionResult> Details(int? id)
        {
            HttpResponseMessage response = await client.GetAsync(ProductApiUrl);
            string strData = await response.Content.ReadAsStringAsync();
            var temp = JObject.Parse(strData);
            dynamic list = temp["value"];

            List<Product> listProducts = JsonConvert.DeserializeObject<List<Product>>(list.ToString());
            var product = listProducts.FirstOrDefault(p => p.ProductId == id);

            return View(product);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            var url = $"{ProductApiUrl}/{id}";

            // Send DELETE request to the API
            var response = await client.DeleteAsync(url);

            // Check if the response indicates success
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return Problem("Error deleting the product.");
            }
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var url = $"{ProductApiUrl}/{id}";

            // Send DELETE request to the API
            var response = await client.DeleteAsync(url);

            // Check if the response indicates success
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return Problem("Error deleting the product.");
            }
        }
        public async Task<IActionResult> Create()
        {

            ViewData["CategoryId"] = new SelectList(await GetCategories(), "CategoryId", "CategoryName");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,CategoryId,UnitPrice")] Product product)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(product);
                HttpResponseMessage res = await client.PostAsJsonAsync(ProductApiUrl, product);

                if (res.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // Handle error response
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            else
            {
                // Log the validation errors
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    // Log or display the error message
                    Console.WriteLine(error.ErrorMessage);
                }
            }
            ViewData["CategoryId"] = new SelectList(await GetCategories(), "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }
        private async Task<IEnumerable<Category>> GetCategories()
        {
            HttpResponseMessage response = await client.GetAsync("https://localhost:7271/odata/Categories");
            string strData = await response.Content.ReadAsStringAsync();

            var temp = JObject.Parse(strData);
            dynamic list = temp["value"];
            List<Category> listCategories = JsonConvert.DeserializeObject<List<Category>>(list.ToString());

            return listCategories;
        }
        public async Task<IActionResult> Search(string pName, decimal? unitPrice)
        {
            var query = $"?pName={pName}&unitPrice={unitPrice}";
            var response = await client.GetAsync($"{ProductApiUrl}/search{query}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var temp = JObject.Parse(json);
                dynamic list = temp["value"];
                var products = JsonConvert.DeserializeObject<IEnumerable<Category>>(list.ToString());
                ViewBag.SearchResults = products;
            }
            else
            {
                ViewBag.SearchResults = new List<Product>();
                ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
            }

            return View("Index");
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Fetch product details from the API
            var response = await client.GetAsync($"{ProductApiUrl}/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var strData = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            var product = JsonConvert.DeserializeObject<Product>(strData);

            if (product == null)
            {
                return NotFound();
            }

            var categories = await GetCategories();

            ViewData["CategoryId"] = new SelectList(categories, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,CategoryId,UnitPrice")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var jsonContent = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");
                    var response = await client.PutAsJsonAsync($"{ProductApiUrl}/{id}", product);

                    if (!response.IsSuccessStatusCode)
                    {
                        return NotFound();
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (HttpRequestException e)
                {

                    throw new Exception(e.Message);
                }
            }

            var categories = await GetCategories();
            ViewData["CategoryId"] = new SelectList(categories, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }
    }
}