using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce_Platform_Ass1.Data.Database.Entities
{
    public class Wallet
    {
        public Guid WalletId { get; set; }
        public Guid UserId { get; set; }

        public decimal Balance { get; set; }

        public decimal? LastChangeAmount { get; set; }
        public string? LastChangeType { get; set; }

        public DateTime UpdatedAt { get; set; }
        public User User { get; set; }
    }
}
