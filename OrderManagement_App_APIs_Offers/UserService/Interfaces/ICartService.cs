using Microsoft.AspNetCore.Mvc;
using UserService.DTOs;

namespace UserService.Interfaces
{
    public interface ICartService
    {
        Task<CartItemDTO[]> ViewCart();
        Task<CartItemDTO[]> AddCartItem(CartItemDTO item);
        Task<CartItemDTO[]> RemoveCartItem(string name);
        Task<string> PurchaseCart(int amt, int cartVal);
        Task<CartItemDTO[]> UpdateCartItemQuantity(string itemName, bool inc = true);
        Task<List<OrderDTO>> GetOrdersOfUser();
        Task<List<OrderDTO>> GetAllOrders();
        Task<decimal> GetCartValue();
        Task<string> UpdateVoucherAmount(decimal amt);
        Task<decimal> GetVoucherAmount();
    
    }
}
