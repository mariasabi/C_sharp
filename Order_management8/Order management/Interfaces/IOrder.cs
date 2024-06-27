using Order_management.Models;

namespace Order_management.Interfaces
{
    public interface IOrder
    {
        Task<List<Item>> GetItems();
        Task<Item> GetItem(int id);
        Task<Item> AddItem(Item request);
        Task<Item> UpdateItem(Item request);
        Task<Item> DeleteItem(int id);
    //    Task<string> BulkAddItem(string filename);
    //    Task<string> BulkAddItemDynamic(string filename);
        Task<List<Item>> GetPaginatedItems(int page, int pageSize);
    }
}
