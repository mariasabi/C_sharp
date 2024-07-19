using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Order_management.Interfaces;
using Order_management.Models;

namespace Order_management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DynamicOrderController : ControllerBase
    {
        private readonly IDynamicOrder _order;

        public DynamicOrderController(IDynamicOrder orderservice)
        {
            _order = orderservice;
        }
        [HttpPost]
        [Route("bulkaddItemsDynamic")]
        public async Task<ActionResult<string>> BulkAddItemDynamic(string filename)
        {
            var response = await _order.BulkAddItem(filename);
            return Ok(response);

        }
        [HttpPut]
        [Route("updateItemDynamic")]
        public async Task<ActionResult<Item>> UpdateItemDynamic(Item request)
        {
            var item = await _order.UpdateItem(request);
            return Ok(item);
        }
    }
}
