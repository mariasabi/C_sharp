using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Order_management.Controllers;
using Order_management.Interfaces;
using Order_management.Models;

namespace OrderManagementTests
{
    public class OrderControllerTests
    {
        private readonly Mock<IOrder> _mockOrder;
        private readonly OrderController _controller;
        public OrderControllerTests() 
        {
            _mockOrder = new Mock<IOrder>();
            _controller = new OrderController(_mockOrder.Object);
        }
        List<Item> _items = new List<Item>
        {
            new Item { Id = 1,Name="Vicks",Type="medicine",Quantity=10,Price=30},
            new Item { Id = 2,Name="Dolo",Type="medicine",Quantity=5,Price=3}
        };
        [Fact]
        public async void BulkAddItem_ReturnsOk()
        {
            var response= "Items added successfully ";
            _mockOrder.Setup(x => x.BulkAddItem(It.IsAny<string>())).ReturnsAsync(response);
            var result = await _controller.BulkAddItem(It.IsAny<string>());

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<string>(okResult.Value);
            //Assert.Equal(2, returnValue.Count);
        }
        [Fact]
        public async void GetItems_ReturnsOk()
        {
           
            _mockOrder.Setup(x => x.GetItems()).ReturnsAsync(_items);
            var result = await _controller.GetItems();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<Item>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }
        [Fact]
        public async void GetItems_ReturnsNotFound()
        {

            _mockOrder.Setup(x => x.GetItems()).ReturnsAsync(new List<Item> { });
            var result = await _controller.GetItems();

            var okResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            var returnValue = Assert.IsType<string>("No items found");
            
        }
        [Fact]
        public async void GetPaginatedItems_ReturnsOk()
        {

            _mockOrder.Setup(x => x.GetPaginatedItems(1,2)).ReturnsAsync(_items);
            var result = await _controller.GetPaginatedItems(1,2);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<Item>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }
        [Fact]
        public async void GetPaginatedItems_ReturnsNotFound()
        {

            _mockOrder.Setup(x => x.GetPaginatedItems(3,2)).ReturnsAsync(new List<Item> { });
            var result = await _controller.GetPaginatedItems(3,2);

            var okResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            var returnValue = Assert.IsType<string>("No items found");

        }
        [Fact]
        public async void AddItem_ReturnsOk()
        {
            var newItem=new Item { Id=1,Name="Lays",Type="food",Quantity=10,Price=20 };
            _mockOrder.Setup(x => x.AddItem(newItem)).ReturnsAsync(newItem);
            var result = await _controller.AddItem(newItem);

            var okResult =Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<Item>(okResult.Value);
            //Assert.Equal(2, returnValue.Count);
        }
        [Fact]
        public async void UpdateItem_ReturnsOk()
        {
            var newItem = new Item { Id = 1, Name = "Lays", Type = "food", Quantity = 10, Price = 20 };
            _mockOrder.Setup(x => x.UpdateItem(newItem)).ReturnsAsync(newItem);
            var result = await _controller.UpdateItem(newItem);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<Item>(okResult.Value);
            //Assert.Equal(2, returnValue.Count);
        }
        [Fact]
        public async void DeleteItem_ReturnsOk()
        {
            var newItem = new Item { Id = 1, Name = "Lays", Type = "food", Quantity = 10, Price = 20 };
            _mockOrder.Setup(x => x.DeleteItem(newItem.Id)).ReturnsAsync(newItem);
            var result = await _controller.DeleteItem(newItem.Id);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<Item>(okResult.Value);
            //Assert.Equal(2, returnValue.Count);
        }
    }
}