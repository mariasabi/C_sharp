using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Order_management.Interfaces;
using Order_management.Models;
using Order_management.Service;
using Order_management.Exceptions;
using Microsoft.AspNetCore.Authorization;
namespace Order_management.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrder _order;
        
        public OrderController(IOrder orderservice)
        {
            _order=orderservice;
        }
       
        [HttpGet]
        [Route("getItems")]
        public async Task<ActionResult<Item>> GetItems()
        {
            var items=await _order.GetItems();
            if(items.Count==0)
                return NotFound("No items found");
            return Ok(items);
        }
        [HttpGet]
        [Route("getPaginatedItems")]
        public async Task<ActionResult<Item>> GetPaginatedItems(int page=1,int pageSize=10)
        {
            var items = await _order.GetPaginatedItems(page,pageSize);
            if (items.Count() == 0)
                return NotFound("No items found");
            return Ok(items);
        }
        [HttpGet]
        [Route("getItem")]
        public async Task<ActionResult<Item>> GetItem(int id)
        {
            var item = await _order.GetItem(id);
            return Ok(item);
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
        public async Task<ActionResult<Item>> UpdateItem(Item request)
        {
            var item = await _order.UpdateItem(request);
            return Ok(item);
        }
        [HttpDelete]
        [Route("deleteItem")]
        public async Task<ActionResult<Item>> DeleteItem(int id)
        {
            var item = await _order.DeleteItem(id);
            return Ok(item);
        }
    }
}
