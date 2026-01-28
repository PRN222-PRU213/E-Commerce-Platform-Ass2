using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce_Platform_Ass1.Data.Database;
using E_Commerce_Platform_Ass1.Data.Database.Entities;
using E_Commerce_Platform_Ass1.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_Platform_Ass1.Data.Repositories
{
    public class ShipmentRepository : IShipmentRepository
    {
        private readonly ApplicationDbContext _context;

        public ShipmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Shipment> AddAsync(Shipment shipment)
        {
            _context.Shipments.Add(shipment);
            await _context.SaveChangesAsync();
            return shipment;
        }

        public async Task<IEnumerable<Shipment>> GetAllAsync()
        {
            return await _context.Shipments.ToListAsync();
        }

        public async Task<Shipment?> GetByIdAsync(Guid id)
        {
            return await _context.Shipments.FindAsync(id);
        }

        public async Task<Shipment?> GetByOrderIdAsync(Guid orderId)
        {
            return await _context.Shipments
                .FirstOrDefaultAsync(x => x.OrderId == orderId);
        }

        public async Task<IEnumerable<Shipment>> GetByStatusAsync(string status)
        {
            return await _context.Shipments
                .Where(s => s.Status == status).ToListAsync();
        }

        public async Task<Shipment> UpdateAsync(Shipment shipment)
        {
            _context.Shipments.Update(shipment);
            await _context.SaveChangesAsync();
            return shipment;
        }
    }
}
