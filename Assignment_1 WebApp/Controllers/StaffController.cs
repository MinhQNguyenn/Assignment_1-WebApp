using Assignment_1_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace Assignment_1_WebApp.Controllers
{
    public class StaffController : Controller
    {
        private readonly HttpClient client = null;
        private string StaffApiUrl = "";
        public StaffController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            StaffApiUrl = "https://localhost:7271/odata/Staffs";
        }
        public async Task<IActionResult> Index(string pName = null)
        {
            HttpResponseMessage response;
            if (!string.IsNullOrEmpty(pName))
            {
                var query = $"?pName={pName}";
                response = await client.GetAsync($"{StaffApiUrl}/search{query}");
            }
            else
            {
                response = await client.GetAsync(StaffApiUrl);
            }

            string strData = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            List<Staff> listStaffs = JsonSerializer.Deserialize<List<Staff>>(strData, options);
            return View(listStaffs);
        }
        public async Task<IActionResult> Details(int? id)
        {
            HttpResponseMessage response = await client.GetAsync(StaffApiUrl);
            string strData = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            List<Staff> listStaffs = JsonSerializer.Deserialize<List<Staff>>(strData, options);
            var Staff = listStaffs.FirstOrDefault(p => p.StaffId == id);

            return View(Staff);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            HttpResponseMessage response = await client.GetAsync(StaffApiUrl);
            string strData = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            List<Staff> listStaffs = JsonSerializer.Deserialize<List<Staff>>(strData, options);
            var Staff = listStaffs.FirstOrDefault(p => p.StaffId == id);


            return View(Staff);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var url = $"{StaffApiUrl}/{id}";

            // Send DELETE request to the API
            var response = await client.DeleteAsync(url);

            // Check if the response indicates success
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return Problem("Error deleting the Staff.");
            }
        }
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StaffId,Name, Password, Role")] Staff Staff)
        {
            if (ModelState.IsValid)
            {
                var json = JsonSerializer.Serialize(Staff);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(StaffApiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // Handle error response
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return View(Staff);
        }

        public async Task<IActionResult> Search(string pName)
        {
            var query = $"?pName={pName}";
            var response = await client.GetAsync($"{StaffApiUrl}/search{query}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var Staffs = JsonSerializer.Deserialize<IEnumerable<Staff>>(json);
                ViewBag.SearchResults = Staffs;
            }
            else
            {
                ViewBag.SearchResults = new List<Staff>();
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

            // Fetch Staff details from the API
            var response = await client.GetAsync($"{StaffApiUrl}/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var strData = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            var Staff = JsonSerializer.Deserialize<Staff>(strData, options);

            if (Staff == null)
            {
                return NotFound();
            }


            return View(Staff);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StaffId,Name, Password, Role")] Staff Staff)
        {
            if (id != Staff.StaffId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var jsonContent = new StringContent(JsonSerializer.Serialize(Staff), Encoding.UTF8, "application/json");
                    var response = await client.PutAsync($"{StaffApiUrl}/{id}", jsonContent);

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
            return View(Staff);
        }
    }
}
