using Assignment_1_API.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Assignment_1_WebApp.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string username,string password) {
            try
            {
                var httpClient = new HttpClient();
                var response = await httpClient.GetAsync("https://localhost:7271/api/Staffs");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    List<Staff> staff = JsonConvert.DeserializeObject<List<Staff>>(content);
                    foreach (var item in staff)
                    {
                        if (username.Equals(item.Name) && password.Equals(item.Password))
                        {
                            Console.WriteLine("duy");
                            int role = item.Role;
                            int id = item.StaffId;
                            HttpContext.Session.SetInt32("UserId", id);
                            HttpContext.Session.SetInt32("UserRole", role);
                            return RedirectToAction("Main","Staffs", item);
                            // 8. Redirect to Details action with StaffId
                            return RedirectToAction("Details", "Staffs", new { id = id });
                        }

                            
                    }
                    }

                }
                
            
            catch (Exception)
            {

                throw;
            }
            return RedirectToAction("Index");

        }
    }
}
