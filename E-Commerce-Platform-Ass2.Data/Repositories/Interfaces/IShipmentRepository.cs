using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce_Platform_Ass1.Data.Database.Entities;

namespace E_Commerce_Platform_Ass1.Data.Repositories.Interfaces
{
    public interface IShipmentRepository
    {
        Task<IEnumerable<Shipment>> GetAllAsync();
        Task<Shipment?> GetByIdAsync(Guid id);
        Task<Shipment?> GetByOrderIdAsync(Guid orderId);
        Task<IEnumerable<Shipment>> GetByStatusAsync(string status);
        Task<Shipment> AddAsync(Shipment shipment);
        Task<Shipment> UpdateAsync(Shipment shipment);
    }
}
