using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Assignment_1_API.Models;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace Assignment_1WebApp.Controllers
{
    public class StaffsController : Controller
    {
        private readonly string _apiBaseUrl = "https://localhost:7271/api/Staffs"; // Base URL of your API

        public StaffsController()
        {

        }
        public async Task<IActionResult> Index()
        {
            try
            {
                var httpClient = new HttpClient();
                var response = await httpClient.GetAsync($"{_apiBaseUrl}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var staffList = JsonConvert.DeserializeObject<List<Staff>>(content); // Assuming Staff is your model class

                    if (staffList == null || staffList.Count == 0)
                    {
                        return NotFound(); // No staff found
                    }

                    return View(staffList);
                }
                else
                {
                    return Problem($"Failed to retrieve staff details. Status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                return Problem($"An error occurred while fetching staff details: {ex.Message}");
            }

        }

        public async Task<IActionResult> Main(Staff staff)
        {
            return View(staff);
        }

        // GET: Staffs/Details/5
        public async Task<IActionResult> Details(int? id)
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
                    return Problem($"Failed to retrieve staff details. Status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                return Problem($"An error occurred while fetching staff details: {ex.Message}");
            }
        }

        // GET: Staffs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Staffs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Password,Role")] Staff staff)
        {
            try
            {
                var httpClient = new HttpClient();
                var json = JsonConvert.SerializeObject(staff);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(_apiBaseUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return Problem($"Failed to create staff. Status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                return Problem($"An error occurred while creating staff: {ex.Message}");
            }
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
                    var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                    var response = await httpClient.PutAsync($"{_apiBaseUrl}/{id}", content);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
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
