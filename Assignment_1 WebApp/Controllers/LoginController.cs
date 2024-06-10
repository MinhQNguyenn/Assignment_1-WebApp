using Assignment_1_API.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
                IConfiguration config = new ConfigurationBuilder()
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
      .Build();
                var httpClient = new HttpClient();
                var response = await httpClient.GetAsync("https://localhost:7271/odata/Staffs");
                string adminUsername = config["Account:Name"];
                string adminPassword = config["Account:Password"];

                if (adminUsername.Equals(username) && adminPassword.Equals(password))
                {
                    HttpContext.Session.SetInt32("UserId", 0);
                    HttpContext.Session.SetInt32("UserRole", 1);
                    return RedirectToAction("Main", "Staffs", new Staff(0, adminUsername, adminPassword, 1));
                }
                if (response.IsSuccessStatusCode)
                {
                    
                    string strData = await response.Content.ReadAsStringAsync();
                    var temp = JObject.Parse(strData);
                    dynamic list = temp["value"];

                 
                    List<Staff> staff = JsonConvert.DeserializeObject<List<Staff>>(list.ToString());
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
