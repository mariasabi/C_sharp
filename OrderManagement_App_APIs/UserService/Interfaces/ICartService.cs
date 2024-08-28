using Microsoft.AspNetCore.Mvc;
using UserService.DTOs;

namespace UserService.Interfaces
{
    public interface ICartService
    {
        Task<CartItemDTO[]> ViewCart();
        Task<CartItemDTO[]> AddCartItem(CartItemDTO item);
        Task<CartItemDTO[]> RemoveCartItem(string name);
        Task<string> PurchaseCart();
        Task<CartItemDTO[]> UpdateCartItemQuantity(string itemName, bool inc = true);
        Task<List<OrderDTO>> GetOrdersOfUser();
        Task<List<OrderDTO>> GetAllOrders();
        Task<decimal> GetCartValue();
    }
}
