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

    }
}
