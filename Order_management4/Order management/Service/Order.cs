using log4net;
using log4net.Config;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Order_management.Interfaces;
using Order_management.Models;


namespace Order_management.Service
{
    
    public class Order:IOrder
    {
       

        private readonly OrderManagementContext _context;
        private static List<Item> items = new List<Item>();
        public Order(OrderManagementContext context)
        {
            _context = context;
          
        }
        public Task<List<Item>> GetItems()
        {
            
            return _context.Items.ToListAsync();
        }
        public async Task<Item> GetItem(int id)
        {
            var item =  _context.Items.FirstOrDefault(x => x.Id == id);
            return item;
        }
        public async Task<Item> AddItem(Item request)
        {
           /* var existingItem = _context.Items.FirstOrDefault(x => x.Id == request.Id);
            if (existingItem != null)
            {
                return null;
            } */
            _context.Items.Add(request);
            _context.SaveChanges();
            
            return request;
        }
        public async Task<Item> UpdateItem(Item request)
        {
            var existingItem = _context.Items.FirstOrDefault(x => x.Id == request.Id);
            if (existingItem == null)
            {
                return null;
            }
            existingItem.Quantity = request.Quantity;
            existingItem.Name = request.Name;
            existingItem.Type = request.Type;
            _context.SaveChanges();
            
            return request;
        }
        public async Task<Item> DeleteItem(int id)
        {
            var item = _context.Items.FirstOrDefault(x => x.Id == id);
            if (item == null)
            {
             return null;
            }
            _context.Items.Remove(item);
            _context.SaveChanges();
           
            return item;
        }
    }
}
