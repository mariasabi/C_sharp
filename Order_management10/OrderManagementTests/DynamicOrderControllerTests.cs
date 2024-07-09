using Microsoft.AspNetCore.Mvc;
using Moq;
using Order_management.Controllers;
using Order_management.Interfaces;
using Order_management.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementTests
{
    public class DynamicOrderControllerTests
    {
        private readonly Mock<IDynamicOrder> _mockOrder;
        private readonly DynamicOrderController _controller;
        public DynamicOrderControllerTests()
        {
            _mockOrder = new Mock<IDynamicOrder>();
            _controller = new DynamicOrderController(_mockOrder.Object);
        }
        [Fact]
        public async void BulkAddItem_ReturnsOk()
        {
            var response = "Items added successfully ";
            _mockOrder.Setup(x => x.BulkAddItem(It.IsAny<string>())).ReturnsAsync(response);
            var result = await _controller.BulkAddItemDynamic(It.IsAny<string>());

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<string>(okResult.Value);
            
        }
        [Fact]
        public async void UpdateItem_ReturnsOk()
        {
            var newItem = new Item { Id = 1, Name = "Lays", Type = "food", Quantity = 10, Price = 20 };
            _mockOrder.Setup(x => x.UpdateItem(newItem)).ReturnsAsync(newItem);
            var result = await _controller.UpdateItemDynamic(newItem);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<Item>(okResult.Value);
          
        }
    }
}
