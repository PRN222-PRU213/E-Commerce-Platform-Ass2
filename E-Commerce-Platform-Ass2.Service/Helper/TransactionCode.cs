using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce_Platform_Ass2.Service.Helper
{
    public class TransactionCode
    {
        public string GenerateTransactionCode(Guid orderId)
        {
            var datePart = DateTime.UtcNow.ToString("ddMMyyyy");
            var guidPart = orderId.ToString("N")[..8].ToUpper();
            return $"TXN-{datePart}-{guidPart}";
        }

    }
}
