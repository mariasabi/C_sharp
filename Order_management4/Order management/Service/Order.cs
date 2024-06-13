using log4net;
using log4net.Config;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Order_management.Interfaces;
using Order_management.Models;
using log4net;
using Order_management.Exceptions;

namespace Order_management.Service
{
    
    public class Order:IOrder
    {
        private readonly OrderManagementContext _context;
        private static readonly ILog log = LogManager.GetLogger(typeof(Order));
        private static List<Item> items = new List<Item>();
        public Order(OrderManagementContext context)
        {
            _context = context;
        }
        public async Task<List<Item>> GetItems()
        {
            var items = await _context.Items.ToListAsync();
            if (items.Count==0)
            {
                log.Debug("No items found to retrieve.");
            }
            log.Info("Retrieved all items.");
            return items;
        }

        public async Task<Item> GetItem(int id)
        {
            var item =  _context.Items.FirstOrDefault(x => x.Id == id);
            if (item == null)
            {
                log.Debug("Item cannot be retrieved.");
                throw new IdNotFoundException("No such item exists");
                //return BadRequest("No item found!");
            }
            log.Info($"Retrieved item with ID {id}.");
            return item;
        }
        public async Task<Item> AddItem(Item request)
        {
            _context.Items.Add(request);
            _context.SaveChanges();
            log.Info($"Item with ID {request.Id} added.");
            return request;
        }
        public async Task<Item> UpdateItem(Item request)
        {
            var existingItem = _context.Items.FirstOrDefault(x => x.Id == request.Id);
            if (existingItem == null)
            {
                log.Debug("Item cannot be retrieved to update.");
                throw new IdNotFoundException("No such item exists to update");
            }
            log.Info($"Item with ID {request.Id} updated.");
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
                log.Debug("Item cannot be retrieved to delete.");
                throw new IdNotFoundException("No such item exists to delete");
            }
            log.Info($"Item with ID {id} deleted.");
            _context.Items.Remove(item);
            _context.SaveChanges();
            return item;
        }
    }
}
