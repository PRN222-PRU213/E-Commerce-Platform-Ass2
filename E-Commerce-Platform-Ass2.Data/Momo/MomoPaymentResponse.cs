using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce_Platform_Ass2.Data.Momo
{
    public class MomoPaymentResponse
    {
        public string payUrl { get; set; }
        public int resultCode { get; set; }
        public string message { get; set; }
    }
}
