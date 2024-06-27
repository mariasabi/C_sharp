using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Order_management.Exceptions;
using Order_management.Interfaces;
using Order_management.Models;
using Order_management.Service;
using Order_management.Exceptions;
using System.Collections.Generic;
namespace OrderManagementTests
{
    public class OrderServiceTests
    {
      
       private readonly Mock<IWebHostEnvironment> _webHostEnvironment;
      
        public OrderServiceTests()
        {
           _webHostEnvironment = new Mock<IWebHostEnvironment>();
        }
        private async Task<OrderManagementContext> GetDatabaseContext(int count = 3)
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var optionsBuilder = new DbContextOptionsBuilder<OrderManagementContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .UseInternalServiceProvider(serviceProvider);
            var context = new OrderManagementContext(optionsBuilder.Options);
            
            if (await context.Items.CountAsync() <= 0)
            {
                for (int i = 0; i < count; i++)
                {
                    context.Items.Add(sampleItems[i]);
                    await context.SaveChangesAsync();
                }
            }

            return context;
        }
      
        private List<Item> sampleItems = new List<Item>()
        {
            new Item{Id=1,Name="Vicks",Type="medicine",Quantity=10,Price=30},
            new Item{Id=2,Name="Dolo",Type="medicine",Quantity=5,Price=3},
            new Item{Id=3,Name="Realme buds",Type="earphone",Quantity=2,Price=600}
        };
        private List<Item> sampleItemsForBulk = new List<Item>()
        {
            new Item{Id=1,Name="Vicks",Type="medicine",Quantity=10},
            new Item{Id=2,Name="Dolo",Type="medicine",Quantity=5},
        };

        [Fact]
        public async void GetItems_ReturnsOneOrMoreItems()
        {
            var _mockContext = await GetDatabaseContext();
            var _orderRepository = new Order(_mockContext,_webHostEnvironment.Object);
            var result= await _orderRepository.GetItems();

            Assert.NotEmpty(result);
            Assert.IsType<List<Item>>(result);
            Assert.Equal(3, result.Count);
        }
        [Fact]
        public async void GetItems_ReturnsZeroItems()
        {
            var _mockContext = await GetDatabaseContext(0);
            var _orderRepository = new Order(_mockContext, _webHostEnvironment.Object);
            var result = await _orderRepository.GetItems();

            Assert.Empty(result);
            Assert.IsType<List<Item>>(result);

        }
        [Fact]
        public async void GetItem_InvalidId_ThrowsException()
        {
            var _mockContext = await GetDatabaseContext();
            var _orderRepository = new Order(_mockContext, _webHostEnvironment.Object);


            await Assert.ThrowsAsync<IdNotFoundException>(() => _orderRepository.GetItem(7));

        }
        [Fact]
        public async void GetItem_ValidId_ReturnsOneOrMoreItems()
        {
            var _mockContext = await GetDatabaseContext();
            var _orderRepository = new Order(_mockContext, _webHostEnvironment.Object);
            var result = await _orderRepository.GetItem(2);

            Assert.NotNull(result);
            Assert.Equal(2, result.Id);
            Assert.Equal("Dolo", result.Name);
            Assert.Equal("medicine", result.Type);
            Assert.Equal(5, result.Quantity);
            Assert.Equal(3, result.Price);
        }
        [Fact]
        public async void AddItem_ReturnsItemAdded()
        {
            var newItem = new Item { Name = "Classmate notebook", Type = "stationary", Quantity = 50, Price = 40 };
            var _mockContext = await GetDatabaseContext();
            var _orderRepository = new Order(_mockContext, _webHostEnvironment.Object);
            var result = await _orderRepository.AddItem(newItem);

            Assert.NotNull(result);
            Assert.IsType<Item>(result);
            Assert.Equal(newItem, result);
           
        }
        [Fact]
        public async void UpdateItem_InvalidId_ThrowsException()
        {
            var newItem = new Item { Id=4,Name = "Classmate notebook", Type = "stationary", Quantity = 50, Price = 40 };
            var _mockContext = await GetDatabaseContext();
            var _orderRepository = new Order(_mockContext, _webHostEnvironment.Object);


            await Assert.ThrowsAsync<IdNotFoundException>(() => _orderRepository.UpdateItem(newItem));

        }
        [Fact]
        public async void UpdateItem_ValidId_ReturnsUpdatedItem()
        {
            var newItem = new Item { Id = 2, Name = "Classmate notebook", Type = "stationary", Quantity = 50, Price = 40 };
            var _mockContext = await GetDatabaseContext();
            var _orderRepository = new Order(_mockContext, _webHostEnvironment.Object);
            var result = await _orderRepository.UpdateItem(newItem);
           
            Assert.NotNull(result);
            Assert.Equal(newItem, result);
            Assert.IsType<Item>(result);
        }
        [Fact]
        public async void DeleteItem_InvalidId_ThrowsException()
        {
           
            var _mockContext = await GetDatabaseContext();
            var _orderRepository = new Order(_mockContext, _webHostEnvironment.Object);


            await Assert.ThrowsAsync<IdNotFoundException>(() => _orderRepository.DeleteItem(5));

        }
        [Fact]
        public async void DeleteItem_ValidId_ReturnsDeletedItem()
        {
           
            var _mockContext = await GetDatabaseContext();
            var _orderRepository = new Order(_mockContext, _webHostEnvironment.Object);
            var result = await _orderRepository.DeleteItem(2);

            Assert.NotNull(result);
            Assert.Equal(sampleItems[1], result);
            Assert.IsType<Item>(result);
        }
        [Fact]
        public async void GetPaginatedItems_ValidPageNo_ReturnsOneOrMoreItems()
        {
            var _mockContext = await GetDatabaseContext();
            var _orderRepository = new Order(_mockContext, _webHostEnvironment.Object);
            var result = await _orderRepository.GetPaginatedItems(2,2);

            Assert.NotEmpty(result);
            Assert.IsType<List<Item>>(result);
            Assert.Single(result);

        }
        [Fact]
        public async void GetPaginatedItems_InvalidPageNo_ReturnsZeroItems()
        {
            var _mockContext = await GetDatabaseContext();
            var _orderRepository = new Order(_mockContext, _webHostEnvironment.Object); 
            await Assert.ThrowsAsync<ArgumentsException>(() => _orderRepository.GetPaginatedItems(3, 2));

        }

      
    }

}
