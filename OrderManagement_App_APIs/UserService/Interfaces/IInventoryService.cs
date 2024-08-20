using UserService.DTOs;

namespace UserService.Interfaces
{
    public interface IInventoryService
    {
        Task<string> CheckInventoryItemQuantity(Item item, int quantity);
        Task<Item> GetInventoryItem(string itemName);
        Task<string> ReduceInventoryItemQuantity(Item item, int reduceBy);
    }

}
