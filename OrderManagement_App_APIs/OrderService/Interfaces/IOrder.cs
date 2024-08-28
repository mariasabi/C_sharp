using OrderService.DTOs;
using OrderService.Models;

namespace OrderService.Interfaces
{
    public interface IOrder
    { 

    Task<List<ItemViewDTO>> GetItems();
    Task<ItemViewDTO> GetItem(int id);
    Task<ItemViewDTO> GetItemByName(string name);
    Task<Item> AddItem(ItemDTO request);
    Task<Item> UpdateItem(ItemDTO request);
    Task<Item> DeleteItem(int id);
    Task<List<Item>> GetPaginatedItems(int page, int pageSize);
        Task<string> BulkAddItem(IFormFile fileName);
        Task<Item> UpdateItemQuantity(string name, int quantity);
}
}
