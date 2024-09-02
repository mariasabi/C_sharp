using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderService.DTOs;
using OrderService.Exceptions;
using OrderService.Interfaces;
using OrderService.Models;

namespace OrderService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class OrderController : ControllerBase
    {
        private readonly IOrder _order;

        public OrderController(IOrder orderservice)
        {
            _order = orderservice;
        }

        [HttpGet]
        [Route("getItems")]
        [Authorize]
        public async Task<ActionResult<List<ItemViewDTO>>> GetItems()
        {
            var items = await _order.GetItems();
            if (items.Count == 0)
                return NotFound("No items found");
            return Ok(items);
        }
        [HttpGet]
        [Route("getPaginatedItems")]
        [Authorize]
        public async Task<ActionResult<List<Item>>> GetPaginatedItems(int page = 1, int pageSize = 10)
        {
            try
            {
                var items = await _order.GetPaginatedItems(page, pageSize);
                if (items.Count == 0)
                    return NotFound("No items found");
                return Ok(items);
            }
            catch (ArgumentsException ex)
            { return BadRequest(ex.Message); }
        }
        [HttpGet]
        [Route("getItem")]
        [Authorize]
        public async Task<ActionResult<ItemViewDTO>> GetItem(int id)
        {
            try
            {
                var item = await _order.GetItem(id);
                return Ok(item);
            }
            catch (IdNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpGet]
        [Route("getItemByName")]
        [Authorize]
        public async Task<ActionResult<ItemViewDTO>> GetItemByName(string name)
        {
            try
            {
                var item = await _order.GetItemByName(name);
                return Ok(item);
            }
            catch (IdNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        [Route("addItem")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Item>> AddItem([FromForm] ItemDTO request)
        {
            try
            {
                var item = await _order.AddItem(request);
                return Ok(item);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message); 
            }
           
        }

        [HttpPut]
        [Route("updateItem")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Item>> UpdateItemDynamic([FromForm] ItemDTO request)       
        {
            try
            {
                var item = await _order.UpdateItem(request);
                return Ok(item);
            }
            catch (IdNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpPut]
        [Route("updateItemQuantity")]
       
        public async Task<ActionResult<Item>> UpdateItemQuantity(string itemname,int quantity)
        {
            try
            { 
                var item = await _order.UpdateItemQuantity(itemname,quantity);
                return Ok(item);
            }
            catch (IdNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete]
        [Route("deleteItem")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Item>> DeleteItem(int id)
        {  
            try
            {
            var item = await _order.DeleteItem(id);
            return Ok(item);
            }
            catch (IdNotFoundException ex)
            {
                return NotFound(ex.Message);
            }

        }
        [HttpPost]
        [Route("bulkaddItemsDynamic")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<string>> BulkAddItemDynamic(IFormFile file)
        {
            try
            {
               // var file = Request.Form.Files[0];


                if (file.Length > 0)
                {

                    var response = await _order.BulkAddItem(file);

                    return Ok(response);
                }
                else { return BadRequest("File is empty"); }

            }
            catch (CSVException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }

        }
        [HttpGet]
        [Route("searchItem")]
        [Authorize]
        public async Task<ActionResult<List<ItemViewDTO>>> SearchItem(string name)
        {
            try
            {
                var items = await _order.SearchItemByName(name);
                return Ok(items);
            }
            catch (IdNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
          
        }

    }

}
