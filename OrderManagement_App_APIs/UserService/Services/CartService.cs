using Microsoft.EntityFrameworkCore;
using UserService.DTOs;
using UserService.Exceptions;
using UserService.Interfaces;
using UserService.Models;

namespace UserService.Services
{
    public class CartService:ICartService
    {
        private readonly OrderContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private int userId;
        private readonly IInventoryService _inventoryService;
        public CartService(IInventoryService inventoryService,OrderContext orderContext, IHttpContextAccessor httpContextAccessor)
        {
            _context = orderContext;
            _inventoryService = inventoryService;
            _httpContextAccessor = httpContextAccessor;
             var userIdString = _httpContextAccessor.HttpContext.User.Claims
            .FirstOrDefault(c => c.Type == "id")?.Value;
            userId = int.Parse(userIdString);
        }
        public async Task<CartItemDTO[]> ViewCart()
        {

            var cart = await _context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart != null && cart.CartItems != null)
            {
                var cartItemDTOs = cart.CartItems
                    .Select(item => new CartItemDTO
                    {
                        ItemName = item.ItemName,
                        Quantity = item.Quantity,
                        Price=item.Price

                    })
                    .ToArray();
                return cartItemDTOs;
            }

            else
            {
                return null;
            }
        }
        public async Task<string> AddCartItem(CartItemDTO cartItemDTO)
        {
            
            var inventoryItem = await _inventoryService.GetInventoryItem(cartItemDTO.ItemName);
            var res = await _inventoryService.CheckInventoryItemQuantity(inventoryItem, (int)cartItemDTO.Quantity);
            
            var cart = await _context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null) 
            {
                cart = new Cart
                {
                    UserId = userId,
                    CartValue=0,
                    CartItems = new List<CartItem>()
                };
                _context.Carts.Add(cart);
            }
          
                var existingItem= cart.CartItems.FirstOrDefault(c => c.ItemName == cartItemDTO.ItemName);
                if(existingItem!=null)
                {

                    existingItem.Quantity += cartItemDTO.Quantity;
                    if (existingItem.Price.HasValue)
                    {
                        cart.CartValue += existingItem.Price.Value * cartItemDTO.Quantity.GetValueOrDefault(1);
                    }

                }
                else
                {
                    var cartItem = new CartItem
                    {
                        CartId = cart.CartId,
                        ItemName = cartItemDTO.ItemName,
                        Quantity = cartItemDTO.Quantity,
                        Price = cartItemDTO.Price
                    };


                    cart.CartItems.Add(cartItem);
                    if (cartItem.Price.HasValue)
                    {
                        cart.CartValue += cartItem.Price.Value * cartItem.Quantity.GetValueOrDefault(1);
                    }

                  
                }
            
            var result = await _context.SaveChangesAsync();

            return "Added item to cart";
        }

       public  async Task<string> RemoveCartItem(string itemName)
        {
            var cart = await _context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.UserId == userId);
            if(cart==null)
            {
                throw new ArgumentsException("Item not found in cart");
            }
            var cartItem = cart.CartItems.FirstOrDefault(c => c.ItemName == itemName);
            if(cartItem==null)
            {
                throw new ArgumentsException("Item not found in cart");
            }
            var inventoryItem = await _inventoryService.GetInventoryItem(itemName);
            if (inventoryItem == null)
                throw new ArgumentsException("Inventory item not found");
            var result = await _inventoryService.ReduceInventoryItemQuantity(inventoryItem, -(int)cartItem.Quantity);

            if (cartItem.Price.HasValue)
            {
                cart.CartValue -= cartItem.Price.Value * cartItem.Quantity.GetValueOrDefault(1);
            }
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return $"Item '{itemName}' successfully removed from the cart.";

        }
        public async Task<string> UpdateCartItemQuantity(string itemName,bool inc=true)
        {
            var cart = await _context.Carts
                        .Include(c => c.CartItems)
                        .FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null)
            {
                throw new ArgumentsException("Item not found in cart");
            }
            var cartItem = cart.CartItems.FirstOrDefault(c => c.ItemName == itemName);
            if (cartItem == null)
            {
                throw new ArgumentsException("Item not found in cart");
            }
            var inventoryItem = await _inventoryService.GetInventoryItem(itemName);
            if (inventoryItem == null)
                throw new ArgumentsException("Inventory item not found");

            if (inc) 
            {
                var result =await _inventoryService.CheckInventoryItemQuantity(inventoryItem, 1);
                if (cartItem.Price.HasValue)
                {
                    cart.CartValue += cartItem.Price.Value;
                }
                cartItem.Quantity++;
            }
            else
            {
                
                var result = await _inventoryService.ReduceInventoryItemQuantity(inventoryItem, -1);
                if(cartItem.Quantity==0)
                    _context.CartItems.Remove(cartItem);
                else
                    cartItem.Quantity--;

                if (cartItem.Price.HasValue)
                {
                    cart.CartValue -= cartItem.Price.Value;
                } 
                
            }
           
            await _context.SaveChangesAsync();

            return $"Item '{itemName}' successfully updated in cart.";

        }


        public async Task<string> PurchaseCart()
        {
             // Retrieve the user's cart based on the userId
                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null)
                {
                    throw new ArgumentsException("No cart found for the specified user.");
                }

                if (!cart.CartItems.Any())
                {
                    throw new ArgumentsException("The cart is empty. Add items to the cart before purchasing.");
                }

                // Remove each CartItem associated with the cart
                foreach (var cartItem in cart.CartItems.ToList())
                {
                    _context.CartItems.Remove(cartItem); // Remove CartItem from the database
                }

                cart.CartValue = 0; // Reset the cart value

                // Save the changes to the database
                await _context.SaveChangesAsync();

                return "Purchase successful";
            

        }
    }
}
