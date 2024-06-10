using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Assignment_1_API.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public int StaffId { get; set; }
        //[JsonIgnore]
        public virtual Staff? Staff { get; set; } = null!;
        [JsonIgnore]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
