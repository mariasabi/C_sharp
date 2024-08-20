using OrderService.Models;

namespace OrderService.Interfaces
{
    public interface IOrder
    { 

    Task<List<Item>> GetItems();
    Task<Item> GetItem(int id);
    Task<Item> GetItemByName(string name);
        Task<Item> AddItem(Item request);
    Task<Item> UpdateItem(Item request);
    Task<Item> DeleteItem(int id);
    Task<List<Item>> GetPaginatedItems(int page, int pageSize);
        Task<string> BulkAddItem(IFormFile fileName);
        Task<Item> UpdateItemQuantity(Item request, int quantity);
}
}
