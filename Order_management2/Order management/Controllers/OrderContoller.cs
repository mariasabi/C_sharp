using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Order_management.Interfaces;
using Order_management.Models;
using Order_management.Service;

namespace Order_management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderContoller : ControllerBase
    {
        private readonly IOrder _order;
        private static readonly ILog log = LogManager.GetLogger(typeof(OrderContoller));
        public OrderContoller(IOrder orderservice)
        {
            _order=orderservice;
        }
        [HttpGet]
        [Route("getItems")]
        public async Task<ActionResult<Item>> GetItems()
        {
            var items=await _order.GetItems();
            if (items == null)
            {
                log.Debug("No item found!");
                return BadRequest("No item found!");
            }
            log.Info("Retrieved items");
            return Ok(items);
        }

        [HttpGet]
        [Route("getItem")]
        public async Task<ActionResult<Item>> GetItem(int id)
        {
            var item = await _order.GetItem(id);
            if (item == null)
            {
                log.Debug("No item found!");
                return BadRequest("No item found!");
            }
            log.Info("Retrieved item");
            return Ok(item);
        }

        [HttpPost]
        [Route("addItem")]
        public async Task<ActionResult<Item>> AddItem(Item request)
        {
            var item = await _order.AddItem(request);
            // if (item==null)
            //   return BadRequest("Such an item already exists!");
            log.Info("Item added");
            return Ok(item);
        }

        [HttpPut]
        [Route("updateItem")]
        public async Task<ActionResult<Item>> UpdateItem(Item request)
        {
            var item = await _order.UpdateItem(request);
            if (item == null)
            {
                log.Debug("No item found!");
                return BadRequest("No item found!");
            }
            log.Info("Item updated");
            return Ok(item);
        }
        [HttpGet]
        [Route("deleteItem")]
        public async Task<ActionResult<Item>> DeleteItem(int id)
        {
            var item = await _order.DeleteItem(id);
            if (item == null)
            {
                log.Debug("No item found!");
                return BadRequest("No item found!");
            }
            log.Info("Item deleted");
            return Ok(item);
        }
    }
}
