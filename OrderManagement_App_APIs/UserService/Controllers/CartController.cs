using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.DTOs;
using UserService.Exceptions;
using UserService.Interfaces;

namespace UserService.Controllers
{
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cart;
        //private readonly IInventoryService _inventoryService;
        public CartController(ICartService cart)//,IInventoryService inventoryService) 
        {
            _cart = cart;
          //  _inventoryService = inventoryService;
        }
        [HttpGet("getCart")]
        public async Task<ActionResult<CartItemDTO>> ViewCart()
        {
            var response = await _cart.ViewCart();
            if (response!=null)
            {
                if (response.Length == 0)
                    return NotFound("No cart items found");
                return Ok(response);
            }
            return NotFound("No cart items found");
        }
        [HttpPost("addCartItem")]
        public async Task<IActionResult> AddCartItem([FromBody]CartItemDTO item)
        {
            try
            {
                var response = await _cart.AddCartItem(item);
                return Ok(response);
            }
            catch (ArgumentsException ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpDelete("removeCartItem")]
        public async Task<IActionResult> RemoveCartItem(string name)
        {
            try
            {
                var response = await _cart.RemoveCartItem(name);
                return Ok(response);
            }
            catch (ArgumentsException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("incrementCartItem")]
        public async Task<IActionResult> IncreaseCartItem([FromBody] string name)
        {
            try
            {
                var response = await _cart.UpdateCartItemQuantity(name);
                return Ok(response);
            }
            catch (ArgumentsException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("decrementCartItem")]
        public async Task<IActionResult> DecreaseCartItem([FromBody] string name)
        {
            try
            {
                var response = await _cart.UpdateCartItemQuantity(name,false);
                return Ok(response);
            }
            catch (ArgumentsException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("purchaseCart")]
        public async Task<IActionResult> PurchaseCart()
        {
            try
            {
                var response = await _cart.PurchaseCart();
                return Ok(response);
            }
            catch (ArgumentsException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("getAllOrders")]
        public async Task<ActionResult<OrderDTO>> GetAllOrders()
        {
            try
            {
                var response = await _cart.GetAllOrders();
                return Ok(response);
            }
            catch (ArgumentsException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("getOrders")]
        public async Task<ActionResult<OrderDTO>> GetOrders()
        {
            try
            {
                var response = await _cart.GetOrdersOfUser();
                return Ok(response);
            }
            catch (ArgumentsException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("getCartValue")]
        public async Task<ActionResult<decimal>> GetCartValue()
        {
            try
            {
                var response = await _cart.GetCartValue();
                return Ok(response);
            }
            catch (ArgumentsException ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}
