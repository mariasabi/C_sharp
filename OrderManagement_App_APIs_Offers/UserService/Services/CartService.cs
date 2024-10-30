using log4net;
using Microsoft.EntityFrameworkCore;
using UserService.DTOs;
using UserService.Exceptions;
using UserService.Interfaces;
using UserService.Models;

namespace UserService.Services
{
    public class CartService:ICartService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CartService));
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
        public async Task<CartItemDTO[]> AddCartItem(CartItemDTO cartItemDTO)
        {
          //  log.Debug($"Add cart item called, {cartItemDTO.ItemName}");
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
                    VoucherAmount=0,
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

            var cartItemsDTOs = covertToCartItemDTO(cart.CartItems);
            return cartItemsDTOs;
        }

       public  async Task<CartItemDTO[]> RemoveCartItem(string itemName)
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

            var cartItemsDTOs = covertToCartItemDTO(cart.CartItems);
            return cartItemsDTOs;

        }
        public async Task<CartItemDTO[]> UpdateCartItemQuantity(string itemName,bool inc=true)
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
                if(cartItem.Quantity==1)
                    _context.CartItems.Remove(cartItem);
                else
                    cartItem.Quantity--;

                if (cartItem.Price.HasValue)
                {
                    cart.CartValue -= cartItem.Price.Value;
                } 
                
            }
           
            await _context.SaveChangesAsync();

            var cartItemsDTOs=covertToCartItemDTO(cart.CartItems);
            return cartItemsDTOs;

        }

public CartItemDTO[] covertToCartItemDTO(ICollection<CartItem> cartItems)
        {
            var cartItemDTOs = cartItems
                 .Select(item => new CartItemDTO
                 {
                     ItemName = item.ItemName,
                     Quantity = item.Quantity,
                     Price = item.Price

                 })
                 .ToArray();
            return cartItemDTOs;
        }
        public async Task<string> PurchaseCart(int amt,int cartVal)
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
                int voucherAmt=amt;
                foreach (var cartItem in cart.CartItems.ToList())
                {
                    
                    var orderItem = new Order
                    {
                        UserId = userId,
                        Itemname =cartItem.ItemName,
                         Quantity= (int)cartItem.Quantity,
                         TotalPrice= (decimal)(cartItem.Quantity*cartItem.Price),
                         OrderTime=DateTime.Now
                    };  
                _context.Orders.Add(orderItem);
                _context.CartItems.Remove(cartItem);
                }
                if(cartVal>=10 && cartVal < 20)
                {
                 voucherAmt += 50;
                }
                else if (cart.CartValue >= 20 )
                {
                 voucherAmt += 100;
                }
                cart.VoucherAmount = voucherAmt;
                cart.CartValue = 0;
                await _context.SaveChangesAsync();
                if(voucherAmt!=amt)
                {
                return $"Voucher amount of Rs.{voucherAmt - amt} added!";
                }
                return "Purchase successful";
            

        }
        public async Task<List<OrderDTO>> GetOrdersOfUser()
        {/*
            var orders = (from o in _context.Orders
                          join u in _context.Users on userId equals u.Id
                          select new OrderDTO
                          {
                              OrderId = o.OrderId,
                              Itemname = o.Itemname,
                              Quantity = o.Quantity,
                              TotalPrice = o.TotalPrice,
                              OrderTime = o.OrderTime
                   
                          }).ToList(); 
            return orders;*/
            var orders = await _context.Orders
         .Where(b => b.UserId == userId)
         .ToListAsync();

            var ordersDTO = orders.Select(order => new OrderDTO
            {
                OrderId = order.OrderId,
                Itemname = order.Itemname,
                Quantity = order.Quantity,
                TotalPrice = order.TotalPrice,
                OrderTime = order.OrderTime
            }).ToList();

            return ordersDTO;


        }
        public async Task<List<OrderDTO>> GetAllOrders()
        {
            var orders = (from o in _context.Orders
                          join u in _context.Users on o.UserId equals u.Id
                          select new OrderDTO
                          {
                              OrderId=o.OrderId,
                              Itemname = o.Itemname,
                              Quantity = o.Quantity,
                              TotalPrice = o.TotalPrice,
                              OrderTime = o.OrderTime,
                              Username = u.Username
                          }).ToList();
            return orders;
        }
        public async Task<decimal> GetCartValue()
        {
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null)
            {
                return 0;
            }
            return (decimal)cart.CartValue;
        }
        public async Task<string> UpdateVoucherAmount(decimal amt)
        {
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null)
            {
                throw new ArgumentsException("Cart cannot be found");
            }
            cart.VoucherAmount = amt;
            await _context.SaveChangesAsync();
            return $"Voucher amount updated.";
        }
        public async Task<decimal> GetVoucherAmount()
        {
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null)
            {
                throw new ArgumentsException("Cart cannot be found");
            }
            return (decimal)cart.VoucherAmount;
        }
    }
}
