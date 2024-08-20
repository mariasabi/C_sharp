using Microsoft.AspNetCore.Mvc;
using UserService.DTOs;

namespace UserService.Interfaces
{
    public interface ICartService
    {
        Task<CartItemDTO[]> ViewCart();
        Task<string> AddCartItem(CartItemDTO item);
        Task<string> RemoveCartItem(string name);
        Task<string> PurchaseCart();
        Task<string> UpdateCartItemQuantity(string itemName, bool inc = true);
    }
}
