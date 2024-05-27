using System;
using System.Collections.Generic;

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
        public string Name { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int Role { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
