using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce_Platform_Ass1.Data.Database.Entities
{
    public class Refund
    {
        public Guid Id { get; set; }
        public Guid PaymentId { get; set; }
        public string RequestId { get; set; }
        public decimal RefundAmount { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; } // Success, Failed
        public DateTime CreatedAt { get; set; }
        public Payment Payment { get; set; }
    }
}
