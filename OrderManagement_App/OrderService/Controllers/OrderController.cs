using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderService.Exceptions;
using OrderService.Interfaces;
using OrderService.Models;

namespace OrderService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrder _order;

        public OrderController(IOrder orderservice)
        {
            _order = orderservice;
        }

        [HttpGet]
        [Route("getItems")]
        public async Task<ActionResult<Item>> GetItems()
        {
            var items = await _order.GetItems();
            if (items.Count == 0)
                return NotFound("No items found");
            return Ok(items);
        }
        [HttpGet]
        [Route("getPaginatedItems")]
        public async Task<ActionResult<Item>> GetPaginatedItems(int page = 1, int pageSize = 10)
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
        public async Task<ActionResult<Item>> GetItem(int id)
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

        [HttpPost]
        [Route("addItem")]
        public async Task<ActionResult<Item>> AddItem(Item request)
        {
            var item = await _order.AddItem(request);
            return Ok(item);
        }

        [HttpPut]
        [Route("updateItem")]
        public async Task<ActionResult<Item>> UpdateItemDynamic(Item request)
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

        [HttpDelete]
        [Route("deleteItem")]
        public async Task<ActionResult<Item>> DeleteItem(int id)
        {
            var item = await _order.DeleteItem(id);
            return Ok(item);
        }
        [HttpPost]
        [Route("bulkaddItemsDynamic")]
        public async Task<ActionResult<string>> BulkAddItemDynamic()
        {
            try
            {
                var file = Request.Form.Files[0];


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
    }

}
