using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce_Platform_Ass2.Service.DTOs
{
    public class WalletPaymentResultDto
    {
        public decimal WalletUsedAmount { get; set; }
        public decimal MomoPayAmount { get; set; }
        public bool NeedMomo { get; set; }
    }
}
