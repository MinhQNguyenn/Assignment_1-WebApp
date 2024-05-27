using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Assignment_1_API.Models
{
    public partial class Staff
    {
        public Staff()
        {
            Orders = new HashSet<Order>();
        }

        public Staff(int staffId, string name, string password, int role)
        {
            StaffId = staffId;
            Name = name;
            Password = password;
            Role = role;
        }

        public int StaffId { get; set; }
        [Display(Name = "Staff Name")]
        public string Name { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int Role { get; set; }
        [JsonIgnore]
        public virtual ICollection<Order> Orders { get; set; }
    }
}
