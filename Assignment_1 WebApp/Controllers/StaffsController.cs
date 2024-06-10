using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Assignment_1_API.Models;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace Assignment_1WebApp.Controllers
{
    public class StaffsController : Controller
    {
        private readonly string _apiBaseUrl = "https://localhost:7271/odata/Staffs"; // Base URL of your API
        private readonly HttpClient client = null;
        private string StaffApiUrl = "";
        public StaffsController()
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
            List<Staff> listStaffs = System.Text.Json.JsonSerializer.Deserialize<List<Staff>>(strData, options);
            return View(listStaffs);

        }

        public async Task<IActionResult> Main(Staff staff)
        {
            return View(staff);
        }

        // GET: Staffs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            HttpResponseMessage response = await client.GetAsync(StaffApiUrl);
            string strData = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            List<Staff> listStaffs = System.Text.Json.JsonSerializer.Deserialize<List<Staff>>(strData, options);
            var Staff = listStaffs.FirstOrDefault(p => p.StaffId == id);

            return View(Staff);
        }

        // GET: Staffs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Staffs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StaffId,Name, Password, Role")] Staff Staff)
        {
            if (ModelState.IsValid)
            {
                var json = System.Text.Json.JsonSerializer.Serialize(Staff);
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
                var Staffs = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<Staff>>(json);
                ViewBag.SearchResults = Staffs;
            }
            else
            {
                ViewBag.SearchResults = new List<Staff>();
                ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
            }

            return View("Index");
        }

        // GET: Staffs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var httpClient = new HttpClient();
                var response = await httpClient.GetAsync($"{_apiBaseUrl}/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var staff = JsonConvert.DeserializeObject<Staff>(content); // Assuming Staff is your model class

                    if (staff == null)
                    {
                        return NotFound();
                    }

                    return View(staff);
                }
                else
                {
                    return Problem($"Failed to retrieve staff details for editing. Status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                return Problem($"An error occurred while fetching staff details for editing: {ex.Message}");
            }
        }

        // POST: Staffs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StaffId,Name,Password,Role")] Staff staff,string newpassword)
        {
            if (id != staff.StaffId)
            {
                return NotFound();
            }
            if(newpassword.Equals(staff.Password))
            {
                try
                {
                    var httpClient = new HttpClient();
                    var json = JsonConvert.SerializeObject(staff);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await httpClient.PutAsync($"{_apiBaseUrl}/{id}", content);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Main");
                        //return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        return Problem($"Failed to update staff. Status code: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    return Problem($"An error occurred while updating staff: {ex.Message}");
                }
            }
            return View(staff);

           
        }
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Remove("UserId");
            HttpContext.Session.Remove("UserRole");
            return RedirectToAction("Index", "Login");

        }
        // GET: Staffs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var httpClient = new HttpClient();
                var response = await httpClient.GetAsync($"{_apiBaseUrl}/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var staff = JsonConvert.DeserializeObject<Staff>(content); // Assuming Staff is your model class

                    if (staff == null)
                    {
                        return NotFound();
                    }

                    return View(staff);
                }
                else
                {
                    return Problem($"Failed to retrieve staff details for deletion. Status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                return Problem($"An error occurred while fetching staff details for deletion: {ex.Message}");
            }
        }

        // POST: Staffs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var httpClient = new HttpClient();
                var response = await httpClient.DeleteAsync($"{_apiBaseUrl}/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return Problem($"Failed to delete staff. Status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                return Problem($"An error occurred while deleting staff: {ex.Message}");
            }
        }
    }
}
