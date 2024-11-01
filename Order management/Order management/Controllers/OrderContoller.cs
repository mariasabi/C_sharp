﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Order_management.Model;
using Order_management.Service;

namespace Order_management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderContoller : ControllerBase
    {
        private readonly IOrder _order;
        public OrderContoller(IOrder orderservice)
        {
            _order=orderservice;
        }
        [HttpGet]
        [Route("getItems")]
        public async Task<ActionResult<Item>> GetItems()
        {
            var items=await _order.GetItems();
            return Ok(items);
        }

        [HttpGet]
        [Route("getItem")]
        public async Task<ActionResult<Item>> GetItem(int id)
        {
            var item = await _order.GetItem(id);
            if (item == null)
                return BadRequest("No item found!");

            return Ok(item);
        }

        [HttpPost]
        [Route("addItem")]
        public async Task<ActionResult<Item>> AddItem(Item request)
        {
            var items = await _order.AddItem(request);
            if (items!=null)
                return BadRequest("Such an item already exists");
            
            return Ok(items);
        }

        [HttpPut]
        [Route("updateItem")]
        public async Task<ActionResult<Item>> UpdateItem(Item request)
        {
            var items = await _order.UpdateItem(request);
            if (items == null)
                return BadRequest("No item found!");

            return Ok(items);
        }
        [HttpGet]
        [Route("deleteItem")]
        public async Task<ActionResult<Item>> DeleteItem(int id)
        {
            var item = await _order.DeleteItem(id);
            if (item == null)
                return BadRequest("No item found!");
            return Ok(item);
        }
    }
}
